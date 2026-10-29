using Ready2Learn.Models;

namespace Ready2Learn.Data.Services
{
    public interface ITeachersService
    {
        Task AddTeacherAsync(Teacher teacher);
        Task DeleteTeacherAsync(Teacher teacher);
        Task<Teacher> GetTeacherByClassLinkAsync(string classLink);
        Task<Teacher> GetTeacherByIdAsync(int id);
        Task<Teacher> GetTeacherByUsernameAsync(string username);
        Task<Teacher> UpdateTeacherAsync(Teacher newTeacher);
        Task<Teacher> ValidateLoginAsync(string username, string password);
    }
}