using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Routeplanner_API.Database_Queries;
using Routeplanner_API.DTO;
using Routeplanner_API.DTO.Location;
using Routeplanner_API.Enums;
using Routeplanner_API.Models;

namespace Routeplanner_API.UoWs
{
    /// <summary>
    /// Unit of Work for managing location-related operations.
    /// Handles data retrieval, creation, update, and deletion of locations.
    /// </summary>
    public class LocationUoW
    {
        private readonly RouteplannerDbContext _context;
        private readonly ILocationDbQueries _locationDbQueries;
        private readonly IMapper _mapper;
        private readonly ILogger<LocationUoW> _logger;

        public LocationUoW(RouteplannerDbContext context, ILocationDbQueries locationDbQueries, IMapper mapper, ILogger<LocationUoW> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _locationDbQueries = locationDbQueries ?? throw new ArgumentNullException(nameof(locationDbQueries));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves all locations.
        /// </summary>
        /// <returns>A collection of <see cref="LocationDto"/>.</returns>
        public async Task<IEnumerable<LocationDto>> GetLocationsAsync()
        {
            _logger.LogInformation("Getting all locations");

            var locations = await _locationDbQueries.GetAllAsync();
            return _mapper.Map<IEnumerable<LocationDto>>(locations);
        }

        /// <summary>
        /// Retrieves unique categories from locations.
        /// </summary>
        /// <returns>A <see cref="StatusCodeResponseDto{T}"/> containing unique category names or a not found status.</returns>
        public async Task<StatusCodeResponseDto<IEnumerable<string?>>> GetUniqueCategoriesAsync()
        {
            _logger.LogInformation("Getting unique location categories");

            IEnumerable<string?> categories = await _locationDbQueries.GetUniqueCategoriesAsync();
            if (categories == null)
            {
                return CreateStatusResponseDto<IEnumerable<string?>>(StatusCodeResponse.NotFound, "No location categories found.", null);
            }
            return CreateStatusResponseDto<IEnumerable<string?>>(StatusCodeResponse.Success, "Categories found.", categories.Where(c => !string.IsNullOrEmpty(c)));
        }

        /// <summary>
        /// Retrieves a location by its unique identifier.
        /// </summary>
        /// <param name="locationId">The unique identifier of the location.</param>
        /// <returns>A <see cref="StatusCodeResponseDto{T}"/> containing the location or a not found status.</returns>
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

        /// <summary>
        /// Creates a new location.
        /// </summary>
        /// <param name="createLocationDto">The DTO containing location creation data.</param>
        /// <param name="userId">The ID of the user creating the location.</param>
        /// <returns>A <see cref="StatusCodeResponseDto{T}"/> containing the created location or an error status.</returns>
        public async Task<StatusCodeResponseDto<LocationDto>> CreateLocationAsync(CreateLocationDto createLocationDto, Guid userId)
        {
            _logger.LogInformation("Creating new location for userId: {UserId}", userId);

            var existingUser = await _context.Users.FindAsync(userId);
            if (existingUser == null)
            {
                _logger.LogWarning("UserId {UserId} not found in DB", userId);
                return CreateStatusResponseDto<LocationDto>(
                    StatusCodeResponse.Unauthorized,
                    "User does not exist in the database.",
                    null);
            }

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
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error when creating location: {@CreateLocationDto}", createLocationDto);
                return CreateStatusResponseDto<LocationDto>(
                    StatusCodeResponse.InternalServerError,
                    "An unexpected database error occurred.",
                    null);
            }
        }

        /// <summary>
        /// Updates an existing location by its ID.
        /// </summary>
        /// <param name="locationId">The unique identifier of the location to update.</param>
        /// <param name="updateLocationDto">The DTO containing updated location data.</param>
        /// <returns>A <see cref="StatusCodeResponseDto{T}"/> containing the updated location or a not found status.</returns>
        public async Task<StatusCodeResponseDto<LocationDto?>> UpdateLocationAsync(Guid locationId, UpdateLocationDto updateLocationDto)
        {
            _logger.LogInformation("Updating location with ID: {LocationId}", locationId);

            Location? existingLocation = await _locationDbQueries.GetByIdAsync(locationId);
            if (existingLocation == null)
            {
                _logger.LogWarning("Location with ID: {LocationId} not found for update", locationId);
                return CreateStatusResponseDto<LocationDto?>(StatusCodeResponse.NotFound, $"Location with ID: {locationId} not found for update", null);
            }

            _mapper.Map(updateLocationDto, existingLocation);

            Location? updatedLocation = await _locationDbQueries.UpdateAsync(existingLocation);
            _logger.LogInformation("Location with ID: {LocationId} updated successfully", locationId);
            return CreateStatusResponseDto<LocationDto?>(StatusCodeResponse.Success, $"Location with ID: {locationId} updated successfully", _mapper.Map<LocationDto>(updatedLocation));
        }

        /// <summary>
        /// Deletes a location by its unique identifier.
        /// </summary>
        /// <param name="locationId">The unique identifier of the location to delete.</param>
        /// <returns>A <see cref="StatusCodeResponseDto{T}"/> indicating success or failure of deletion.</returns>
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

        /// <summary>
        /// Retrieves all selectable locations as a flat list.
        /// </summary>
        /// <returns>A collection of <see cref="SelectableLocationDto"/>.</returns>
        /// <exception cref="Exception">Throws if an error occurs during retrieval.</exception>
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

        /// <summary>
        /// Retrieves selectable locations grouped by their categories.
        /// </summary>
        /// <returns>A <see cref="StatusCodeResponseDto{T}"/> containing a dictionary with categories as keys and list of locations as values.</returns>
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

        /// <summary>
        /// Retrieves all selectable locations under a specific category.
        /// </summary>
        /// <param name="nameCategory">The category name to filter locations.</param>
        /// <returns>A <see cref="StatusCodeResponseDto{T}"/> containing the filtered locations.</returns>
        public async Task<StatusCodeResponseDto<IEnumerable<SelectableLocationDto>>> GetAllLocationFromOneCategory(string nameCategory)
        {
            _logger.LogInformation("Getting all locations that falls under specific given category");

            IEnumerable<SelectableLocationDto> AllLocation = await _locationDbQueries.GetSelectableLocationsAsync();
            // TODO: Make seperate Db call in _locationDbQueries. 
            IEnumerable<SelectableLocationDto> sortedLocation = AllLocation.Where(L => L.Category == nameCategory);

            _logger.LogInformation("Succesfull retrieved and sorted all locations from one category.");
            return CreateStatusResponseDto(StatusCodeResponse.Success, "Sucessfully retrieved and sorted all location that contains the given category", sortedLocation);
        }

        /// <summary>
        /// Creates a standardized response DTO.
        /// </summary>
        /// <typeparam name="T">The type of data contained in the response.</typeparam>
        /// <param name="statusCodeResponse">The status code of the response.</param>
        /// <param name="message">An optional message associated with the response.</param>
        /// <param name="data">The data contained in the response.</param>
        /// <returns>A <see cref="StatusCodeResponseDto{T}"/> containing the response details.</returns>
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
