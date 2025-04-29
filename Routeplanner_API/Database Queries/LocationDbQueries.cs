using System;
using Npgsql;

namespace Routeplanner_API.Database_Queries
{
    public class LocationDbQueries
    {
        private readonly IConfiguration _configuration;

        public LocationDbQueries(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public Location[]? GetLocations()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Error: Database Connection string 'DefaultConnection' not found.");
            }

            try
            {
                using var connection = new NpgsqlConnection(connectionString);
                connection.Open();
                const string selectQuery = "SELECT Name, Latitude, Longitude, Description FROM Locations";
                using var command = new NpgsqlCommand(selectQuery, connection);
                using var reader = command.ExecuteReader();
                var locations = new List<Location>();
                while (reader.Read())
                {
                    var name = reader.GetString(0);
                    var latitude = reader.GetFloat(1);
                    var longitude = reader.GetFloat(2);
                    var description = reader.GetString(3);
                    var location = new Location
                    {
                        name = name,
                        latitude = latitude,
                        longitude = longitude,
                        description = description,
                    };
                    locations.Add(location);
                }
                return locations.ToArray();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                // TODO: Implement better and more specific logging for example using ILogger
                return null;
            }
        }

        public void AddLocation(Location location)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                Console.WriteLine("Error: Database connection string 'DefaultConnection' not found in configuration.");
                // Consider throwing an exception
                return;
            }

            try
            {
                using var connection = new NpgsqlConnection(connectionString);
                connection.Open();
                const string insertQuery = "INSERT INTO Locations (Name, Latitude, Longitude, Description) VALUES (@Name, @Latitude, @Longitude, @Description)";
                using var cmd = new NpgsqlCommand(insertQuery, connection);
                cmd.Parameters.AddWithValue("Name", location.name);
                cmd.Parameters.AddWithValue("Latitude", location.latitude);
                cmd.Parameters.AddWithValue("Longitude", location.longitude);
                cmd.Parameters.AddWithValue("Description", location.description);
                var rowsAffected = cmd.ExecuteNonQuery();
                Console.WriteLine($"Inserted {rowsAffected} row(s) into the database."); // Use ILogger ideally
            }
            catch (NpgsqlException pgEx)
            {
                Console.WriteLine($"Database error in AddLocation: {pgEx.Message}");
                // Consider re-throwing or handling differently
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Generic error in AddLocation: {exception}");
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
