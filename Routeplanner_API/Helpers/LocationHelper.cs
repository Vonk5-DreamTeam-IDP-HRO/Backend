namespace Routeplanner_API.Helpers
{
    public class LocationHelper
    {
        public static bool ValidateLocation(Location location)
        {
            if(location != null)
            {
                if(location.Name != null && location.Description != null && location.Latitude != 0 && location.Longitude != 0)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool ValidateLocationDetails(LocationDetails locationDetails)
        {
            if (locationDetails != null)
            {
                if (locationDetails.Address != null
                    && locationDetails.City != null
                    && locationDetails.Country != null
                    && locationDetails.ZipCode != null
                    && locationDetails.PhoneNumber != null
                    && locationDetails.Website != null
                    && locationDetails.Category != null
                    && locationDetails.Accessibility != null)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
