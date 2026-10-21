using Microsoft.AspNetCore.Mvc;
using Ready2Learn.Data.Services;
using Ready2Learn.Models;

namespace Ready2Learn.Controllers
{
    public class StudentController : Controller
    {
        private readonly IStudentsService _studentService;

        public StudentController(IStudentsService studentService)
        {
            _studentService = studentService;
        }

        // Account/{teacherId}/AddStudent
        public IActionResult AddStudent()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddStudent([Bind("FirstName,LastName")] Student student)
        {
            student.TeacherID = int.Parse(Request.Cookies["TeacherID"]);
            if (!ModelState.IsValid)
            {
                return View("Students");
            }
            await _studentService.AddStudent(student);

            IEnumerable<Student> students = await _studentService.GetAllStudentsByTeacherIdAsync(student.TeacherID);
            return View("Students", students);
        }

        // Account/{teacherId}/DeleteStudentAccount/{studentId}
        public async Task<IActionResult> DeleteStudentAccount(int id)
        {
            Student student = await _studentService.GetStudentByIdAsync(id);

            if (student == null) return View("NotFound");
            return View(student);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteStudentConfirmed(int id)
        {
            Student student = await _studentService.GetStudentByIdAsync(id);
            if (student == null) return View("NotFound");

            await _studentService.DeleteStudentAsync(student);

            IEnumerable<Student> students = await _studentService.GetAllStudentsByTeacherIdAsync(student.TeacherID);
            return View("Students", students);
        }

        // Account/{teacherId}/EditStudentDetails/{studentId}
        public async Task<IActionResult> EditStudentDetails(int id)
        {
            Student student = await _studentService.GetStudentByIdAsync(id);

            if (student == null) return View("NotFound");
            return View(student);
        }

        [HttpPost]
        public async Task<IActionResult> EditStudentDetails(Student student)
        {
            if (!ModelState.IsValid)
            {
                // Return the edit view so validation errors can be corrected
                return View(student);
            }

            // Retrieve the tracked entity from the DB/context
            var existing = await _studentService.GetStudentByIdAsync(student.ID);
            if (existing == null) return View("NotFound");

            // Only update the editable fields we want to change
            existing.FirstName = student.FirstName;
            existing.LastName = student.LastName;

            // Persist changes on the tracked entity
            await _studentService.UpdateStudentAsync(existing);

            // Return to the students list for this teacher
            IEnumerable<Student> students = await _studentService.GetAllStudentsByTeacherIdAsync(existing.TeacherID);
            return View("Students", students);
        }

        // Account/{teacherId}/ResetStudentPassword/{studentId}
        public async Task<IActionResult> ResetStudentPassword(int id)
        {
            Student student = await _studentService.GetStudentByIdAsync(id);
            if (student == null) return View("NotFound");

            student.PasswordHash = null;
            await _studentService.UpdateStudentAsync(student);

            IEnumerable<Student> students = await _studentService.GetAllStudentsByTeacherIdAsync(student.TeacherID);
            return View("Students", students);
        }

        public async Task<IActionResult> StudentLogIn(int teacherId)
        {
            Response.Cookies.Delete("TeacherID");
            IEnumerable<Student> students = await _studentService.GetAllStudentsByTeacherIdAsync(teacherId);
            if (students == null) return View("NotFound");
            return View("StudentLogIn", students);
        }

        // Account/{teacherId}/Students
        public async Task<IActionResult> Students(int teacherId)
        {
            IEnumerable<Student> students = await _studentService.GetAllStudentsByTeacherIdAsync(teacherId);
            if (students == null) return View("NotFound");
            return View(students);
        }
    }
}
