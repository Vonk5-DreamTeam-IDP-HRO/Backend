using Microsoft.AspNetCore.Mvc;
using Routeplanner_API.UoWs;
using System.Text.Json;

namespace Routeplanner_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class LocationController : ControllerBase
    {
        private readonly LocationUoW _locationUoW;
        private readonly ILogger<LocationController> _logger; // TODO: Implement better and more specific logging for example using ILogger

        public LocationController(LocationUoW locationUoW, ILogger<LocationController> logger)
        {
            _locationUoW = locationUoW ?? throw new ArgumentNullException(nameof(locationUoW));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public ActionResult<Location[]> GetLocations()
        {
            try
            {
                var locations = _locationUoW.GetLocations();
                if (locations == null || locations.Length == 0)
                {
                    _logger.LogWarning("GetLocations returned null, possibly due to a database issue.");
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to retrieve locations.");
                }
                return Ok(locations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting locations.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        // TODO: refactor this method to use the LocationUoW
        [HttpPost]
        public IActionResult AddLocation([FromBody] JsonElement jsonBody)
        {
            try
            {
                _locationUoW.AddLocation(jsonBody);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok("Location added successfully.");
        }

        [HttpPost]
        public IActionResult AddLocationDetails([FromBody] JsonElement jsonBody)
        {
            try
            {
                UoWs.LocationUoW.AddLocationDetails(jsonBody);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok("Location added successfully.");
        }
    }
}
