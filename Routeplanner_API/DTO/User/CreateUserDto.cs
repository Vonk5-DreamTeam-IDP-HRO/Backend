using System.ComponentModel.DataAnnotations;

namespace Routeplanner_API.DTO.User
{
    /// <summary>
    /// DTO for creating a new user.
    /// </summary>
    public sealed class CreateUserDto
    {
        /// <summary>
        /// Gets or sets the username for the new user.
        /// </summary>
        [Required]
        public string Username { get; init; } = null!;

        /// <summary>
        /// Gets or sets the email address for the new user.
        /// </summary>
        [Required]
        public string Email { get; init; } = null!;

        /// <summary>
        /// Gets or sets the password for the new user.
        /// </summary>
        [Required]
        public string Password { get; init; } = null!;
    }
}
