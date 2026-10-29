using Microsoft.EntityFrameworkCore;
using Ready2Learn.Models;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace Ready2Learn.Data.Services
{
    public class TeachersService : ITeachersService
    {
        private readonly Ready2LearnDbContext _context;

        public TeachersService(Ready2LearnDbContext context)
        {
            _context = context;
        }

        public async Task AddTeacherAsync(Teacher teacher)
        {
            // Controller is responsible for setting PasswordHash from provided plaintext before calling this method.
            await _context.Teachers.AddAsync(teacher);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTeacherAsync(Teacher teacher)
        {
            // Get a list of all the students that belong to the teacher
            List<Student> students = await _context.Students.Where(s => s.TeacherID == teacher.ID).ToListAsync();
            foreach (Student student in students)
            {
                // Remove all results for the students associated with the teacher
                List<Result> results = await _context.Results.Where(r => r.StudentID == student.ID).ToListAsync();
                foreach (Result result in results)
                {
                    _context.Results.Remove(result);
                }

                // Remove all students associated with the teacher
                _context.Students.Remove(student);
            }

            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();
        }

        public async Task<Teacher> GetTeacherByClassLinkAsync(string classLink)
        {
            if (string.IsNullOrWhiteSpace(classLink)) return null;

            // Normalize and compare case-insensitively and trimmed to avoid mismatches from URL casing or whitespace.
            var normalized = classLink.Trim().ToUpperInvariant();
            Teacher? teacher = await _context.Teachers
                .FirstOrDefaultAsync(t => t.ClassLink != null && t.ClassLink.ToUpper() == normalized);
            return teacher;
        }

        public async Task<Teacher> GetTeacherByIdAsync(int id)
        {
            Teacher? teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.ID == id);
            return teacher;
        }

        public async Task<Teacher> GetTeacherByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return null;

            var normalized = username.Trim();
            var normalizedUpper = normalized.ToUpperInvariant();

            Teacher? teacher = await _context.Teachers
                .FirstOrDefaultAsync(t => t.Username != null && t.Username.ToUpper() == normalizedUpper);

            return teacher;
        }

        public async Task<Teacher> UpdateTeacherAsync(Teacher newTeacher)
        {
            // Controller should set PasswordHash if a new password was provided.
            _context.Update(newTeacher);
            await _context.SaveChangesAsync();
            return newTeacher;
        }

        public async Task<Teacher> ValidateLoginAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password)) return null;

            // Normalize username for lookup to avoid case-sensitivity issues.
            // Use invariant uppercase to make EF translate successfully to SQL.
            var normalized = username.Trim().ToUpperInvariant();

            Teacher? teacher = await _context.Teachers
                .FirstOrDefaultAsync(t => t.Username != null && t.Username.ToUpper() == normalized);

            if (teacher == null) return null;

            if (string.IsNullOrEmpty(teacher.PasswordHash)) return null;

            bool ok = Ready2Learn.Security.PasswordHashing.VerifyHash(teacher.PasswordHash, password);
            return ok ? teacher : null;
        }
    }
}