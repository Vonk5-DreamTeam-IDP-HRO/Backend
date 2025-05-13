using AutoMapper;
using Routeplanner_API.Models;
using Routeplanner_API.Database_Queries;
using Routeplanner_API.DTO.Location;

namespace Routeplanner_API.UoWs
{
    public class LocationUoW
    {
        private readonly ILocationDbQueries _locationDbQueries;
        private readonly IMapper _mapper;
        private readonly ILogger<LocationUoW> _logger;

        public LocationUoW(ILocationDbQueries locationDbQueries, IMapper mapper, ILogger<LocationUoW> logger)
        {
            _locationDbQueries = locationDbQueries ?? throw new ArgumentNullException(nameof(locationDbQueries));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<LocationDto>> GetLocationsAsync()
        {
            _logger.LogInformation("Getting all locations");
            var locations = await _locationDbQueries.GetAllAsync();
            return _mapper.Map<IEnumerable<LocationDto>>(locations);
        }

        public async Task<LocationDto?> GetLocationByIdAsync(int locationId)
        {
            _logger.LogInformation("Getting location with ID: {LocationId}", locationId);
            var location = await _locationDbQueries.GetByIdAsync(locationId);
            if (location == null)
            {
                _logger.LogWarning("Location with ID: {LocationId} not found", locationId);
                return null;
            }
            return _mapper.Map<LocationDto>(location);
        }

        public async Task<LocationDto> CreateLocationAsync(CreateLocationDto createLocationDto)
        {
            _logger.LogInformation("Creating new location");

            try
            {
                var locationEntity = _mapper.Map<Location>(createLocationDto);

                // Potentially set UserId if applicable and not directly from DTO
                // locationEntity.UserId = ...; 

                var createdLocation = await _locationDbQueries.CreateAsync(locationEntity);
                _logger.LogInformation("Location created successfully with ID: {LocationId}", createdLocation.LocationId);
                return _mapper.Map<LocationDto>(createdLocation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating location: {ErrorMessage}", ex.Message);
                throw;
            }
        }

        public async Task<LocationDto?> UpdateLocationAsync(int locationId, UpdateLocationDto updateLocationDto)
        {
            _logger.LogInformation("Updating location with ID: {LocationId}", locationId);

            var existingLocation = await _locationDbQueries.GetByIdAsync(locationId);
            if (existingLocation == null)
            {
                _logger.LogWarning("Location with ID: {LocationId} not found for update", locationId);
                return null;
            }

            try
            {
                // Map the changes from DTO to the existing entity
                _mapper.Map(updateLocationDto, existingLocation);

                // Ensure UpdatedAt is set (AutoMapper profile also does this, but explicit here is fine too)
                // existingLocation.UpdatedAt = DateTime.UtcNow;

                var updatedLocation = await _locationDbQueries.UpdateAsync(existingLocation);
                _logger.LogInformation("Location with ID: {LocationId} updated successfully", locationId);
                return _mapper.Map<LocationDto>(updatedLocation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating location with ID: {LocationId}: {ErrorMessage}", locationId, ex.Message);
                throw;
            }
        }

        public async Task<bool> DeleteLocationAsync(int locationId)
        {
            _logger.LogInformation("Deleting location with ID: {LocationId}", locationId);
            try
            {
                var result = await _locationDbQueries.DeleteAsync(locationId);
                if (result)
                {
                    _logger.LogInformation("Location with ID: {LocationId} deleted successfully", locationId);
                }
                else
                {
                    _logger.LogWarning("Location with ID: {LocationId} not found for deletion", locationId);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting location with ID: {LocationId}: {ErrorMessage}", locationId, ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<string?>> GetUniqueCategoriesAsync()
        {
            _logger.LogInformation("Getting unique location categories");
            try
            {
                var categories = await _locationDbQueries.GetUniqueCategoriesAsync();
                return categories.Where(c => !string.IsNullOrEmpty(c)); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unique location categories: {ErrorMessage}", ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<SelectableLocationDto>> GetSelectableLocationsAsync()
        {
            _logger.LogInformation("Getting all selectable locations (flat list)");
            try
            {
                var locations = await _locationDbQueries.GetSelectableLocationsAsync();
                return locations;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all selectable locations (flat list): {ErrorMessage}", ex.Message);
                throw; // Rethrow to be handled by the controller or global error handler
            }
        }

        public async Task<Dictionary<string, List<SelectableLocationDto>>> GetGroupedSelectableLocationsAsync()
        {
            _logger.LogInformation("Getting selectable locations grouped by category");
            try
            {
                // Fetch the flat list of selectable locations
                var selectableLocations = await _locationDbQueries.GetSelectableLocationsAsync();

                // Group by category
                var groupedLocations = selectableLocations
                    .GroupBy(s => s.Category ?? "Uncategorized") // Group by category, handle null/empty categories
                    .ToDictionary(
                        g => g.Key,
                        g => g.ToList()
                    );

                _logger.LogInformation("Successfully retrieved and grouped {Count} categories.", groupedLocations.Count);
                return groupedLocations;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting and grouping selectable locations: {ErrorMessage}", ex.Message);
                throw;
            }
        }
    }
}
