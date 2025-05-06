using System;
using Npgsql;
using Routeplanner_API.Extensions;

namespace Routeplanner_API.Database_Queries
{
    public class RouteDbQueries
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly ILogger _logger;

        public RouteDbQueries(IConfiguration configuration, ILogger logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _connectionString = _configuration.GetValidatedConnectionString();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        }

        public Route[]? GetRoutes()
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    string selectQuery = "SELECT name, description, Locations FROM Routes";

                    using (var command = new NpgsqlCommand(selectQuery, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        List<Route> routes = new List<Route>();

                        while (reader.Read())
                        {
                            string name = reader.GetString(0);
                            string description = reader.GetString(1);

                            //var locations = reader.IsDBNull(2) ? null : reader.GetFieldValue<string[]>(2);

                            Route route = new Route
                            {
                                Name = name,
                                Description = description,
                                //locations = locations
                            };
                            routes.Add(route);
                        }

                        return routes.ToArray();
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.ToString());
                return null;
            }
        }


        public void AddRoute(Route route)
        {

        }

        public void EditRoute(Route route)
        {

        }

        public void DeleteRoute(Route route)
        {

        }
    }
}
