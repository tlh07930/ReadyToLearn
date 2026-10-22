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

        // Public family links use the pattern: /{classLink}
        [HttpGet("/{classLink}", Order = 100)]
        public async Task<IActionResult> Index(string classLink)
        {
            if (string.IsNullOrWhiteSpace(classLink))
            {
                return NotFound();
            }

            // Look up the adult account by its family link.
            var teacher = await _teachersService.GetTeacherByClassLinkAsync(classLink.Trim());

            if (teacher == null)
            {
                return View("ClassNotFound");
            }

            // Redirect to the student/child login list for the adult account.
            return RedirectToAction("StudentLogIn", "Student", new { teacherId = teacher.ID });
        }
    }
}