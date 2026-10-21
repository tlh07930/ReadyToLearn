using Microsoft.EntityFrameworkCore;
using Ready2Learn.Models;
using Ready2Learn.Security;

namespace Ready2Learn.Data.Services
{
    public class StudentsService : IStudentsService
    {
        private readonly Ready2LearnDbContext _context;

        public StudentsService(Ready2LearnDbContext context)
        {
            _context = context;
        }

        public async Task AddStudent(Student student)
        {
            // Note: password must be set via SetStudentPasswordAsync if needed.
            await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteStudentAsync(Student student)
        {
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Student>> GetAllStudentsByTeacherIdAsync(int teacherId)
        {
            IEnumerable<Student> students = await _context.Students.Where(s => s.TeacherID == teacherId).ToListAsync();
            return students;
        }

        public async Task<Student> GetStudentByIdAsync(int studentId)
        {
            Student? student = await _context.Students.FirstOrDefaultAsync(s => s.ID == studentId);
            return student;
        }

        public async Task<Student> GetStudentByName(string firstName, string lastName)
        {
            Student? student = await _context.Students.FirstOrDefaultAsync(s => s.FirstName.Equals(firstName) && s.LastName.Equals(lastName));
            return student;
        }
        
        public async Task<int> GetTeacherIdByStudentIdAsync(int studentId)
        {
            Student? student = await _context.Students.FirstOrDefaultAsync(s => s.ID == studentId);
            return student?.TeacherID ?? 0;
        }

        public async Task ResetStudentPasswordAsync(int studentId)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.ID == studentId);
            if (student == null) return;

            student.PasswordHash = null;
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
        }

        public async Task SetStudentPasswordAsync(int studentId, List<string> password)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.ID == studentId);
            if (student == null) return;

            var plaintext = string.Join("|", password);
            student.PasswordHash = PasswordHashing.CreateHash(plaintext);
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
        }

        public async Task<Student> UpdateStudentAsync(Student newStudent)
        {
            // Note: to change a student's password, call SetStudentPasswordAsync
            _context.Update(newStudent);
            await _context.SaveChangesAsync();
            return newStudent;
        }

        public async Task<Student> ValidateLogInAsync(string firstName, string lastName, List<string> password)
        {
            Student? student = await _context.Students.FirstOrDefaultAsync(s => s.FirstName.Equals(firstName) && s.LastName.Equals(lastName));
            if (student == null) return null;
            if (string.IsNullOrEmpty(student.PasswordHash))
            {
                // No password set
                return null;
            }

            var plaintext = string.Join("|", password);
            bool ok = PasswordHashing.VerifyHash(student.PasswordHash, plaintext);
            return ok ? student : null;
        }
    }
}