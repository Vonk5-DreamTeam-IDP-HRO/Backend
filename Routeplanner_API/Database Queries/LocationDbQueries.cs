using Microsoft.EntityFrameworkCore;
using Routeplanner_API.DTO.Location;
using Routeplanner_API.Models;

namespace Routeplanner_API.Database_Queries
{
    /// <summary>
    /// Provides database operations for locations, including retrieval, creation, updating, and deletion.
    /// Also supports queries for unique categories and selectable locations.
    /// </summary>
    public class LocationDbQueries : ILocationDbQueries
    {
        private readonly RouteplannerDbContext _context;

        public LocationDbQueries(RouteplannerDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Retrieves a location by its unique identifier.
        /// </summary>
        /// <param name="locationId">The ID of the location to retrieve.</param>
        /// <returns>The location if found; otherwise, null.</returns>
        public async Task<Location?> GetByIdAsync(Guid locationId)
        {
            return await _context.Locations.FirstOrDefaultAsync(l => l.LocationId == locationId);
        }

        /// <summary>
        /// Retrieves all locations.
        /// </summary>
        /// <returns>A list of all locations.</returns>
        public async Task<IEnumerable<Location>> GetAllAsync()
        {
            return await _context.Locations.ToListAsync();
        }

        /// <summary>
        /// Creates a new location.
        /// </summary>
        /// <param name="location">The location entity to create.</param>
        /// <returns>The created location.</returns>
        public async Task<Location> CreateAsync(Location location)
        {
            ArgumentNullException.ThrowIfNull(location);

            await _context.Locations.AddAsync(location);
            await _context.SaveChangesAsync();
            return location;
        }

        /// <summary>
        /// Updates an existing location and its details.
        /// </summary>
        /// <param name="location">The location entity with updated values.</param>
        /// <returns>The updated location if found; otherwise, null.</returns>
        public async Task<Location?> UpdateAsync(Location location)
        {
            ArgumentNullException.ThrowIfNull(location);

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
                    existingLocation.LocationDetail = location.LocationDetail;
                }
                else
                {
                    _context.Entry(existingLocation.LocationDetail).CurrentValues
                        .SetValues(location.LocationDetail);
                }
            }

            await _context.SaveChangesAsync();
            return existingLocation;
        }

        /// <summary>
        /// Deletes a location by its ID.
        /// </summary>
        /// <param name="locationId">The ID of the location to delete.</param>
        /// <returns>True if the location was deleted; otherwise, false.</returns>
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

        /// <summary>
        /// Retrieves a list of unique location categories.
        /// </summary>
        /// <returns>A list of distinct category names.</returns>
        public async Task<IEnumerable<string?>> GetUniqueCategoriesAsync()
        {
            return await _context.LocationDetails
                                 .Select(ld => ld.Category)
                                 .Distinct()
                                 .ToListAsync();
        }

        /// <summary>
        /// Retrieves selectable locations filtered by a specific category.
        /// </summary>
        /// <param name="categoryName">The category name to filter locations by.</param>
        /// <returns>A list of selectable locations in the specified category.</returns>
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

        /// <summary>
        /// Retrieves all selectable locations.
        /// </summary>
        /// <returns>A list of selectable locations.</returns>
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
                        Category = locationDetail.Category
                    })
                .ToListAsync();
        }
    }
}
