using System.Text.Json;

namespace Routeplanner_API.UoWs
{
    public class RouteUoW
    {
        public static void AddRoute(JsonElement jsonBody)
        {
            Route route = Mappers.RouteMapper.MapJsonBodyToRouteObject(jsonBody);

            Database_Queries.RouteDbQueries.AddRoute(route);
        }
    }
}
