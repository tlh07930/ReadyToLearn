using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Ready2Learn.Data;
using Ready2Learn.Data.Services;

namespace Ready2Learn
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // DbContext Configuration
            builder.Services.AddDbContext<Ready2LearnDbContext>(options =>
            {
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 2,
                        maxRetryDelay: TimeSpan.FromSeconds(2),
                        errorNumbersToAdd: null);

                    sqlOptions.CommandTimeout(10);
                });
            });

            // Services Configuration
            builder.Services.AddScoped<IGamesService, GamesService>();
            builder.Services.AddScoped<IResultsService, ResultsService>();
            builder.Services.AddScoped<IStudentsService, StudentsService>();
            builder.Services.AddScoped<ITeachersService, TeachersService>();

            // Register the game content service (singleton since content is static)
            builder.Services.AddSingleton<IGameContentService, GameContentService>();

            builder.Services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(name: "default", pattern: "{controller=LogIn}/{action=LogIn}/{id?}");

            app.Run();
        }
    }
}