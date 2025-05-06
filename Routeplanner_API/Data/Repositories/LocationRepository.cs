using Microsoft.EntityFrameworkCore;
using Routeplanner_API.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Routeplanner_API.Data.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly RouteplannerDbContext _context;

        public LocationRepository(RouteplannerDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Location?> GetByIdAsync(int locationId)
        {
            return await _context.Locations
            // Uncomment if you want to include LocationDetail and OpeningTimes
                                 // .Include(l => l.LocationDetail) 
                                 // .Include(l => l.OpeningTimes) // Add other includes as needed
                                 .FirstOrDefaultAsync(l => l.LocationId == locationId);
        }

        public async Task<IEnumerable<Location>> GetAllAsync()
        {
            return await _context.Locations
                                 // .Include(l => l.LocationDetail)
                                 // .Include(l => l.OpeningTimes)
                                 .ToListAsync();
        }

        public async Task<Location> CreateAsync(Location location)
        {
            ArgumentNullException.ThrowIfNull(location);

            // location.CreatedAt = DateTime.UtcNow; // Already handled by AutoMapper profile
            // location.UpdatedAt = DateTime.UtcNow; // Already handled by AutoMapper profile

            await _context.Locations.AddAsync(location);
            await _context.SaveChangesAsync();
            return location;
        }

        public async Task<Location?> UpdateAsync(Location location)
        {
            ArgumentNullException.ThrowIfNull(location);

            var existingLocation = await _context.Locations.FindAsync(location.LocationId);
            if (existingLocation == null)
            {
                return null;
            }

            // EF Core's change tracker will automatically detect changes to properties
            // when SaveChangesAsync is called. If you are mapping from a DTO,
            // ensure all intended properties are updated on 'existingLocation'.
            // AutoMapper can help here in the service layer.
            // For a direct update like this, you'd typically update properties of 'existingLocation'
            // from 'location' (the input parameter).

            // Example of updating properties (assuming 'location' has the new values):
            _context.Entry(existingLocation).CurrentValues.SetValues(location);
            existingLocation.UpdatedAt = DateTime.UtcNow;

            // If you only want to update specific fields and 'location' is a detached entity
            // with only those fields set, you might need more granular updates:
            // existingLocation.Name = location.Name;
            // existingLocation.Description = location.Description;
            // ... etc.

            await _context.SaveChangesAsync();
            return existingLocation;
        }

        public async Task<bool> DeleteAsync(int locationId)
        {
            var locationToDelete = await _context.Locations.FindAsync(locationId);
            if (locationToDelete == null)
            {
                return false;
            }

            _context.Locations.Remove(locationToDelete);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
