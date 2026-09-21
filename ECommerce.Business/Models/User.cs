using System.ComponentModel.DataAnnotations;

namespace ECommerce.Business.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public required string FullName { get; set; }

        public required string Email { get; set; }

        public required string PasswordHash { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        public string Role { get; set; } = "Customer";
    }
}