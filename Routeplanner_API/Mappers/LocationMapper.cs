using System.Text.Json;
using System.Text.Json.Nodes;
using Routeplanner_API;


namespace Routeplanner_API.Mappers
{
    public class LocationMapper
    {
        public static Location MapJsonbodyToLocationObject(JsonElement jsonBody)
        {
            return new Location()
            {
                Name = jsonBody.GetProperty("name").GetString(),
                Description = jsonBody.GetProperty("description").GetString(),
                Latitude = jsonBody.GetProperty("latitude").GetDouble(),
                Longitude = jsonBody.GetProperty("longitude").GetDouble(),
            };
        }

        public static LocationDetails MapJsonbodyToLocationDetailsObject(JsonElement jsonBody)
        {
            return new LocationDetails()
            {
                Address = jsonBody.GetProperty("name").GetString(),
                City = jsonBody.GetProperty("name").GetString(),
                Country = jsonBody.GetProperty("name").GetString(),
                ZipCode = jsonBody.GetProperty("name").GetString(),
                PhoneNumber = jsonBody.GetProperty("name").GetString(),
                Website = jsonBody.GetProperty("name").GetString(),
                Category = jsonBody.GetProperty("name").GetString(),
                Accessibility = jsonBody.GetProperty("name").GetString(),
            };
        }
    }
}
