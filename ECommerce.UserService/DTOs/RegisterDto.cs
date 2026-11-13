using System.ComponentModel.DataAnnotations;

namespace ECommerce.UserService.DTOs
{
    public class RegisterDto
    {
        [Required]
        public required string FullName { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [MinLength(6)]
        public required string Password { get; set; }

        public string Role { get; set; } = "Customer";
    }
}