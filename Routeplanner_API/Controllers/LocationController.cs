using Microsoft.AspNetCore.Mvc;
using Routeplanner_API.UoWs;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Routeplanner_API.DTO.Location;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Routeplanner_API.Controllers
{
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

        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<LocationDto>>> GetLocations()
        {
            try
            {
                var locations = await _locationUoW.GetLocationsAsync();
                return Ok(locations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all locations.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An unexpected error occurred while retrieving locations.");
            }
        }

        [HttpGet("categories")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<string?>>> GetUniqueCategories()
        {
            try
            {
                _logger.LogInformation("Getting unique location categories");
                var categories = await _locationUoW.GetUniqueCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting unique location categories.");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    "An unexpected error occurred while retrieving unique location categories.");
            }
        }

        [HttpGet("{locationId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<LocationDto>> GetLocationById(Guid locationId)
        {
            try
            {
                var location = await _locationUoW.GetLocationByIdAsync(locationId);
                return Ok(location);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Location with ID {LocationId} not found (404).", locationId);
                return NotFound($"Location with ID {locationId} not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting location with ID {LocationId}.", locationId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An unexpected error occurred while retrieving location {locationId}.");
            }
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<LocationDto>> CreateLocation([FromBody] CreateLocationDto createLocationDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("CreateLocation called with invalid model state.");
                return BadRequest(ModelState);
            }

            // Get UserId from authenticated user's claims
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                _logger.LogWarning("CreateLocation called by an authenticated user with missing or invalid UserId claim.");
                // Consider returning 400 Bad Request or 401 Unauthorized if claim is essential and missing/malformed
                return StatusCode(StatusCodes.Status400BadRequest, "User ID claim is missing or invalid.");
            }

            try
            {
                // Pass the authenticated userId to the Unit of Work method
                var createdLocation = await _locationUoW.CreateLocationAsync(createLocationDto, userId);
                return CreatedAtAction(nameof(GetLocationById), new { locationId = createdLocation.LocationId },
                    createdLocation);
            }

            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument when creating location: {@CreateLocationDto}",
                    createLocationDto);
                return UnprocessableEntity(ex.Message);
            }

            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Conflict when creating location: {@CreateLocationDto}", createLocationDto);
                return Conflict(ex.Message);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a new location. Input: {@CreateLocationDto}",
                    createLocationDto);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An unexpected error occurred while creating the location.");
            }
        }

        [HttpPut("{locationId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<LocationDto>> UpdateLocation(Guid locationId, [FromBody] LocationDto locationDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("UpdateLocation called with invalid model state for ID {LocationId}.", locationId);
                return BadRequest(ModelState);
            }

            try
            {
                var updatedLocation = await _locationUoW.UpdateLocationAsync(locationId, locationDto);
                if (updatedLocation == null)
                {
                    _logger.LogWarning("Attempted to update non-existent location with ID {LocationId}.", locationId);
                    return NotFound($"Location with ID {locationId} not found for update.");
                }
                return Ok(updatedLocation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating location with ID {LocationId}. Input: {@locationDto}", locationId, locationDto);
                return StatusCode(StatusCodes.Status500InternalServerError, $"An unexpected error occurred while updating location {locationId}.");
            }
        }

        [HttpDelete("{locationId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteLocation(Guid locationId)
        {
            try
            {
                var success = await _locationUoW.DeleteLocationAsync(locationId);
                if (!success)
                {
                    _logger.LogWarning("Attempted to delete non-existent location with ID {LocationId}.", locationId);
                    return NotFound($"Location with ID {locationId} not found for deletion.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting location with ID {LocationId}.", locationId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An unexpected error occurred while deleting location {locationId}.");
            }
        }

        [HttpGet("GroupedSelectableLocations")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGroupedSelectableLocations()
        {
            _logger.LogInformation("API endpoint called: GetGroupedSelectableLocations");
            try
            {
                var groupedLocations = await _locationUoW.GetGroupedSelectableLocationsAsync();
                return Ok(groupedLocations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting grouped selectable locations.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An unexpected error occurred while retrieving grouped selectable locations.");
            }
        }
    }
}
