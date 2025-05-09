using System;
using Npgsql;
using Routeplanner_API.Extensions; // Added using statement for the extension method

namespace Routeplanner_API.Database_Queries
{
    public class UserDbQueries
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly ILogger _logger;

        public UserDbQueries(IConfiguration configuration, ILogger logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _connectionString = _configuration.GetValidatedConnectionString();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public User[]? GetUsers()
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);              
                connection.Open();
                string selectQuery = "SELECT username, email, password_hash FROM Users"; // Error: Cannot find Users table.
                using var command = new NpgsqlCommand(selectQuery, connection);
                using var reader = command.ExecuteReader();
                List<User> users = new List<User>();

                while (reader.Read())
                {
                    string username = reader.GetString(0);
                    string email = reader.GetString(1);
                    string password_Hash = reader.GetString(2);

                    User user = new User()
                    {
                        UserName = username,
                        Email = email,
                        PasswordHash = password_Hash
                    };
                    users.Add(user);
                }
                return users.ToArray();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.ToString());
                return null;
            }
        }

        public void AddUser(User user)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);             
                connection.Open();
                string insertQuery = "INSERT INTO Users (username, email, password_hash) VALUES (@Username, @Email, @PasswordHash)";
                using var command = new NpgsqlCommand(insertQuery, connection);

                //cmd.Parameters.AddWithValue("UserId", userId);
                command.Parameters.AddWithValue("Username", user.UserName);
                command.Parameters.AddWithValue("Email", user.Email);
                command.Parameters.AddWithValue("PasswordHash", user.PasswordHash);

                int rowsAffected = command.ExecuteNonQuery();
                _logger.LogInformation("Inserted {RowsAffected} row(s) into the database.", rowsAffected);                
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.ToString());
            }
        }

        public void EditUser(User user)
        {

        }

        public void DeleteUser(User user)
        {

        }
    }
}
