using System;
using Npgsql;

namespace Routeplanner_API.Database_Queries
{
    public class LocationDbQueries
    {
        private static string connectionString = "Host=145.24.222.95;Port=8765;Username=dreamteam;Password=dreamteam;Database=postgres";

        public static Location[]? GetLocations()
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    string selectQuery = "SELECT Name, Latitude, Longitude, Description FROM Locations";

                    using (var command = new NpgsqlCommand(selectQuery, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        List<Location> locations = new List<Location>();


                        while (reader.Read())
                        {
                            string Name = reader.GetString(0);
                            float Latitude = reader.GetDouble(1);
                            float Longitude = reader.GetDouble(2);
                            string Description = reader.GetString(3);

                            Location location = new Location()
                            {
                                name = Name,
                                description = Description
                                latitude = Latitude,
                                longitude = Longitude,
                            };
                            locations.Add(location);
                        }
                        return locations.ToArray();
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
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
