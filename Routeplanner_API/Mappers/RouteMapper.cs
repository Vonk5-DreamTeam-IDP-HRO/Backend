using System.Text.Json;

namespace Routeplanner_API.Mappers
{
    public class RouteMapper
    {
        public static Route MapJsonBodyToRouteObject(JsonElement jsonBody)
        {
            return new Route()
            {
                name = jsonBody.GetProperty("name").GetString(),
                description = jsonBody.GetProperty("description").GetString(),
                //locations = 
            };
        }
    }
}
