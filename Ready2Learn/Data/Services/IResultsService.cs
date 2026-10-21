using Ready2Learn.Models;

namespace Ready2Learn.Data.Services
{
    public interface IResultsService
    {
        Task<List<Result>> GetResultsByStudentId(int id);
        Task<List<Result>> GetResultsByTeacherId(int id);
        Task<List<Result>> GetResultsByTeacherIdFiltered(int teacherId, int? studentId = null, string? subject = null, int? gameId = null);
        Task SubmitResultAsync(Result result);
    }
}
