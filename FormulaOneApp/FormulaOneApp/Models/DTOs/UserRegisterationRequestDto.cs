using System.ComponentModel.DataAnnotations;

namespace FormulaOneApp.Models.DTOs
{
    public class UserRegisterationRequestDto //To accept registeration request from user
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
