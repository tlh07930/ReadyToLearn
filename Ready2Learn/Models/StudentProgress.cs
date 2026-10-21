namespace Ready2Learn.Models
{
    public class StudentProgress
    {
        public StudentProgress() { }

        public StudentProgress(IEnumerable<Student> students, IEnumerable<Game> games, IEnumerable<Result> results)
        {
            Students = students;
            Games = games;
            Results = results;
        }

        public IEnumerable<Student> Students { get; set; }

        public IEnumerable<Game> Games { get; set; }

        public IEnumerable<Result> Results { get; set; }
    }
}