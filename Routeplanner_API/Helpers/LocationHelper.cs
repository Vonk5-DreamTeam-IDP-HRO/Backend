namespace Routeplanner_API.Helpers
{
    public class LocationHelper
    {
        private static List<string> errors = new List<string>();

        public static bool isLocationValid(Location location)
        {
            ValidateLocation(location);
            return !errors.Any();
        }

        public static void ValidateLocation(Location location)
        {
            if (location == null)
            {
                errors.Add("Location cannot be null.");
                return;
            }

            if (string.IsNullOrWhiteSpace(location.Name))
            {
                errors.Add("Name is required.");
            }

            if (location.Name != null && location.Name.Length > 255)
            {
                errors.Add("Name must not exceed 255 characters.");
            }

            if (location.Latitude == 0)
            {
                errors.Add("Latitude cannot be 0");
            }

            if (location.Longitude == 0)
            {
                errors.Add("Longitude cannot be 0");
            }
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

// Check if location is null.
// Check if name already exists
// Check if name is not null.
// Check if name to too long for varchar
// Check if lat long is not null.
// Check if lat long is on land. 