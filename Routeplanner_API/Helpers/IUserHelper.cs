using Routeplanner_API.DTO;

namespace Routeplanner_API.Helpers
{
    public interface IUserHelper
    {
        string GenerateUserJwtToken(UserDto user);

        string GenerateAdminJwtToken(UserDto admin);
    }
}
