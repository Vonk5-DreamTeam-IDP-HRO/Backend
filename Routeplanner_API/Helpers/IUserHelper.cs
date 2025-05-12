using Routeplanner_API.DTO;

namespace Routeplanner_API.Helpers
{
    public interface IUserHelper
    {
        public string GenerateJwtToken(UserDto user);
    }
}
