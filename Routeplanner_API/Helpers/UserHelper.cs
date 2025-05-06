namespace Routeplanner_API.Helpers
{
    public class UserHelper
    {
        private static List<string> errors = new List<string>();

        public static bool isUserValid(User user)
        {
            ValidateUser(user);
            return !errors.Any();
        }

        public static void ValidateUser(User user) 
        {
            if (user == null)
            {
                errors.Add("User cannot be null.");
                return;
            }

            if (string.IsNullOrWhiteSpace(user.UserName))
            {
                errors.Add("Username is required.");
            }

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                errors.Add("Email is required.");
            }

            if (string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                errors.Add("PasswordHash is required.");
            }
        }
    }
}
