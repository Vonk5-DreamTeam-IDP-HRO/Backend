namespace Routeplanner_API.Helpers
{
    public class RouteHelper
    {
        public static bool ValidateRoute(Route route)
        {
            if (route != null)
            {
                if (route.name != null || route.description != null || route.locations != null)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
