using Microsoft.EntityFrameworkCore;
using Routeplanner_API.Models;
using Route = Routeplanner_API.Models.Route;

namespace Routeplanner_API.Database_Queries
{
    /// <summary>
    /// Provides database operations for Route entities, including CRUD actions.
    /// </summary>
    public class RouteDbQueries : IRouteDbQueries
    {
        private readonly RouteplannerDbContext _context;

        public RouteDbQueries(RouteplannerDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Retrieves a route by its unique identifier, including related LocationRoutes.
        /// </summary>
        /// <param name="routeId">The unique identifier of the route.</param>
        /// <returns>The route matching the ID or null if not found.</returns>
        public async Task<Route?> GetByIdAsync(Guid routeId)
        {
            return await _context.Routes
                .Include(r => r.LocationRoutes)
                .FirstOrDefaultAsync(r => r.RouteId == routeId);
        }

        /// <summary>
        /// Retrieves all routes including their related LocationRoutes.
        /// </summary>
        /// <returns>A collection of all routes.</returns>
        public async Task<IEnumerable<Route?>> GetAllAsync()
        {
            return await _context.Routes
                .Include(r => r.LocationRoutes)
                .ToListAsync();
        }

        /// <summary>
        /// Adds a new route to the database.
        /// </summary>
        /// <param name="route">The route entity to create.</param>
        /// <returns>The created route entity.</returns>
        public async Task<Route> CreateAsync(Route route)
        {
            ArgumentNullException.ThrowIfNull(route);
            await _context.Routes.AddAsync(route);
            await _context.SaveChangesAsync();
            return route;
        }

        /// <summary>
        /// Updates an existing route in the database.
        /// </summary>
        /// <param name="route">The route entity with updated data.</param>
        /// <returns>The updated route entity, or null if no matching route was found.</returns>
        public async Task<Route?> UpdateRouteAsync(Route route)
        {
            var existingRoute = await _context.Routes.FindAsync(route.RouteId);
            if (existingRoute == null)
            {
                return null;
            }
            _context.Entry(existingRoute).CurrentValues.SetValues(route);
            await _context.SaveChangesAsync();
            return existingRoute;
        }

        /// <summary>
        /// Deletes a route by its unique identifier.
        /// </summary>
        /// <param name="routeId">The unique identifier of the route to delete.</param>
        /// <returns>True if the route was deleted; otherwise, false.</returns>
        public async Task<bool> DeleteRouteAsync(Guid routeId)
        {
            var routeToDelete = await _context.Routes.FindAsync(routeId);
            if (routeToDelete == null)
            {
                return false;
            }

            _context.Routes.Remove(routeToDelete);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
