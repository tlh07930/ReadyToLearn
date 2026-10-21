using Ready2Learn.Models;
using Ready2Learn.Security;

namespace Ready2Learn.Data
{
    public class DbTest
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<Ready2LearnDbContext>();

                context.Database.EnsureCreated();

                // Teacher
                if (!context.Teachers.Any())
                {
                    var t = new Teacher()
                    {
                        FirstName = "Test",
                        LastName = "Teacher",
                        Grade = "KG",
                        Username = "teacher",
                        ClassLink = "testKgLink"
                    };
                    t.PasswordHash = PasswordHashing.CreateHash("Kg123!");
                    context.Teachers.Add(t);
                    context.SaveChanges();
                }

                // Student
                if (!context.Students.Any())
                {
                    var s = new Student()
                    {
                        TeacherID = 2,
                        FirstName = "Student",
                        LastName = "One"
                    };
                    // set initial 4-image password as hash
                    s.PasswordHash = PasswordHashing.CreateHash(string.Join("|", new List<string> { "cat", "dog", "bike", "slide" }));
                    context.Students.Add(s);
                    context.SaveChanges();
                }

                // Games
                if (!context.Games.Any())
                {
                    context.Games.AddRange(new List<Game>()
                    {
                        new()
                        {
                            Subject = "ELA",
                            Name = "Popcorn Words"
                        },

                        new()
                        {
                            Subject = "Math",
                            Name = "Fast Facts"
                        },

                        new()
                        {
                            Subject = "Science",
                            Name = "Weather"
                        },

                        new()
                        {
                            Subject = "Social Studies",
                            Name = "Maps"
                        },
                    });
                    context.SaveChanges();
                }

                // Results
                if (!context.Results.Any())
                {
                    context.Results.Add(new Result()
                    {
                        StudentID = 1,
                        GameID = 1,
                        Question = "This is a test question?",
                        Answers = new List<string> { "answer1", "answer2", "answer3" }
                    });
                    context.SaveChanges();
                }

            }
        }
    }
}
