using System;
using Npgsql;

namespace Routeplanner_API.Database_Queries
{
    public class LocationDbQueries
    {
        private static string connectionString = "Host=145.24.222.95;Port=8765;Username=dreamteam;Password=dreamteam;Database=postgres";

        public void GetLocations()
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT Name, Latitude, Longitude, Description FROM Locations";

                    using (var command = new NpgsqlCommand(sql, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string Name = reader.GetString(0);
                            float Latitude = reader.GetFloat(1);
                            float Longitude = reader.GetFloat(2);
                            string Description = reader.GetString(3);

                            Console.WriteLine($"Name: {Name}, Latitude: {Latitude}, Longitude: {Longitude}, Description: {Description}");
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public static void AddLocation(Location location)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    string insertQuery = "INSERT INTO Locations (Name, Latitude, Longitude, Description) VALUES (@Name, @Latitude, @Longitude, @Description)";

                    using var cmd = new NpgsqlCommand(insertQuery, connection);
                    //cmd.Parameters.AddWithValue("UserId", userId);
                    cmd.Parameters.AddWithValue("Name", location.name);
                    cmd.Parameters.AddWithValue("Latitude", location.latitude);
                    cmd.Parameters.AddWithValue("Longitude", location.longitude);
                    cmd.Parameters.AddWithValue("Description", location.description);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    Console.WriteLine($"Inserted {rowsAffected} row(s) into the database.");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void EditLocation(Location location)
        {

        }

        public void DeleteLocation(Location location)
        {

        }
    }
}
