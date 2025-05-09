using System.Text.Json;
using Routeplanner_API.Database_Queries;

namespace Routeplanner_API.UoWs
{
    public class RouteUoW
    {
        private readonly RouteDbQueries _routeDbQueries;

        public RouteUoW(IConfiguration configuration)
        {
            _routeDbQueries = new RouteDbQueries(configuration);
        }

        public Route[]? GetRoutes()
        {
            return _routeDbQueries.GetRoutes(); // Get the Routes from the database.
        }

        public void AddRoute(JsonElement jsonBody)
        {
            Route route = Mappers.RouteMapper.MapJsonBodyToRouteObject(jsonBody); // Map the jsonBody to a Route object.

            bool routeIsValid = Helpers.RouteHelper.ValidateRoute(route); // Validate the Route.

            if (routeIsValid)
            {
                _routeDbQueries.AddRoute(route); // Add the Route to the database.
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
