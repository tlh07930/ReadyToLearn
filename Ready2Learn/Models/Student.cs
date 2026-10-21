using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ready2Learn.Models
{
    public class Student
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public required int TeacherID { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [StringLength(50)]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(50)]
        public required string LastName { get; set; }

        // Persisted secure hash of the sequence (e.g. "bike|cat|dog|sun") using PBKDF2
        public string? PasswordHash { get; set; }

        // Relationships
        public Teacher? Teacher { get; set; }

        public List<Result>? Results { get; set; }
    }
}