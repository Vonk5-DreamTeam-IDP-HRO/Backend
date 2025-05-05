using Routeplanner_API.Database_Queries;
using System.Text.Json;

namespace Routeplanner_API.UoWs
{
    public class LocationUoW
    {
        private readonly LocationDbQueries _locationDbQueries;

        public LocationUoW(LocationDbQueries locationDbQueries)
        {
            _locationDbQueries = locationDbQueries ?? throw new ArgumentNullException(nameof(locationDbQueries));
        }
        public Location[]? GetLocations()
        {
            return _locationDbQueries.GetLocations(); // Get all Locations from the database directlyu.
        }
        
        public void AddLocation(JsonElement jsonBody)
        {
            var location = Mappers.LocationMapper.MapJsonbodyToLocationObject(jsonBody); // Map jsonBody to a Location object.

            var locationIsValid = Helpers.LocationHelper.ValidateLocation(location); // Validate the Location.

            if(locationIsValid)
            {
                _locationDbQueries.AddLocation(location); // Add the Location to the database.
            }
            else
            {
                throw new NotImplementedException("Validation failed");
            }
        }
    }
}
