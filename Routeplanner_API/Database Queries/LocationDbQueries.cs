using Microsoft.EntityFrameworkCore;
using Routeplanner_API.Models;
using Routeplanner_API.DTO.Location;
using System;

namespace Routeplanner_API.Database_Queries
{
    public class LocationDbQueries : ILocationDbQueries
    {
        private readonly RouteplannerDbContext _context;

        public LocationDbQueries(RouteplannerDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Location?> GetByIdAsync(Guid locationId)
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

            // Load existing Location including LocationDetail
            var existingLocation = await _context.Locations
                .Include(l => l.LocationDetail)
                .FirstOrDefaultAsync(l => l.LocationId == location.LocationId);

            if (existingLocation == null)
            {
                return null;
            }

            // Update scalar properties
            _context.Entry(existingLocation).CurrentValues.SetValues(location);
            existingLocation.UpdatedAt = DateTime.UtcNow;

            // Update or assign LocationDetail
            if (location.LocationDetail != null)
            {
                if (existingLocation.LocationDetail == null)
                {
                    // No detail existed before, so assign new one
                    existingLocation.LocationDetail = location.LocationDetail;
                }
                else
                {
                    // Update existing detail
                    _context.Entry(existingLocation.LocationDetail).CurrentValues
                        .SetValues(location.LocationDetail);
                }
            }

            await _context.SaveChangesAsync();
            return existingLocation;
        }


        public async Task<bool> DeleteAsync(Guid locationId)
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

        public async Task<IEnumerable<string?>> GetUniqueCategoriesAsync()
        {
            return await _context.LocationDetails
                                 .Select(ld => ld.Category)
                                 .Distinct()
                                 .ToListAsync();
        }

        public async Task<IEnumerable<SelectableLocationDto>> GetAllLocationsFromOneCategoryAsync(string categoryName)
        {
            return await _context.Locations
                .Join(
                    _context.LocationDetails.Where(ld => ld.Category == categoryName),
                    location => location.LocationId,
                    locationDetail => locationDetail.LocationId,
                    (location, locationDetail) => new SelectableLocationDto
                    {
                        LocationId = location.LocationId,
                        Name = location.Name,
                        Category = locationDetail.Category
                    })
                .ToListAsync();

        }
        public async Task<IEnumerable<SelectableLocationDto>> GetSelectableLocationsAsync()
        {
            return await _context.Locations
                .Join(
                    _context.LocationDetails,
                    location => location.LocationId,
                    locationDetail => locationDetail.LocationId,
                    (location, locationDetail) => new SelectableLocationDto
                    {
                        LocationId = location.LocationId,
                        Name = location.Name,
                        Category = locationDetail.Category // This comes from LocationDetail
                    })
                .ToListAsync();
        }
    }
}
