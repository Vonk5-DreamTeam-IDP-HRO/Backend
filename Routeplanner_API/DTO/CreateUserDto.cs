using System.ComponentModel.DataAnnotations;

namespace Routeplanner_API.DTO
{
    public class CreateUserDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public string Username { get; set; } = null!;

        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;
    }
}
