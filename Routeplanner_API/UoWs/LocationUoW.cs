using AutoMapper;
using Routeplanner_API.Models;
using Routeplanner_API.Database_Queries;
using Routeplanner_API.DTO.Location;
using Routeplanner_API.DTO;
using Routeplanner_API.Enums;
using System.Security.Claims;

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

        public async Task<StatusCodeResponseDto<IEnumerable<string?>>> GetUniqueCategoriesAsync()
        {
            _logger.LogInformation("Getting unique location categories");

            IEnumerable<string?> categories = await _locationDbQueries.GetUniqueCategoriesAsync();
            if(categories == null)
            {
                return CreateStatusResponseDto<IEnumerable<string?>>(StatusCodeResponse.NotFound, "No location categories found.", null);
            }
            return CreateStatusResponseDto<IEnumerable<string?>>(StatusCodeResponse.Success, "Categories found.", categories.Where(c => !string.IsNullOrEmpty(c)));
        }

        public async Task<StatusCodeResponseDto<LocationDto?>> GetLocationByIdAsync(Guid locationId)
        {
            _logger.LogInformation("Getting location with ID: {LocationId}", locationId);

            Location? location = await _locationDbQueries.GetByIdAsync(locationId);
            if (location == null)
            {
                _logger.LogWarning("Location with ID: {LocationId} not found", locationId);
                return CreateStatusResponseDto<LocationDto?>(StatusCodeResponse.NotFound, $"Location with ID: {locationId} not found", null);
            }
            return CreateStatusResponseDto<LocationDto?>(StatusCodeResponse.Success, $"Location with ID: {locationId} found.", _mapper.Map<LocationDto?>(location));
        }

        public async Task<StatusCodeResponseDto<LocationDto>> CreateLocationAsync(CreateLocationDto createLocationDto, Guid userId)
        {
            _logger.LogInformation("Creating new location");

            // Use userId directly
            Location locationEntity = _mapper.Map<Location>(createLocationDto, opt => opt.Items["UserId"] = userId);

            try
            {
                Location createdLocation = await _locationDbQueries.CreateAsync(locationEntity);
                _logger.LogInformation("Location created successfully with ID: {LocationId} for UserId: {UserId}", createdLocation.LocationId, userId);

                return CreateStatusResponseDto<LocationDto>(StatusCodeResponse.Success, $"Location created successfully with ID: {createdLocation.LocationId}", _mapper.Map<LocationDto>(createdLocation));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument when creating location: {@CreateLocationDto}", createLocationDto);
                return CreateStatusResponseDto<LocationDto>(StatusCodeResponse.BadRequest, $"Invalid argument when creating location: {createLocationDto}", null);
            }
        }


        public async Task<StatusCodeResponseDto<LocationDto?>> UpdateLocationAsync(Guid locationId, UpdateLocationDto updateLocationDto)
        {
            _logger.LogInformation("Updating location with ID: {LocationId}", locationId);

            Location? existingLocation = await _locationDbQueries.GetByIdAsync(locationId);
            if (existingLocation == null)
            {
                _logger.LogWarning("Location with ID: {LocationId} not found for update", locationId);
                return CreateStatusResponseDto<LocationDto?>(StatusCodeResponse.NotFound, $"Location with ID: {locationId} not found for update", null);
            }

            // Map the changes from DTO to the existing entity
            _mapper.Map(updateLocationDto, existingLocation);

            // Ensure UpdatedAt is set (AutoMapper profile also does this, but explicit here is fine too)
            // existingLocation.UpdatedAt = DateTime.UtcNow;

            Location? updatedLocation = await _locationDbQueries.UpdateAsync(existingLocation);
            _logger.LogInformation("Location with ID: {LocationId} updated successfully", locationId);
            return CreateStatusResponseDto<LocationDto?>(StatusCodeResponse.Success, $"Location with ID: {locationId} updated successfully", _mapper.Map<LocationDto>(updatedLocation));
        }

        public async Task<StatusCodeResponseDto<bool>> DeleteLocationAsync(Guid locationId)
        {
            _logger.LogInformation("Deleting location with ID: {LocationId}", locationId);

            bool result = await _locationDbQueries.DeleteAsync(locationId);
            if (result)
            {
                _logger.LogInformation("Location with ID: {LocationId} deleted successfully", locationId);
                return CreateStatusResponseDto<bool>(StatusCodeResponse.Success, $"Location with ID: {locationId} deleted successfully", true);
            }
            else
            {
                _logger.LogWarning("Location with ID: {LocationId} not found for deletion", locationId);
                return CreateStatusResponseDto<bool>(StatusCodeResponse.NotFound, $"Location with ID: {locationId} not found for deletion", false);
            }     
        }

        public async Task<IEnumerable<SelectableLocationDto>> GetSelectableLocationsAsync() // Has 0 references? 
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

        public async Task<StatusCodeResponseDto<Dictionary<string, List<SelectableLocationDto>>>> GetGroupedSelectableLocationsAsync()
        {
            _logger.LogInformation("Getting selectable locations grouped by category");

            // Fetch the flat list of selectable locations
            IEnumerable<SelectableLocationDto> selectableLocations = await _locationDbQueries.GetSelectableLocationsAsync();

            // Group by category
            Dictionary<string, List<SelectableLocationDto>> groupedLocations = selectableLocations
                .GroupBy(s => s.Category ?? "Uncategorized") // Group by category, handle null/empty categories
                .ToDictionary(
                    g => g.Key,
                    g => g.ToList()
                );

            _logger.LogInformation("Successfully retrieved and grouped {Count} categories.", groupedLocations.Count);
            return CreateStatusResponseDto<Dictionary<string, List<SelectableLocationDto>>>(StatusCodeResponse.Success, $"Successfully retrieved and grouped {groupedLocations.Count} categories.", groupedLocations);
        }

        public StatusCodeResponseDto<T> CreateStatusResponseDto<T>(StatusCodeResponse statusCodeResponse, string? message, T? data)
        {
            return new StatusCodeResponseDto<T>
            {
                StatusCodeResponse = statusCodeResponse,
                Message = message,
                Data = data
            };
        }
    }
}
