using System.Text.Json;
using Ready2Learn.Models;

namespace Ready2Learn.Data.Services
{
    public class GameContentService : IGameContentService
    {
        private readonly Dictionary<
            string,
            Dictionary<string, List<GameQA>>> _content =
            new(StringComparer.OrdinalIgnoreCase);

        private readonly ILogger<GameContentService> _logger;

        public GameContentService(
            IWebHostEnvironment env,
            ILogger<GameContentService> logger)
        {
            _logger = logger;

            try
            {
                var file = Path.Combine(
                    env.ContentRootPath,
                    "Data",
                    "game-questions.json");

                if (!File.Exists(file))
                {
                    _logger.LogWarning(
                        "Game questions file not found: {File}",
                        file);

                    return;
                }

                var text = File.ReadAllText(file);

                var content = JsonSerializer.Deserialize<
                    Dictionary<string, Dictionary<string, List<GameQA>>>>(
                    text,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (content == null)
                {
                    return;
                }

                foreach (var game in content)
                {
                    _content[game.Key] = new Dictionary<
                        string,
                        List<GameQA>>(
                        game.Value ?? new Dictionary<
                            string,
                            List<GameQA>>(),
                        StringComparer.OrdinalIgnoreCase);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to load game questions.");
            }
        }

        public IReadOnlyList<GameQA> GetQuestions(
            string gameName,
            string grade)
        {
            if (string.IsNullOrWhiteSpace(gameName) ||
                string.IsNullOrWhiteSpace(grade))
            {
                return Array.Empty<GameQA>();
            }

            if (_content.TryGetValue(gameName, out var grades) &&
                grades.TryGetValue(grade, out var questions))
            {
                return questions;
            }

            return Array.Empty<GameQA>();
        }

        public GameQA? GetRandomQuestion(
            string gameName,
            string grade)
        {
            var questions = GetQuestions(gameName, grade);

            if (questions.Count == 0)
            {
                return null;
            }

            return questions[Random.Shared.Next(questions.Count)];
        }
    }
}