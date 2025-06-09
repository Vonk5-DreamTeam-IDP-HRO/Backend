namespace Routeplanner_API.DTO.User
{
    /// <summary>
    /// Data Transfer Object representing a user.
    /// </summary>
    public sealed class UserDto
    {
        /// <summary>
        /// Gets the unique identifier of the user.
        /// </summary>
        public Guid UserId { get; init; }

        /// <summary>
        /// Gets the username of the user.
        /// </summary>
        public string Username { get; init; } = null!;

        /// <summary>
        /// Gets the email address of the user.
        /// </summary>
        public string Email { get; init; } = null!;

        /// <summary>
        /// Gets the password of the user.
        /// </summary>
        public string Password { get; init; } = null!;
    }
}
