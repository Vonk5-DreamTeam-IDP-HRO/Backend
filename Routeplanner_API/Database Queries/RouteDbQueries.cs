using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Routeplanner_API.Data;
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

        public async Task<Route?> GetByIdAsync(int routeId)
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
            _context.Entry(existingRoute).CurrentValues.SetValues(route);
            existingRoute.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return existingRoute;
        }
    }
}
