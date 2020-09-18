using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Web.Models.Home
{
    public class RegisterViewModel
    {
        [HiddenInput]
        public int Id { get; set; }

        [Required]
        [MaxLength(80, ErrorMessage = "Username cannot be longer than 80 characters")]
        public string Username { get; set; }

        [Required]
        [StringLength(18, ErrorMessage = "The password must be at least 6 characters", MinimumLength = 6)]
        [RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
