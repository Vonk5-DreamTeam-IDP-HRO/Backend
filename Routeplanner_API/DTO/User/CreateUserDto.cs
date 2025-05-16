using System.ComponentModel.DataAnnotations;

namespace Routeplanner_API.DTO.User
{
    public sealed class CreateUserDto
    {
        [Required]
        public string Username { get; init; } = null!;

        [Required]
        public string Email { get; init; } = null!;

        [Required]
        public string PasswordHash { get; init; } = null!;

        [Required]
        public int UserRightId { get; init; }
    }
}
