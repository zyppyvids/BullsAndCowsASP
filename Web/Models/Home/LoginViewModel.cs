using System.ComponentModel.DataAnnotations;

namespace Web.Models.Home
{
    public class LoginViewModel
    {
        [Required]
        [MaxLength(80, ErrorMessage = "Username cannot be longer than 80 characters")]
        public string Username { get; set; }

        [Required]
        [StringLength(18, ErrorMessage = "The password must be at least 6 characters", MinimumLength = 6)]
        [RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$", ErrorMessage = "Password does not match the desired requirements!")]
        [DataType(DataType.Password)]
        public string PasswordHash { get; set; }
    }
}
