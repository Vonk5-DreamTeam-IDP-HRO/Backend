using System;
using Npgsql;
using Routeplanner_API.Extensions;

namespace Routeplanner_API.Database_Queries
{
    public class LocationDbQueries
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public LocationDbQueries(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _connectionString = _configuration.GetValidatedConnectionString();
        }

        public Location[]? GetLocations()
        {
          try
            {
                using var connection = new NpgsqlConnection(_connectionString);
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
                        List<Location> locations = new List<Location>();

                        while (reader.Read())
                        {
                            string Name = reader.GetString(0);
                            double Latitude = reader.GetDouble(1);
                            double Longitude = reader.GetDouble(2);
                            string Description = reader.GetString(3);

                            Location location = new Location()
                            {
                                Name = Name,
                                Description = Description,
                                Latitude = Latitude,
                                Longitude = Longitude,
                            };
                            locations.Add(location);
                        }
                        return locations.ToArray();
                    }
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
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    string insertQuery = "INSERT INTO Locations (name, latitude, longitude, description) VALUES (@Name, @Latitude, @Longitude, @Description)";

                    using var cmd = new NpgsqlCommand(insertQuery, connection);
                    //cmd.Parameters.AddWithValue("UserId", userId);
                    cmd.Parameters.AddWithValue("Name", location.Name);
                    cmd.Parameters.AddWithValue("Latitude", location.Latitude);
                    cmd.Parameters.AddWithValue("Longitude", location.Longitude);
                    cmd.Parameters.AddWithValue("Description", location.Description);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    Console.WriteLine($"Inserted {rowsAffected} row(s) into the database.");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Generic error in AddLocation: {exception}");
            }
        }

        public static void AddLocationDetails(LocationDetails locationDetails)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    string insertQuery = @"INSERT INTO location_details (address, city, country, zip_code, phone_number, website, category, accessibility)
                                           VALUES (@Address, @City, @Country, @ZipCode, @PhoneNumber, @Website, @Category, @Accessibility)";

                    using var cmd = new NpgsqlCommand(insertQuery, connection);
                    cmd.Parameters.AddWithValue("@Address", locationDetails.Address);
                    cmd.Parameters.AddWithValue("@City", locationDetails.City);
                    cmd.Parameters.AddWithValue("@Country", locationDetails.Country);
                    cmd.Parameters.AddWithValue("@ZipCode", locationDetails.ZipCode);
                    cmd.Parameters.AddWithValue("@PhoneNumber", locationDetails.PhoneNumber);
                    cmd.Parameters.AddWithValue("@Website", locationDetails.Website);
                    cmd.Parameters.AddWithValue("@Category", locationDetails.Category);
                    cmd.Parameters.AddWithValue("@Accessibility", locationDetails.Accessibility);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    Console.WriteLine($"Inserted {rowsAffected} row(s) into the database.");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("An error occurred while inserting data:");
                Console.WriteLine(exception.Message);
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
