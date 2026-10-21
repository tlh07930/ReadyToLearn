using System.ComponentModel.DataAnnotations;

namespace Ready2Learn.Models
{
    public class Result
    {
        public Result() {}

        public Result(int studentId, int gameId, string question, List<string> answers)
        {
            StudentID = studentId;
            GameID = gameId;
            Question = question;
            Answers = answers;
        }

        [Key]
        public int ID { get; }

        public int StudentID { get; set; }

        public int GameID { get; set; }

        public string Question { get; set; }

        public List<string> Answers { get; set; }

        // Relationships
        public Student? Student { get; set; }

        public Game? Game { get; set; }
    }
}
