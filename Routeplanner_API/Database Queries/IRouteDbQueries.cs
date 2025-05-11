using Route = Routeplanner_API.Models.Route;

namespace Routeplanner_API.Database_Queries;

public interface IRouteDbQueries
{
    Task<Route?> GetByIdAsync(int routeId);
    Task<IEnumerable<Route?>> GetAllAsync();
    Task<Route> CreateAsync(Route route);
    Task<Route?> UpdateAsync(Route route); // Returns null if not found
}
