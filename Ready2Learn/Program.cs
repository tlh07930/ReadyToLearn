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
            builder.Services.AddDbContext<Ready2LearnDbContext>(options => options.UseSqlServer(connectionString));

            // Services Configuration
            //builder.Services.AddScoped<IDisplayStateService, DisplayStateService>();
            builder.Services.AddScoped<IGamesService, GamesService>();
            builder.Services.AddScoped<IResultsService, ResultsService>();
            builder.Services.AddScoped<IStudentsService, StudentsService>();
            builder.Services.AddScoped<ITeachersService, TeachersService>(); // LOOK INTO SCOPED VERSUS OTHER OPTIONS (SINGLETON OR TRANSIENT)

            // Register the game content service (singleton since content is static)
            builder.Services.AddSingleton<IGameContentService, GameContentService>();

            //builder.Services.AddHttpContextAccessor();
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(name: "default", pattern: "{controller=LogIn}/{action=LogIn}/{id?}");
            //app.MapControllerRoute();
            
            app.Run();
        }
    }
}
