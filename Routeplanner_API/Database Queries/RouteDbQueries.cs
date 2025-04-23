using System;
using Npgsql;

namespace Routeplanner_API.Database_Queries
{
    public class RouteDbQueries
    {
        private string connectionString = "Host=145.24.222.95;Port=8765;Username=dreamteam;Password=dreamteam;Database=postgres";

        public void GetRoutes()
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    string selectQuery = "SELECT Name, Description, Locations FROM Routes";

                    using (var command = new NpgsqlCommand(selectQuery, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string Name = reader.GetString(0);
                            string Description = reader.GetString(1);
                            //Location[] Locations = reader.GetObject(2);

                            Console.WriteLine($"Name: {Name}, Description: {Description}, Locations: ");
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
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
