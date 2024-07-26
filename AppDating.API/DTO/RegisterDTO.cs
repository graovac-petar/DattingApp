using System.ComponentModel.DataAnnotations;

namespace AppDating.API.DTO
{
    public class RegisterDTO
    {
        [Required]
        public required string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
