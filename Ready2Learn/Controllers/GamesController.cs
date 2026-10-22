using Microsoft.AspNetCore.Mvc;
using Ready2Learn.Data.Services;
using Ready2Learn.Models;

namespace Ready2Learn.Controllers
{
    public class GamesController : Controller
    {
        private readonly IGamesService _gamesService;
        private readonly IResultsService _resultsService;
        private readonly IStudentsService _studentsService;
        private readonly ITeachersService _teachersService;

        public GamesController(
            IGamesService gamesService,
            IResultsService resultsService,
            IStudentsService studentsService,
            ITeachersService teachersService)
        {
            _gamesService = gamesService;
            _resultsService = resultsService;
            _studentsService = studentsService;
            _teachersService = teachersService;
        }

        public async Task<IActionResult> DisplayGamesBySubject(string subject)
        {
            var gameList = await _gamesService.GetGamesBySubjectAsync(subject);
            return View(gameList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitResult(int student, string game, string question, string answer, string guessesString)
        {
            // If student == -1 (teacher or no student), do not submit result to database
            if (student == -1)
            {
                return RedirectToAction(game.Replace(" ",""));
            }

            int studentId = student;
            var gameObj = await _gamesService.GetGameByNameAsync(game);
            if (gameObj == null)
            {
                return NotFound();
            }
            int gameId = gameObj.ID;

            List<string> guesses = string.IsNullOrWhiteSpace(guessesString) ? new List<string>() : guessesString.Split(',').ToList();

            guesses.Add(answer);

            Result result = new(studentId, gameId, question, guesses);
            await _resultsService.SubmitResultAsync(result);

            // Return to the game so a new question is shown
            return RedirectToAction("RhymeTime");
        }

        private async Task<string?> GetCurrentGradeAsync()
        {
            if (Request.Cookies.TryGetValue("StudentID", out var studentCookie) &&
                int.TryParse(studentCookie, out var studentId))
            {
                var student = await _studentsService.GetStudentByIdAsync(studentId);

                if (student == null)
                {
                    return null;
                }

                var teacher = await _teachersService.GetTeacherByIdAsync(
                    student.TeacherID);

                return teacher?.Grade;
            }

            if (Request.Cookies.TryGetValue("TeacherID", out var teacherCookie) &&
                int.TryParse(teacherCookie, out var teacherId))
            {
                var teacher = await _teachersService.GetTeacherByIdAsync(
                    teacherId);

                return teacher?.Grade;
            }

            return null;
        }

        private async Task<IActionResult> GetGameViewAsync(string gameName)
        {
            var game = await _gamesService.GetGameByNameAsync(gameName);

            if (game == null)
            {
                return NotFound();
            }

            ViewData["Grade"] = await GetCurrentGradeAsync();

            return View(game);
        }

        /* ELA Games */
        public async Task<IActionResult> Alphabet()
        {
            return await GetGameViewAsync("Alphabet");
        }

        public async Task<IActionResult> LetterSounds()
        {
            return await GetGameViewAsync("Letter Sounds");
        }

        public async Task<IActionResult> PopcornWords()
        {
            return await GetGameViewAsync("Popcorn Words");
        }

        public async Task<IActionResult> RhymeTime()
        {
            return await GetGameViewAsync("Rhyme Time");
        }

        /* Math Games */
        public async Task<IActionResult> BeforeandAfter()
        {
            return await GetGameViewAsync("Before and After");
        }

        public async Task<IActionResult> CountBy10s()
        {
            return await GetGameViewAsync("Count By 10s");
        }

        public async Task<IActionResult> DaysoftheWeek()
        {
            return await GetGameViewAsync("Days of the Week");
        }

        public async Task<IActionResult> FastFacts()
        {
            return await GetGameViewAsync("Fast Facts");
        }

        public async Task<IActionResult> Money()
        {
            return await GetGameViewAsync("Money");
        }

        public async Task<IActionResult> MoreorLess()
        {
            return await GetGameViewAsync("More or Less");
        }

        public async Task<IActionResult> Time()
        {
            return await GetGameViewAsync("Time");
        }

        /* Science Games */
        public async Task<IActionResult> BigandSmall()
        {
            return await GetGameViewAsync("Big and Small");
        }

        public async Task<IActionResult> Colors()
        {
            return await GetGameViewAsync("Colors");
        }

        public async Task<IActionResult> FixIt()
        {
            return await GetGameViewAsync("Fix It");
        }

        public async Task<IActionResult> Seasons()
        {
            return await GetGameViewAsync("Seasons");
        }

        public async Task<IActionResult> Weather()
        {
            return await GetGameViewAsync("Weather");
        }

        /* Social Studies Games */
        public async Task<IActionResult> Maps()
        {
            return await GetGameViewAsync("Maps");
        }
    }
}
