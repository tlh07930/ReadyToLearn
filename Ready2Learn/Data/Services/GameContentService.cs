using System.Text.Json;
using Ready2Learn.Models;

namespace Ready2Learn.Data.Services
{
    public class GameContentService : IGameContentService
    {
        private readonly Dictionary<string, List<GameQA>> _content = new(StringComparer.OrdinalIgnoreCase);
        private readonly ILogger<GameContentService> _logger;

        public GameContentService(IWebHostEnvironment env, ILogger<GameContentService> logger)
        {
            _logger = logger;
            try
            {
                var file = Path.Combine(env.ContentRootPath, "Data", "game-questions.json");
                if (File.Exists(file))
                {
                    var text = File.ReadAllText(file);
                    var doc = JsonSerializer.Deserialize<Dictionary<string, List<GameQA>>>(text, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    if (doc != null)
                    {
                        foreach (var kv in doc)
                        {
                            _content[kv.Key] = kv.Value ?? new List<GameQA>();
                        }
                    }
                }
                else
                {
                    _logger.LogWarning("Game questions file not found: {File}", file);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load game questions.");
            }
        }

        public IReadOnlyList<GameQA> GetQuestions(string gameName)
        {
            if (string.IsNullOrWhiteSpace(gameName)) return Array.Empty<GameQA>();
            return _content.TryGetValue(gameName, out var list) ? list : Array.Empty<GameQA>();
        }

        public GameQA? GetRandomQuestion(string gameName)
        {
            var list = GetQuestions(gameName);
            if (list.Count == 0) return null;
            var idx = Random.Shared.Next(0, list.Count);
            return list[idx];
        }
    }
}