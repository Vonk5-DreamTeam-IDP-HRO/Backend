using Routeplanner_API.DTO;

namespace Routeplanner_API.Helpers
{
    public interface IUserHelper
    {
        string GenerateJwtToken(UserDto user);
    }
}
