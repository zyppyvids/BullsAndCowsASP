using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Entity
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [MaxLength(80, ErrorMessage = "Username cannot be longer than 80 characters")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(18, ErrorMessage = "The password must be at least 6 characters", MinimumLength = 6)]
        [RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$")]
        [DataType(DataType.Password)]
        public string PasswordHash { get; set; }

        public int GamesWon { get; set; }
    }
}
