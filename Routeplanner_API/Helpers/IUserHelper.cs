using Routeplanner_API.DTO.User;

namespace Routeplanner_API.Helpers
{
    public interface IUserHelper
    {
        string GenerateUserJwtToken(UserDto user);
    }
}
