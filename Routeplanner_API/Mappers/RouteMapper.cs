using System.Text.Json;

namespace Routeplanner_API.Mappers
{
    public class RouteMapper
    {
        public static Route MapJsonBodyToRouteObject(JsonElement jsonBody)
        {
            return new Route()
            {
                Name = jsonBody.GetProperty("name").GetString(),
                Description = jsonBody.GetProperty("description").GetString(),
                //locations = 
            };
        }
    }
}
