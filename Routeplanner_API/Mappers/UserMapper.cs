using System.Text.Json;

namespace Routeplanner_API.Mappers
{
    public class UserMapper
    {
        public static User MapJsonBodyToUserObject(JsonElement jsonBody)
        {
            return new User()
            {
                UserName = jsonBody.GetProperty("username").GetString(),
                Email = jsonBody.GetProperty("email").GetString(),
                PasswordHash = jsonBody.GetProperty("passwordhash").GetString()
            };
        }
    }
}
