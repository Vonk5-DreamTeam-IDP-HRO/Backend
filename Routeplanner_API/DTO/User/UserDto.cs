namespace Routeplanner_API.DTO.User
{
    public sealed class UserDto
    {
        public Guid UserId { get; init; }
        public string Username { get; init; } = null!;
        public string Email { get; init; } = null!;
        public string Password { get; init; } = null!;
    }
}
