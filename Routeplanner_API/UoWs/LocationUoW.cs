using System.Text.Json;

namespace Routeplanner_API.UoWs
{
    public class LocationUoW
    {
        public static void AddLocation(JsonElement jsonBody)
        {
            Location location = Mappers.LocationMapper.MapJsonbodyToLocationObject(jsonBody);

            Database_Queries.LocationDbQueries.AddLocation(location);
        }
    }
}
