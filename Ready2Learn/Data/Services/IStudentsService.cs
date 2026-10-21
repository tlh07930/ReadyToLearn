using Ready2Learn.Models;

namespace Ready2Learn.Data.Services
{
    public interface IStudentsService
    {
        Task AddStudent(Student student);
        Task DeleteStudentAsync(Student student);
        Task<IEnumerable<Student>> GetAllStudentsByTeacherIdAsync(int teacherId);
        Task<Student> GetStudentByIdAsync(int studentId);
        Task<Student> GetStudentByName(string firstName, string lastName);
        Task<int> GetTeacherIdByStudentIdAsync(int studentId);
        Task ResetStudentPasswordAsync(int studentId);
        Task SetStudentPasswordAsync(int studentId, List<string> password);
        Task<Student> UpdateStudentAsync(Student newStudent);
        Task<Student> ValidateLogInAsync(string firstName, string lastName, List<string> password);
    }
}