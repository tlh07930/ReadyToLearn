using Microsoft.AspNetCore.Mvc;
using Ready2Learn.Data.Services;
using Ready2Learn.Models;
using Ready2Learn.Security;

namespace Ready2Learn.Controllers
{
    public class AccountController : Controller
    {
        private readonly ITeachersService _teacherService;

        public AccountController(ITeachersService teacherService)
        {
            _teacherService = teacherService;
        }

        // Account/{teacherId}
        public async Task<IActionResult> Account(int id)
        {
            var teacherAccount = await _teacherService.GetTeacherByIdAsync(id);

            if (teacherAccount == null) return View("NotFound");

            ViewData["ClassLinkFull"] = $"{Request.Scheme}://{Request.Host}/class/{teacherAccount.ClassLink ?? string.Empty}";

            return View("Account", teacherAccount);
        }

        // Account/{teacherId}/Edit
        public async Task<IActionResult> EditAccount(int id)
        {
            var teacherAccount = await _teacherService.GetTeacherByIdAsync(id);

            if (teacherAccount == null) return View("NotFound");
            return View(teacherAccount);
        }

        [HttpPost]
        public async Task<IActionResult> EditAccount(Teacher teacher, string Password, string ConfirmPassword)
        {
            // Load the existing tracked teacher from the DB
            var existing = await _teacherService.GetTeacherByIdAsync(teacher.ID);
            if (existing == null) return View("NotFound");

            // Check ClassLink uniqueness only if it changed
            if (!string.IsNullOrWhiteSpace(teacher.ClassLink) &&
                !string.Equals(teacher.ClassLink, existing.ClassLink, StringComparison.OrdinalIgnoreCase))
            {
                var other = await _teacherService.GetTeacherByClassLinkAsync(teacher.ClassLink);
                if (other != null && other.ID != existing.ID)
                {
                    ModelState.AddModelError("ClassLink", "This class link is already in use. Please choose a different class link.");
                }
            }

            // If the client submitted the exact stored hash (we are pre-filling the inputs with it),
            // treat that as "no change" so we don't re-hash the hash and overwrite the stored value.
            bool passwordEqualsExistingHash =
                !string.IsNullOrEmpty(Password) &&
                Password == existing.PasswordHash &&
                !string.IsNullOrEmpty(ConfirmPassword) &&
                ConfirmPassword == existing.PasswordHash;

            bool passwordProvided = !(string.IsNullOrEmpty(Password) && string.IsNullOrEmpty(ConfirmPassword));

            if (passwordProvided && !passwordEqualsExistingHash)
            {
                if (Password != ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Passwords do not match.");
                }
                else
                {
                    existing.PasswordHash = PasswordHashing.CreateHash(Password);
                }
            }

            // Update other editable fields
            existing.FirstName = teacher.FirstName;
            existing.LastName = teacher.LastName;
            existing.Username = teacher.Username;
            existing.Grade = teacher.Grade;
            existing.ClassLink = teacher.ClassLink;

            if (!ModelState.IsValid)
            {
                return View(teacher);
            }

            await _teacherService.UpdateTeacherAsync(existing);

            // Use PRG to load the Account view through its action so ViewData["ClassLinkFull"] is populated
            return RedirectToAction("Account", new { id = existing.ID });
        }

        // Account/{teacherId}/DeleteAccount
        public async Task<IActionResult> DeleteAccount(int id)
        {
            Teacher teacher = await _teacherService.GetTeacherByIdAsync(id);

            if (teacher == null) return View("NotFound");
            return View(teacher);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Teacher teacher = await _teacherService.GetTeacherByIdAsync(id);
            if (teacher == null) return View("NotFound");

            await _teacherService.DeleteTeacherAsync(teacher);
            return RedirectToAction("LogOut", "LogIn");
        }
    }
}
