using Microsoft.EntityFrameworkCore;
using Ready2Learn.Models;

namespace Ready2Learn.Data.Services
{
    public class ResultsService : IResultsService
    {
        private readonly Ready2LearnDbContext _context;

        public ResultsService(Ready2LearnDbContext context)
        {
            _context = context;
        }

        public async Task<List<Result>> GetResultsByStudentId(int id)
        {
            List<Result>? results = await _context.Results.Where(s => s.StudentID == id).ToListAsync();
            return results;
        }

        public async Task<List<Result>> GetResultsByTeacherId(int id)
        {
            List<int> studentIds = await _context.Students.Where(s => s.TeacherID == id).Select(s => s.ID).ToListAsync();
            List<Result>? results = await _context.Results.Where(r => studentIds.Contains(r.StudentID)).ToListAsync();
            return results;
        }

        public async Task<List<Result>> GetResultsByTeacherIdFiltered(int teacherId, int? studentId = null, string? subject = null, int? gameId = null)
        {
            // Get teacher's student IDs
            List<int> studentIds = await _context.Students.Where(s => s.TeacherID == teacherId).Select(s => s.ID).ToListAsync();

            // Base query: results for that teacher's students, include navigation properties needed for projection
            IQueryable<Result> query = _context.Results
                                               .Include(r => r.Game)
                                               .Include(r => r.Student)
                                               .Where(r => studentIds.Contains(r.StudentID));

            if (studentId.HasValue)
            {
                query = query.Where(r => r.StudentID == studentId.Value);
            }

            if (gameId.HasValue)
            {
                query = query.Where(r => r.GameID == gameId.Value);
            }

            if (!string.IsNullOrEmpty(subject))
            {
                query = query.Where(r => r.Game != null && r.Game.Subject == subject);
            }

            List<Result> results = await query.ToListAsync();
            return results;
        }

        public async Task SubmitResultAsync(Result result)
        {
            _context.Results.Add(result);
            await _context.SaveChangesAsync();
        }
    }
}
