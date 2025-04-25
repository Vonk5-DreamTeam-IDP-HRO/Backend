namespace Routeplanner_API.Helpers
{
    public class LocationHelper
    {
        public static bool ValidateLocation(Location location)
        {
            if(location != null)
            {
                if(location.name != null || location.description != null || location.latitude != null || location.longitude != null)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
