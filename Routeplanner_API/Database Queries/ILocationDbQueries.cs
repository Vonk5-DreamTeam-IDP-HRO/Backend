    using Routeplanner_API.DTO.Location;
    using Routeplanner_API.Models;

    namespace Routeplanner_API.Database_Queries
    {
        public interface ILocationDbQueries
        {
            Task<Location?> GetByIdAsync(Guid locationId);
            Task<IEnumerable<Location>> GetAllAsync();
            Task<Location> CreateAsync(Location location);
            Task<Location?> UpdateAsync(Location location); // Returns null if not found
            Task<bool> DeleteAsync(Guid locationId);
            Task<IEnumerable<string?>> GetUniqueCategoriesAsync();
            Task<IEnumerable<SelectableLocationDto>> GetSelectableLocationsAsync();
            Task<IEnumerable<SelectableLocationDto>> GetAllLocationsFromOneCategoryAsync(string categoryName);

        }
    }
