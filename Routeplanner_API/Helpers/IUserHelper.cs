using Routeplanner_API.DTO;
using Routeplanner_API.JWT;

namespace Routeplanner_API.Helpers
{
    public interface IUserHelper
    {
        string GenerateUserJwtToken(UserDto user);

        string GenerateAdminJwtToken(UserDto admin);
    }
}
