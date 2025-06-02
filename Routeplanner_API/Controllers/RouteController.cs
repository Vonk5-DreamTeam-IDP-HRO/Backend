using Microsoft.AspNetCore.Mvc;
using Routeplanner_API.Database_Queries;
using System.Text.Json;
using Routeplanner_API.UoWs;
using AutoMapper;
using Routeplanner_API.DTO.Route;
using Routeplanner_API.DTO.Location;
using Routeplanner_API.DTO.User;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Routeplanner_API.DTO;
using Routeplanner_API.Enums;
using Routeplanner_API.Models;

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
