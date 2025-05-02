namespace Routeplanner_API.Helpers
{
    public class RouteHelper
    {
        public static bool ValidateRoute(Route route)
        {
            if (route != null)
            {
                if (route.Name != null && route.Description != null && route.Locations != null)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
