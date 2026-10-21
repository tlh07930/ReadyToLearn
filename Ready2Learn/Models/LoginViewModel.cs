using System.ComponentModel.DataAnnotations;

namespace Ready2Learn.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(100)]
        public required string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}