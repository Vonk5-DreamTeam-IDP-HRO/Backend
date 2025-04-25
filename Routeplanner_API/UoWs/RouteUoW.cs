using System.Text.Json;

namespace Routeplanner_API.UoWs
{
    public class RouteUoW
    {
        public static Route[]? GetRoutes()
        {
            return Database_Queries.RouteDbQueries.GetRoutes();
        }

        public static void AddRoute(JsonElement jsonBody)
        {
            Route route = Mappers.RouteMapper.MapJsonBodyToRouteObject(jsonBody);

            Database_Queries.RouteDbQueries.AddRoute(route);
        }
    }
}
