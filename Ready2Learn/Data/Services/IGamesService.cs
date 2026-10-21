using Ready2Learn.Models;

namespace Ready2Learn.Data.Services
{
    public interface IGamesService
    {
        Task<IEnumerable<Game>> GetAllGames();
        Task<Game?> GetGameByNameAsync(string name);
        Task<IEnumerable<Game>> GetGamesBySubjectAsync(string subject);
    }
}