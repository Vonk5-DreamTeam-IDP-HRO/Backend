using System;
using Npgsql;
using Routeplanner_API.Extensions;

namespace Routeplanner_API.Database_Queries
{
    public class LocationDbQueries
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly ILogger _logger;

        public LocationDbQueries(IConfiguration configuration, ILogger logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _connectionString = _configuration.GetValidatedConnectionString();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
                List<Location> locations = new List<Location>();

                while (reader.Read())
                {
                    string name = reader.GetString(0);
                    double latitude = reader.GetDouble(1);
                    double longitude = reader.GetDouble(2);
                    string description = reader.GetString(3);

                    Location location = new Location()
                    {
                        Name = name,
                        Description = description,
                        Latitude = latitude,
                        Longitude = longitude,
                    };
                    locations.Add(location);
                }
                return locations.ToArray();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.ToString());            
                return null;
            }
        }

        public void AddLocation(Location location)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);                
                connection.Open();
                string insertQuery = "INSERT INTO Locations (name, latitude, longitude, description) VALUES (@Name, @Latitude, @Longitude, @Description)";
                using var command = new NpgsqlCommand(insertQuery, connection);

                //command.Parameters.AddWithValue("UserId", new Guid());
                command.Parameters.AddWithValue("Name", location.Name);
                command.Parameters.AddWithValue("Latitude", location.Latitude);
                command.Parameters.AddWithValue("Longitude", location.Longitude);
                command.Parameters.AddWithValue("Description", location.Description);

                int rowsAffected = command.ExecuteNonQuery();
                _logger.LogInformation("Inserted {RowsAffected} row(s) into the database.", rowsAffected);


            }
            catch (Exception exception)
            {
                _logger.LogError(exception.ToString());
            }
        }

        public void AddLocationDetails(LocationDetails locationDetails)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);               
                connection.Open();
                string insertQuery = @"INSERT INTO location_details (address, city, country, zip_code, phone_number, website, category, accessibility)
                                        VALUES (@Address, @City, @Country, @ZipCode, @PhoneNumber, @Website, @Category, @Accessibility)";
                using var command = new NpgsqlCommand(insertQuery, connection);

                command.Parameters.AddWithValue("@Address", locationDetails.Address);
                command.Parameters.AddWithValue("@City", locationDetails.City);
                command.Parameters.AddWithValue("@Country", locationDetails.Country);
                command.Parameters.AddWithValue("@ZipCode", locationDetails.ZipCode);
                command.Parameters.AddWithValue("@PhoneNumber", locationDetails.PhoneNumber);
                command.Parameters.AddWithValue("@Website", locationDetails.Website);
                command.Parameters.AddWithValue("@Category", locationDetails.Category);
                command.Parameters.AddWithValue("@Accessibility", locationDetails.Accessibility);

                int rowsAffected = command.ExecuteNonQuery();
                _logger.LogInformation("Inserted {RowsAffected} row(s) into the database.", rowsAffected);


            }
            catch (Exception exception)
            {
                _logger.LogError(exception.ToString());
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
