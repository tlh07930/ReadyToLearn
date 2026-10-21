using Microsoft.EntityFrameworkCore;
using Ready2Learn.Models;

namespace Ready2Learn.Data
{
    public class Ready2LearnDbContext : DbContext
    {
        public Ready2LearnDbContext(DbContextOptions<Ready2LearnDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>().HasKey(g => g.ID);

            modelBuilder.Entity<Result>().HasKey(r => r.ID);
            modelBuilder.Entity<Result>().HasOne(s => s.Student);
            modelBuilder.Entity<Result>().HasOne(g => g.Game);

            modelBuilder.Entity<Student>().HasKey(s => s.ID);
            modelBuilder.Entity<Student>().HasOne(t => t.Teacher);
            modelBuilder.Entity<Student>().HasMany(r => r.Results);

            modelBuilder.Entity<Teacher>().HasKey(t => t.ID);
            modelBuilder.Entity<Teacher>().HasMany(s => s.Students);
            
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Teacher> Teachers { get; set; }

        public DbSet<Student> Students { get; set; }

        public DbSet<Game> Games { get; set; }

        public DbSet<Result> Results { get; set; }
    }
}
