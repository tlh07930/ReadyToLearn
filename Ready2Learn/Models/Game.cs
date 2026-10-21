using System.ComponentModel.DataAnnotations;

namespace Ready2Learn.Models
{
    public class Game
    {
        [Key]
        public int ID { get; }

        [Required]
        public string? Subject { get; set; }

        [Required]
        public string? Name { get; set; }
    }
}
