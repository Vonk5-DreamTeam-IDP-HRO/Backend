﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Routeplanner_API.DTO;
using Routeplanner_API.DTO.Location;
using Routeplanner_API.Enums;
using Routeplanner_API.UoWs;
using System.Security.Claims;

namespace Routeplanner_API.Controllers
{
    /// <summary>
    /// API controller for managing location-related operations such as retrieval, creation, updating, and deletion of locations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class LocationController : ControllerBase
    {
        private readonly LocationUoW _locationUoW;
        private readonly ILogger<LocationController> _logger;

        public LocationController(LocationUoW locationUoW, ILogger<LocationController> logger)
        {
            _locationUoW = locationUoW ?? throw new ArgumentNullException(nameof(locationUoW));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves a list of all locations.
        /// </summary>
        /// <returns>A status response containing a collection of <see cref="LocationDto"/> objects.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StatusCodeResponseDto<IEnumerable<LocationDto>>>> GetLocations()
        {
            _logger.LogInformation("Executing LocationController.GetLocations");
            try
            {
                var locations = await _locationUoW.GetLocationsAsync();
                return _locationUoW.CreateStatusResponseDto<IEnumerable<LocationDto>>(StatusCodeResponse.Success, null, locations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all locations.");
                return _locationUoW.CreateStatusResponseDto<IEnumerable<LocationDto>>(StatusCodeResponse.InternalServerError, "An unexpected error occurred while retrieving locations.", null);
            }
        }

        /// <summary>
        /// Retrieves a list of unique location categories.
        /// </summary>
        /// <returns>A status response containing a collection of unique category strings.</returns>
        [HttpGet("categories")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StatusCodeResponseDto<IEnumerable<string?>>>> GetUniqueCategories()
        {
            _logger.LogInformation("Executing LocationController.GetUniqueCategories");
            try
            {
                _logger.LogInformation("Getting unique location categories");
                return await _locationUoW.GetUniqueCategoriesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting unique location categories.");
                return _locationUoW.CreateStatusResponseDto<IEnumerable<string?>>(StatusCodeResponse.InternalServerError, "An unexpected error occurred while retrieving unique location categories.", null);
            }
        }

        /// <summary>
        /// Retrieves a location by its unique identifier.
        /// </summary>
        /// <param name="locationId">The unique identifier of the location to retrieve.</param>
        /// <returns>A status response containing the <see cref="LocationDto"/> or null if not found.</returns>
        [HttpGet("{locationId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StatusCodeResponseDto<LocationDto?>>> GetLocationById(Guid locationId)
        {
            _logger.LogInformation("Executing LocationController.GetLocationsById");
            try
            {
                return await _locationUoW.GetLocationByIdAsync(locationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting location with ID {LocationId}.", locationId);
                return _locationUoW.CreateStatusResponseDto<LocationDto?>(StatusCodeResponse.InternalServerError, $"An unexpected error occurred while retrieving location {locationId}.", null);
            }
        }

        /// <summary>
        /// Creates a new location using the provided data.
        /// </summary>
        /// <param name="createLocationDto">The data required to create a new location.</param>
        /// <returns>A status response containing the created <see cref="LocationDto"/>.</returns>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StatusCodeResponseDto<LocationDto>>> CreateLocation([FromBody] CreateLocationDto createLocationDto)
        {
            _logger.LogInformation("Executing LocationController.CreateLocation");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("CreateLocation called with invalid model state.");
                return _locationUoW.CreateStatusResponseDto<LocationDto>(StatusCodeResponse.BadRequest, "CreateLocation called with invalid model state.", null);
            }

            // Get user ID here
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                _logger.LogWarning("Authenticated user has missing or invalid UserId claim.");
                return _locationUoW.CreateStatusResponseDto<LocationDto>(StatusCodeResponse.Unauthorized, "User ID claim is missing or invalid.", null);
            }

            try
            {
                return await _locationUoW.CreateLocationAsync(createLocationDto, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a new location. Input: {@CreateLocationDto}", createLocationDto);
                return _locationUoW.CreateStatusResponseDto<LocationDto>(StatusCodeResponse.InternalServerError, "An unexpected error occurred while creating the location.", null);
            }
        }

        /// <summary>
        /// Updates an existing location with the provided data.
        /// </summary>
        /// <param name="locationId">The unique identifier of the location to update.</param>
        /// <param name="locationDto">The updated location information.</param>
        /// <returns>A status response containing the updated <see cref="LocationDto"/> or null.</returns>
        [HttpPut("{locationId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StatusCodeResponseDto<LocationDto?>>> UpdateLocation(Guid locationId, [FromBody] UpdateLocationDto locationDto)
        {
            _logger.LogInformation("Executing LocationController.UpdateLocation");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("UpdateLocation called with invalid model state for ID {LocationId}.", locationId);
                return _locationUoW.CreateStatusResponseDto<LocationDto?>(StatusCodeResponse.BadRequest, $"UpdateLocation called with invalid model state for ID {locationId}.", null);
            }

            try
            {
                return await _locationUoW.UpdateLocationAsync(locationId, locationDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating location with ID {LocationId}. Input: {@locationDto}", locationId, locationDto);
                return _locationUoW.CreateStatusResponseDto<LocationDto?>(StatusCodeResponse.InternalServerError, $"An unexpected error occurred while updating location {locationId}.", null);
            }
        }

        /// <summary>
        /// Deletes a location by its unique identifier.
        /// </summary>
        /// <param name="locationId">The unique identifier of the location to delete.</param>
        /// <returns>A status response indicating whether the deletion was successful.</returns>
        [HttpDelete("{locationId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<StatusCodeResponseDto<bool>> DeleteLocation(Guid locationId)
        {
            _logger.LogInformation("Executing LocationController.DeleteLocation");

            try
            {
                return await _locationUoW.DeleteLocationAsync(locationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting location with ID {LocationId}.", locationId);
                return _locationUoW.CreateStatusResponseDto<bool>(StatusCodeResponse.InternalServerError, $"An unexpected error occurred while deleting location {locationId}.", false);
            }
        }

        /// <summary>
        /// Retrieves selectable locations grouped by a key.
        /// </summary>
        /// <returns>A status response containing a dictionary grouping keys to lists of <see cref="SelectableLocationDto"/>.</returns>
        [HttpGet("GroupedSelectableLocations")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<StatusCodeResponseDto<Dictionary<string, List<SelectableLocationDto>>>> GetGroupedSelectableLocations()
        {
            _logger.LogInformation("Executing LocationController.GetGroupedSelectableLocations");

            try
            {
                return await _locationUoW.GetGroupedSelectableLocationsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting grouped selectable locations.");
                return _locationUoW.CreateStatusResponseDto<Dictionary<string, List<SelectableLocationDto>>>(StatusCodeResponse.InternalServerError, "An unexpected error occurred while retrieving grouped selectable locations.", null);
            }
        }

        /// <summary>
        /// Retrieves all selectable locations for a specified category.
        /// </summary>
        /// <param name="nameCategory">The name of the category to filter locations by.</param>
        /// <returns>A status response containing a collection of <see cref="SelectableLocationDto"/> for the given category.</returns>
        [HttpGet ("AllLocationsForOneCategory")]
        public async Task<StatusCodeResponseDto<IEnumerable<SelectableLocationDto>>> GetAllSelectableLocationsFromOneCategory(string nameCategory)
        {
            _logger.LogInformation("Executing LocationController.GetAllSelectableLocationsFromOneCategory");

            try
            {
                return await _locationUoW.GetAllLocationFromOneCategory(nameCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while getting all locations from one category.");
                return _locationUoW.CreateStatusResponseDto<IEnumerable<SelectableLocationDto>>(StatusCodeResponse.InternalServerError, "An unexpected error occured while getting all locations from one category", null);
            }
        }
    }
}
