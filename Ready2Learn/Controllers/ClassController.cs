using Microsoft.AspNetCore.Mvc;
using Ready2Learn.Data.Services;
using System.Threading.Tasks;

namespace Ready2Learn.Controllers
{
    public class ClassController : Controller
    {
        private readonly ITeachersService _teachersService;

        public ClassController(ITeachersService teachersService)
        {
            _teachersService = teachersService;
        }

        // Public class links use the pattern: /class/{classLink}
        [HttpGet("/class/{classLink}")]
        public async Task<IActionResult> Index(string classLink)
        {
            if (string.IsNullOrWhiteSpace(classLink))
            {
                return NotFound();
            }

            // Look up teacher by class link (service will handle case/trim)
            var teacher = await _teachersService.GetTeacherByClassLinkAsync(classLink.Trim());
            if (teacher == null)
            {
                // Return a NotFound or a friendly view for missing class links.
                return View("ClassNotFound");
            }

            // Redirect to StudentController.StudentLogIn to show the student login list for that teacher.
            return RedirectToAction("StudentLogIn", "Student", new { teacherId = teacher.ID });
        }
    }
}