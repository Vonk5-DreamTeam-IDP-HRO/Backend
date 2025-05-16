using System.ComponentModel.DataAnnotations;

namespace Routeplanner_API.DTO.User
{
    public sealed class UpdateUserDto
    {
        public Guid UserId { get; init; } 

        public string? Username { get; set; } 

        public string? Email { get; set; } 

        public string? PasswordHash { get; set; }
    }
}
