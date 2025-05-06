using Routeplanner_API.Database_Queries;
using System.Text.Json;

namespace Routeplanner_API.UoWs
{
    public class UserUoW
    {
        private readonly UserDbQueries _userDbQueries;

        public UserUoW(UserDbQueries userDbQueries)
        {
            _userDbQueries = userDbQueries ?? throw new ArgumentNullException(nameof(userDbQueries));
        }

        public User[] GetUsers()
        {
            return _userDbQueries.GetUsers();
        }

        public void AddUser(JsonElement jsonBody)
        {
            var user = Mappers.UserMapper.MapJsonBodyToUserObject(jsonBody); // Map the JsonBody to an User object. 

            bool userIsValid = Helpers.UserHelper.isUserValid(user); // Validate the User.

            if (userIsValid)
            {
                _userDbQueries.AddUser(user);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
