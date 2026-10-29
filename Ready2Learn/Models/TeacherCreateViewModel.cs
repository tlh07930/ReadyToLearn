using System.ComponentModel.DataAnnotations;

namespace Ready2Learn.Models
{
    // ViewModel used only for CreateAccount to host password validation.
    public class TeacherCreateViewModel
    {
        [StringLength(50)]
        public string? FirstName { get; set; }

        [StringLength(50)]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Grade is required")]
        [RegularExpression(@"^(KG|PK|1)$", ErrorMessage = "Grade must be one of the following: PK, KG, or 1")]
        public required string Grade { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(100)]
        [RegularExpression(@"^[A-Za-z0-9._-]+$", ErrorMessage = "Username can only contain letters, numbers, periods, underscores, and hyphens.")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*\-])(?!.*\s).{6,}$",
            ErrorMessage = "Password must be at least 6 characters and include upper and lower case letters, a number, and a special character.")]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string? ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Class Link is required")]
        [StringLength(100)]
        public required string ClassLink { get; set; }
    }
}