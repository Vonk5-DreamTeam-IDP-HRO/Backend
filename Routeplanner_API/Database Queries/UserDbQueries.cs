using System;
using Npgsql;

namespace Routeplanner_API.Database_Queries
{
    public class UserDbQueries
    {
        private string connectionString = "Host=145.24.222.95;Port=8765;Username=dreamteam;Password=dreamteam;Database=postgres";

        public void GetUsers()
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
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
                using (var connection = new NpgsqlConnection(connectionString))
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
            catch(Exception exception)
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
