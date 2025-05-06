using System;
using Npgsql;
using Routeplanner_API.Extensions; // Added using statement for the extension method

namespace Routeplanner_API.Database_Queries
{
    public class UserDbQueries
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public UserDbQueries(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _connectionString = _configuration.GetValidatedConnectionString();
        }

        public User[] GetUsers()
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    string selectQuery = "SELECT username, email, password_hash FROM Users"; // Error: Cannot find Users table.

                    using (var command = new NpgsqlCommand(selectQuery, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string Username = reader.GetString(0);
                            string Email = reader.GetString(1);
                            string Password_Hash = reader.GetString(2);

                            Console.WriteLine($"Username: {Username}, Email: {Email}, Password_Hash: {Password_Hash}");
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void AddUser(User user)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    string insertQuery = "INSERT INTO Users (username, email, password_hash) VALUES (@Username, @Email, @PasswordHash)";

                    using var cmd = new NpgsqlCommand(insertQuery, connection);
                    //cmd.Parameters.AddWithValue("UserId", userId);
                    cmd.Parameters.AddWithValue("Username", user.UserName);
                    cmd.Parameters.AddWithValue("Email", user.Email);
                    cmd.Parameters.AddWithValue("PasswordHash", user.PasswordHash);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    Console.WriteLine($"Inserted {rowsAffected} row(s) into the database.");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
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
