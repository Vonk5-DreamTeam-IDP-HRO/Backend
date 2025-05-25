using Microsoft.AspNetCore.Mvc;
using Routeplanner_API.Database_Queries;
using System.Text.Json;
using Routeplanner_API.UoWs;
using AutoMapper;
using Routeplanner_API.DTO.Route;
using Microsoft.AspNetCore.Authorization;

namespace Routeplanner_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RouteController : ControllerBase
    {
        private readonly RouteUoW _routeUoW;
        private readonly ILogger<RouteController> _logger;

        public RouteController(RouteUoW routeUoW, ILogger<RouteController> logger)
        {
            _routeUoW = routeUoW ?? throw new ArgumentNullException(nameof(routeUoW));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<RouteDto>>> GetRoutes()
        {
            try
            {
                var routes = await _routeUoW.GetRoutesAsync();
                if (routes == null || !routes.Any())
                {
                    _logger.LogInformation("No routes found.");
                    return NotFound("No routes found.");
                }
                return Ok(routes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all routes.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while retrieving routes.");
            }
        }

        [HttpGet("{routeId}", Name = "GetRouteById")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RouteDto>> GetRouteById(Guid routeId)
        {
            try
            {
                var route = await _routeUoW.GetRouteByIdAsync(routeId);
                if (route == null)
                {
                    _logger.LogInformation("Route with ID {RouteId} not found.", routeId);
                    return NotFound($"Route with ID {routeId} not found.");
                }
                return Ok(route);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting route with ID {RouteId}.", routeId);
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while retrieving the route.");
            }
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddRoute([FromBody] CreateRouteDto createRouteDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var createdRouteDto = await _routeUoW.CreateRouteAsync(createRouteDto);

                return CreatedAtAction(nameof(GetRouteById), new { routeId = createdRouteDto.RouteId }, createdRouteDto);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "Error adding route due to null argument.");
                return BadRequest(ex.Message);
            }
            catch (AutoMapperMappingException ex) // Catching potential mapping errors from UoW
            {
                _logger.LogError(ex, "Error adding route due to mapping issue: {ErrorMessage}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred during data mapping: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (JsonException ex) // Should be less likely now but good to keep for other JSON issues
            {
                _logger.LogError(ex, "Error adding route due to JSON processing issue.");
                return BadRequest($"Invalid JSON format or data: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a new route.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while adding the route.");
            }
        }
    }
}
