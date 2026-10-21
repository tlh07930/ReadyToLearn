using Microsoft.AspNetCore.Mvc;
using Ready2Learn.Data.Services;
using Ready2Learn.Models;

namespace Ready2Learn.Controllers
{
    public class GamesController : Controller
    {
        private readonly IGamesService _gamesService;
        private readonly IResultsService _resultsService;

        public GamesController(IGamesService gamesService, IResultsService resultsService)
        {
            _gamesService = gamesService;
            _resultsService = resultsService;
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

        /* ELA Games */
        public async Task<IActionResult> Alphabet()
        {
            var game = await _gamesService.GetGameByNameAsync("Alphabet");
            return View(game);
        }

        public async Task<IActionResult> LetterSounds()
        {
            var game = await _gamesService.GetGameByNameAsync("Letter Sounds");
            return View(game);
        }

        public async Task<IActionResult> PopcornWords()
        {
            var game = await _gamesService.GetGameByNameAsync("Popcorn Words");
            return View(game);
        }

        public async Task<IActionResult> RhymeTime()
        {
            var game = await _gamesService.GetGameByNameAsync("Rhyme Time");
            return View(game);
        }

        /* Math Games */
        public async Task<IActionResult> BeforeandAfter()
        {
            var game = await _gamesService.GetGameByNameAsync("Before and After");
            return View(game);
        }

        public async Task<IActionResult> CountBy10s()
        {
            var game = await _gamesService.GetGameByNameAsync("Count By 10s");
            return View(game);
        }

        public async Task<IActionResult> DaysoftheWeek()
        {
            var game = await _gamesService.GetGameByNameAsync("Days of the Week");
            return View(game);
        }

        public async Task<IActionResult> FastFacts()
        {
            var game = await _gamesService.GetGameByNameAsync("Fast Facts");
            return View(game);
        }

        public async Task<IActionResult> Money()
        {
            var game = await _gamesService.GetGameByNameAsync("Money");
            return View(game);
        }

        public async Task<IActionResult> MoreorLess()
        {
            var game = await _gamesService.GetGameByNameAsync("More or Less");
            return View(game);
        }

        public async Task<IActionResult> Time()
        {
            var game = await _gamesService.GetGameByNameAsync("Time");
            return View(game);
        }

        /* Science Games */
        public async Task<IActionResult> BigandSmall()
        {
            var game = await _gamesService.GetGameByNameAsync("Big and Small");
            return View(game);
        }

        public async Task<IActionResult> Colors()
        {
            var game = await _gamesService.GetGameByNameAsync("Colors");
            return View(game);
        }

        public async Task<IActionResult> FixIt()
        {
            var game = await _gamesService.GetGameByNameAsync("Fix It");
            return View(game);
        }

        public async Task<IActionResult> Seasons()
        {
            var game = await _gamesService.GetGameByNameAsync("Seasons");
            return View(game);
        }

        public async Task<IActionResult> Weather()
        {
            var game = await _gamesService.GetGameByNameAsync("Weather");
            return View(game);
        }

        /* Social Studies Games */
        public async Task<IActionResult> Maps()
        {
            var game = await _gamesService.GetGameByNameAsync("Maps");
            return View(game);
        }
    }
}
