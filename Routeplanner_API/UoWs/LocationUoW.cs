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
            bool locationIsValid = Helpers.LocationHelper.isLocationValid(location); // Validate the Location.

            if(locationIsValid)
            {
                _locationDbQueries.AddLocation(location); // Add the Location to the database.
            }
            else
            {
                throw new NotImplementedException("Validation failed");
            }
        }

        public void AddLocationDetails(JsonElement jsonBody)
        {
            AddLocation(jsonBody); // Add the Location.

            LocationDetails locationDetails = Mappers.LocationMapper.MapJsonbodyToLocationDetailsObject(jsonBody); // Map jsonBody to a LocationDetails object.

            bool locationDetailsIsValid = Helpers.LocationHelper.ValidateLocationDetails(locationDetails); // Validate the LocationDetails.

            if (locationDetailsIsValid)
            {
                _locationDbQueries.AddLocationDetails(locationDetails); // Add the LocationDetails to the database.
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
