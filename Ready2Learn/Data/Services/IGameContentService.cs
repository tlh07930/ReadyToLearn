using Ready2Learn.Models;

namespace Ready2Learn.Data.Services
{
    public interface IGameContentService
    {
        /// <summary>
        /// Return all QA pairs for a given game name. Returns empty list when not found.
        /// </summary>
        IReadOnlyList<GameQA> GetQuestions(string gameName, string grade);

        /// <summary>
        /// Return a random QA pair for the game, or null if none.
        /// </summary>
        GameQA? GetRandomQuestion(string gameName, string grade);
    }
}