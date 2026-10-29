using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Ready2Learn.Data.Services;
using Ready2Learn.Models;
using Ready2Learn.Security;

namespace Ready2Learn.Controllers
{
    public class LogInController : Controller
    {
        private readonly IStudentsService _studentsService;
        private readonly ITeachersService _teacherService;

        public LogInController(IStudentsService studentsService, ITeachersService teacherService)
        {
            _studentsService = studentsService;
            _teacherService = teacherService;
        }

        public IActionResult CreateAccount()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount(TeacherCreateViewModel model)
        {
            // Ensure ClassLink is unique if provided
            if (!string.IsNullOrWhiteSpace(model.ClassLink))
            {
                var existing = await _teacherService.GetTeacherByClassLinkAsync(model.ClassLink);
                if (existing != null)
                {
                    ModelState.AddModelError("ClassLink", "This class link is already in use. Please choose a different class link.");
                }
            }

            if (!ModelState.IsValid)
            {
                var baseUrl = $"{Request.Scheme}://{Request.Host}/class/";
                ViewData["ClassLinkBase"] = baseUrl;
                return View(model);
            }

            // Map ViewModel to entity and hash password
            var teacher = new Teacher
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Grade = model.Grade,
                Username = model.Username,
                ClassLink = model.ClassLink,
                PasswordHash = PasswordHashing.CreateHash(model.Password ?? string.Empty)
            };

            await _teacherService.AddTeacherAsync(teacher);
            return RedirectToAction("LogIn");
        }

        public IActionResult Home()
        {
            return View();
        }

        public IActionResult LogIn()
        {
            if (Request.Cookies.ContainsKey("TeacherID"))
            {
                return RedirectToAction("LogOut");
            }
            if (Request.Cookies.ContainsKey("StudentID"))
            {
                return RedirectToAction("LogOut");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogIn([Bind("Username,Password")] LoginViewModel login)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }

            try
            {
                Teacher loggedInTeacher = await _teacherService.ValidateLoginAsync(login.Username, login.Password);
                if (loggedInTeacher == null)
                {
                    ModelState.AddModelError("Password", "Incorrect username or password.");
                    return View(login);
                }

                Response.Cookies.Append("TeacherID", loggedInTeacher.ID.ToString());
                return RedirectToAction("Home");
            }
            catch (SqlException ex) when (IsTransient(ex))
            {
                // Immediate retry before giving up
                try
                {
                    Teacher retryTeacher = await _teacherService.ValidateLoginAsync(login.Username, login.Password);
                    if (retryTeacher != null)
                    {
                        Response.Cookies.Append("TeacherID", retryTeacher.ID.ToString());
                        return RedirectToAction("Home");
                    }
                }
                catch (SqlException)
                {
                    // Do not hold the browser up — retry in background
                    _ = RetryTeacherLoginInBackground(login.Username, login.Password);

                    TempData["Message"] = "The database is temporarily unavailable. We are retrying in the background.";
                    ModelState.AddModelError("Password", "The database is temporarily unavailable. Please try again.");
                    return View(login);
                }

                ModelState.AddModelError("Password", "Incorrect username or password.");
                return View(login);
            }
        }

        private async Task RetryTeacherLoginInBackground(string username, string password)
        {
            var delays = new[] { 2, 5, 10, 15 };

            foreach (var delay in delays)
            {
                await Task.Delay(TimeSpan.FromSeconds(delay));

                try
                {
                    var teacher = await _teacherService.ValidateLoginAsync(username, password);
                    if (teacher != null)
                    {
                        // Optional: log success or update session if you want
                        return;
                    }
                }
                catch (SqlException ex) when (IsTransient(ex))
                {
                    // keep retrying
                }
            }
        }

        private static bool IsTransient(SqlException ex)
        {
            return ex.Number == 40615 ||
                   ex.Number == 40197 ||
                   ex.Number == 49918 ||
                   ex.Number == 18456 ||
                   ex.Number == 1205 ||
                   ex.Number == 121;
        }

        public async Task<IActionResult> LogOut()
        {
            if (Request.Cookies.ContainsKey("TeacherID"))
            {
                Response.Cookies.Delete("TeacherID");
            }

            if (Request.Cookies.ContainsKey("StudentID"))
            {
                // Look up teacher id to redirect to the correct student login page
                int teacherId = await _studentsService.GetTeacherIdByStudentIdAsync(int.Parse(Request.Cookies["StudentID"]));
                Response.Cookies.Delete("StudentID");
                return RedirectToAction("StudentLogIn", "Student", new { teacherId });
            }

            return RedirectToAction("LogIn");
        }

        public IActionResult StudentLogIn()
        {
            if (Request.Cookies.ContainsKey("TeacherID"))
            {
                return RedirectToAction("LogOut");
            }
            if (Request.Cookies.ContainsKey("StudentID"))
            {
                return RedirectToAction("LogOut");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> StudentLogIn([Bind("FirstName,LastName")] Student student,
            string selectedPasswordImage_0, string selectedPasswordImage_1, string selectedPasswordImage_2, string selectedPasswordImage_3,
            int teacherId = 0)
        {
            IEnumerable<Student> studentsForView = null;
            if (teacherId != 0)
            {
                studentsForView = await _studentsService.GetAllStudentsByTeacherIdAsync(teacherId);
            }
            else
            {
                studentsForView = new List<Student>();
            }

            if (string.IsNullOrWhiteSpace(student.FirstName) || string.IsNullOrWhiteSpace(student.LastName))
            {
                ViewData["StudentSelectError"] = "Please select a student.";
                return View("~/Views/Student/StudentLogIn.cshtml", studentsForView);
            }

            Student studentToLogIn = await _studentsService.GetStudentByName(student.FirstName, student.LastName);
            if (studentToLogIn == null)
            {
                ViewData["StudentSelectError"] = "Student name not found. Please check the first and last name.";
                return View("~/Views/Student/StudentLogIn.cshtml", studentsForView);
            }

            List<string> postedPassword = new List<string>() { selectedPasswordImage_0, selectedPasswordImage_1, selectedPasswordImage_2, selectedPasswordImage_3 };

            // If no password hash exists, set the student's password via the service (first-time setup)
            if (string.IsNullOrEmpty(studentToLogIn.PasswordHash))
            {
                await _studentsService.SetStudentPasswordAsync(studentToLogIn.ID, postedPassword);

                Response.Cookies.Append("StudentID", studentToLogIn.ID.ToString());
                return RedirectToAction("Home");
            }

            Student validated = await _studentsService.ValidateLogInAsync(student.FirstName, student.LastName, postedPassword);
            if (validated == null)
            {
                ViewData["PasswordImagesError"] = "Incorrect password. Please try again.";
                return View("~/Views/Student/StudentLogIn.cshtml", studentsForView);
            }

            Response.Cookies.Append("StudentID", validated.ID.ToString());
            return RedirectToAction("Home");
        }
    }
}