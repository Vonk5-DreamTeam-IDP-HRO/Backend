using System.Text.Json;

namespace Routeplanner_API.UoWs
{
    public class RouteUoW
    {
        public static Route[]? GetRoutes()
        {
            return Database_Queries.RouteDbQueries.GetRoutes(); // Get the Routes from the database.
        }

        public static void AddRoute(JsonElement jsonBody)
        {
            Route route = Mappers.RouteMapper.MapJsonBodyToRouteObject(jsonBody); // Map the jsonBody to a Route object.

            bool routeIsValid = Helpers.RouteHelper.ValidateRoute(route); // Validate the Route.

            if (routeIsValid)
            {
                Database_Queries.RouteDbQueries.AddRoute(route); // Add the Route to the database.
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
