using System.Text.Json;

namespace Routeplanner_API.UoWs
{
    public class LocationUoW
    {
        public static Location[]? GetLocations()
        {
            return Database_Queries.LocationDbQueries.GetLocations();
        }
        
        public static void AddLocation(JsonElement jsonBody)
        {
            Location location = Mappers.LocationMapper.MapJsonbodyToLocationObject(jsonBody);

            bool locationIsValid = Helpers.LocationHelper.ValidateLocation(location);

            if(locationIsValid)
            {
                Database_Queries.LocationDbQueries.AddLocation(location);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
