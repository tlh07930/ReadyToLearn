using Microsoft.AspNetCore.Mvc;
using Ready2Learn.Data.Services;
using Ready2Learn.Models;

namespace Ready2Learn.Controllers
{
    public class StudentProgressController : Controller
    {
        private readonly IGamesService _gamesService;
        private readonly IResultsService _resultsService;
        private readonly IStudentsService _studentService;

        public StudentProgressController(IGamesService gamesService, IResultsService resultsService, IStudentsService studentService)
        {
            _gamesService = gamesService;
            _resultsService = resultsService;
            _studentService = studentService;
        }

        // Account/{teacherId}/StudentProgress
        public async Task<IActionResult> StudentProgress(int id)
        {
            IEnumerable<Student> students = await _studentService.GetAllStudentsByTeacherIdAsync(id);
            IEnumerable<Game> games = await _gamesService.GetAllGames();
            IEnumerable<Result> results = await _resultsService.GetResultsByTeacherId(id);

            // Save teacher id for client-side requests
            ViewData["TeacherId"] = id;

            StudentProgress studentProgress = new StudentProgress(students, games, results);

            if (studentProgress == null) return View("NotFound");
            return View("StudentProgress", studentProgress);
        }

        // GET: /StudentProgress/GetFilteredResults?teacherId=2&studentId=1&subject=ELA&gameId=3
        [HttpGet]
        public async Task<IActionResult> GetFilteredResults(int teacherId, int? studentId, string? subject, int? gameId)
        {
            List<Result> results = await _resultsService.GetResultsByTeacherIdFiltered(teacherId, studentId, subject, gameId);

            var safe = results.Select(r => new
            {
                id = r.ID,
                studentId = r.StudentID,
                studentName = r.Student != null ? (r.Student.FirstName + " " + r.Student.LastName) : "",
                gameId = r.GameID,
                game = r.Game != null ? r.Game.Name : "",
                subject = r.Game != null ? r.Game.Subject : "",
                question = r.Question,
                answers = r.Answers,
                isPassed = r.Answers.Count == 1
            });

            return Json(safe);
        }
    }
}
