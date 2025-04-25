using System.Text.Json;
using System.Text.Json.Nodes;

namespace Routeplanner_API.Mappers
{
    public class LocationMapper
    {
        public static Location MapJsonbodyToLocationObject(JsonElement jsonBody)
        {
            return new Location()
            {
                name = jsonBody.GetProperty("name").GetString(),
                description = jsonBody.GetProperty("description").GetString(),
                latitude = jsonBody.GetProperty("latitude").GetDouble(),
                longitude = jsonBody.GetProperty("longitude").GetDouble(),
            };
        }
    }
}
