using System;
using Npgsql;

namespace Routeplanner_API.Database_Queries
{
    public class UserDbQueries
    {
        private string connectionString = "Host=145.24.222.95;Port=8765;Username=dreamteam;Password=dreamteam;Database=postgres";

        public void ReadUsers()
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT Username, Email, Password_Hash FROM Users"; // Error: Cannot find Users table.

                    using (var command = new NpgsqlCommand(sql, connection))
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
    }
}
