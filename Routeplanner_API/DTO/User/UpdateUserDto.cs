namespace Routeplanner_API.DTO.User
{
    /// <summary>
    /// DTO for updating an existing user.
    /// </summary>
    public sealed class UpdateUserDto
    {
        /// <summary>
        /// Gets the unique identifier of the user to update.
        /// </summary>
        public Guid UserId { get; init; }

        /// <summary>
        /// Gets or sets the updated username of the user. Optional.
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// Gets or sets the updated email address of the user. Optional.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the updated password hash of the user. Optional.
        /// </summary>
        public string? PasswordHash { get; set; }
    }
}
