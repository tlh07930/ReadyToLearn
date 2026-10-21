using System.ComponentModel.DataAnnotations;

namespace Ready2Learn.Models
{
    public class Teacher
    {
        [Key]
        public int ID { get; set; }

        [StringLength(50)]
        public string? FirstName { get; set; }

        [StringLength(50)]
        public string? LastName { get; set; }
        
        [Required(ErrorMessage = "Grade is required")]
        [RegularExpression(@"^(KG|PK|1)$", ErrorMessage = "Grade must be one of the following: PK, KG, or 1")]
        public required string Grade { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(100)]
        public required string Username { get; set; }
        
        // Persisted hashed password (mapped)
        public string? PasswordHash { get; set; }

        [Required(ErrorMessage = "Class Link is required")]
        [StringLength(100)]
        public required string ClassLink { get; set; }

        // Relationships
        public List<Student>? Students { get; set; }
    }
}