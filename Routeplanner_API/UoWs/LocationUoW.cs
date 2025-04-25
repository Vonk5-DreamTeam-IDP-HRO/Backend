using System.Text.Json;

namespace Routeplanner_API.UoWs
{
    public class LocationUoW
    {
        public static Location[]? GetLocations()
        {
            return Database_Queries.LocationDbQueries.GetLocations(); // Get the Locations from the database.
        }
        
        public static void AddLocation(JsonElement jsonBody)
        {
            Location location = Mappers.LocationMapper.MapJsonbodyToLocationObject(jsonBody); // Map jsonBody to a Location object.

            bool locationIsValid = Helpers.LocationHelper.ValidateLocation(location); // Validate the Location.

            if(locationIsValid)
            {
                Database_Queries.LocationDbQueries.AddLocation(location); // Add the Location to the database.
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
