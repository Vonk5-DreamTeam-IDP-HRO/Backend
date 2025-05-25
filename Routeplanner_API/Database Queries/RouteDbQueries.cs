using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Routeplanner_API.Models;
using Route = Routeplanner_API.Models.Route;


namespace Routeplanner_API.Database_Queries
{
    public class RouteDbQueries : IRouteDbQueries
    {
        private readonly RouteplannerDbContext _context;

        public RouteDbQueries(RouteplannerDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Route?> GetByIdAsync(Guid routeId)
        {
            return await _context.Routes
                .FirstOrDefaultAsync(r => r.RouteId == routeId);
        }
        public async Task<IEnumerable<Route?>> GetAllAsync()
        {
            return await _context.Routes
                .ToListAsync();
        }

        public async Task<Route> CreateAsync(Route route)
        {
            ArgumentNullException.ThrowIfNull(route);
            await _context.Routes.AddAsync(route);
            await _context.SaveChangesAsync();
            return route;
        }

        public async Task<Route?> UpdateAsync(Route route)
        {
            ArgumentNullException.ThrowIfNull(route);
            var existingRoute = await _context.Routes.FindAsync(route.RouteId);
            if (existingRoute == null)
            {
                return null;
            }
            // EF Core's change tracker will automatically detect changes to properties
            // when SaveChangesAsync is called. If you are mapping from a DTO,
            // ensure all intended properties are updated on 'existingRoute'.
            // AutoMapper can help here in the service layer.
            // For a direct update like this, you'd typically update properties of 'existingRoute'
            // from 'route' (the input parameter).
            // Example of updating properties (assuming 'route' has the new values):
            _context.Entry(existingRoute).CurrentValues.SetValues(route);
            existingRoute.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return existingRoute;
        }

        public async Task<bool> DeleteRouteAsync(Guid routeId)
        {
            var routeToDelete = await _context.Users.FindAsync(routeId);
            if (routeToDelete == null)
            {
                return false;
            }

            _context.Users.Remove(routeToDelete);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
