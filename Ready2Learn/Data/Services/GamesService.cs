using Microsoft.EntityFrameworkCore;
using Ready2Learn.Models;

namespace Ready2Learn.Data.Services
{
    public class GamesService : IGamesService
    {
        private readonly Ready2LearnDbContext _context;

        public GamesService(Ready2LearnDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Game>> GetAllGames()
        {
            List<Game> games = await _context.Games.ToListAsync();
            return games;
        }

        public async Task<Game?> GetGameByNameAsync(string name)
        {
            Game? game = await _context.Games.FirstOrDefaultAsync(g => g.Name == name);
            return game;
        }

        public async Task<IEnumerable<Game>> GetGamesBySubjectAsync(string subject)
        {
            List<Game> subjectGames = await _context.Games.Where(g => g.Subject == subject).ToListAsync();
            return subjectGames;
        }
    }
}