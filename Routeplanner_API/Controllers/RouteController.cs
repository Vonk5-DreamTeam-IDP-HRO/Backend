using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Routeplanner_API.DTO;
using Routeplanner_API.DTO.Route;
using Routeplanner_API.Enums;
using Routeplanner_API.Models;
using Routeplanner_API.UoWs;
using System.Security.Claims;

namespace Routeplanner_API.Controllers
{
    /// <summary>
    /// API controller for managing route-related operations such as retrieval, creation, updating, and deletion of routes.
    /// </summary>
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

        /// <summary>
        /// Retrieves a list of all routes.
        /// </summary>
        /// <returns>A status response containing a collection of <see cref="RouteDto"/> objects.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StatusCodeResponseDto<IEnumerable<RouteDto>>>> GetRoutes()
        {
            _logger.LogInformation("Executing RouteController.GetRoutes");
            try
            {
                IEnumerable<RouteDto> routes = await _routeUoW.GetRoutesAsync();
                if (routes == null || !routes.Any())
                {
                    _logger.LogInformation("No routes found.");
                    return _routeUoW.CreateStatusResponseDto<IEnumerable<RouteDto>>(StatusCodeResponse.NotFound, "No routes found.", null);
                }
                return _routeUoW.CreateStatusResponseDto<IEnumerable<RouteDto>>(StatusCodeResponse.Success, null, routes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all routes.");
                return _routeUoW.CreateStatusResponseDto<IEnumerable<RouteDto>>(StatusCodeResponse.InternalServerError, "An unexpected error occurred while retrieving routes.", null);
            }
        }

        /// <summary>
        /// Retrieves a route by its unique identifier.
        /// </summary>
        /// <param name="routeId">The unique identifier of the route to retrieve.</param>
        /// <returns>A status response containing the <see cref="RouteDto"/> or null if not found.</returns>
        [HttpGet("{routeId}", Name = "GetRouteById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StatusCodeResponseDto<RouteDto?>>> GetRouteById(Guid routeId)
        {
            _logger.LogInformation("Executing RouteController.GetRouteById");

            try
            {
                return await _routeUoW.GetRouteByIdAsync(routeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting route with ID {RouteId}.", routeId);
                return _routeUoW.CreateStatusResponseDto<RouteDto?>(StatusCodeResponse.InternalServerError, "An unexpected error occurred while retrieving the route.", null);
            }
        }

        /// <summary>
        /// Creates a new route using the provided data.
        /// </summary>
        /// <param name="createRouteDto">The data required to create a new route.</param>
        /// <returns>A status response containing an identifier or message related to the created route.</returns>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<StatusCodeResponseDto<string?>> CreateRoute([FromBody] CreateRouteDto createRouteDto)
        {
            _logger.LogInformation("Executing RouteController.CreateRoute");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("CreateRoute called with invalid model state.");
                return _routeUoW.CreateStatusResponseDto<string?>(StatusCodeResponse.BadRequest, "CreateRoute called with invalid model state.", null);
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                _logger.LogWarning("CreateRoute called by user with invalid/missing UserId claim.");
                return _routeUoW.CreateStatusResponseDto<string?>(StatusCodeResponse.Unauthorized, "User ID claim is missing or invalid.", null);
            }

            try
            {
                return await _routeUoW.CreateRouteAsync(createRouteDto, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a new route.");
                return _routeUoW.CreateStatusResponseDto<string?>(StatusCodeResponse.InternalServerError, "An unexpected error occurred while adding the route.", null);
            }
        }

        /// <summary>
        /// Updates an existing route identified by its ID using the provided data.
        /// </summary>
        /// <param name="routeId">The unique identifier of the route to update.</param>
        /// <param name="updateRouteDto">The updated data for the route.</param>
        /// <returns>A status response containing the updated <see cref="RouteDto"/> or null if not found.</returns>
        [HttpPut("{routeId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StatusCodeResponseDto<RouteDto?>>> UpdateRoute(Guid routeId, [FromBody] UpdateRouteDto updateRouteDto)
        {
            _logger.LogInformation("Executing RouteController.UpdateRoute");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("UpdateRoute called with invalid model state for ID {routeId}.", routeId);
                return _routeUoW.CreateStatusResponseDto<RouteDto?>(StatusCodeResponse.BadRequest, $"UpdateRoute called with invalid model state for ID {routeId}.", null);
            }
            try
            {
                return await _routeUoW.UpdateRouteAsync(routeId, updateRouteDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating route with ID {routeId}. Input: {@updateRouteDto}", routeId, updateRouteDto);
                return _routeUoW.CreateStatusResponseDto<RouteDto?>(StatusCodeResponse.InternalServerError, $"An unexpected error occurred while updating route {routeId}.", null);
            }
        }

        /// <summary>
        /// Deletes a route identified by its unique ID.
        /// </summary>
        /// <param name="routeId">The unique identifier of the route to delete.</param>
        /// <returns>A status response indicating success or failure of the deletion.</returns>
        [HttpDelete("{routeId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<StatusCodeResponseDto<bool>> DeleteRoute(Guid routeId)
        {
            _logger.LogInformation("Executing RouteController.DeleteRoute");
            try
            {
                return await _routeUoW.DeleteRouteAsync(routeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting route with ID {routeId}.", routeId);
                return _routeUoW.CreateStatusResponseDto<bool>(StatusCodeResponse.InternalServerError, $"An unexpected error occurred while deleting route {routeId}.", false);
            }
        }
    }
}
