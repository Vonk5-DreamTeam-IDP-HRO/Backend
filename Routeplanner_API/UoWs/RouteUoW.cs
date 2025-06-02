using System.Text.Json;
using AutoMapper;
using Routeplanner_API.Database_Queries;
using Routeplanner_API.DTO;
using Routeplanner_API.DTO.Route;
using Routeplanner_API.DTO.User;
using Routeplanner_API.Enums;
using Routeplanner_API.Models;
using System.Security.Claims;


namespace Routeplanner_API.UoWs
{
    public class RouteUoW
    {
        private readonly IRouteDbQueries _routeDbQueries;
        private readonly ILogger<RouteUoW> _logger;
        private readonly IMapper _mapper;

        public RouteUoW(IRouteDbQueries routeDbQueries, ILogger<RouteUoW> logger, IMapper mapper)
        {
            _routeDbQueries = routeDbQueries ?? throw new ArgumentNullException(nameof(routeDbQueries));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<RouteDto>> GetRoutesAsync()
        {
            _logger.LogInformation("Getting all routes");
            var routes = await _routeDbQueries.GetAllAsync();
            return _mapper.Map<IEnumerable<RouteDto>>(routes);
        }

        public async Task<StatusCodeResponseDto<RouteDto?>> GetRouteByIdAsync(Guid routeId)
        {
            _logger.LogInformation("Getting route with ID: {RouteId}", routeId);

            var route = await _routeDbQueries.GetByIdAsync(routeId);
            if (route == null)
            {
                _logger.LogWarning("Route with ID: {RouteId} not found", routeId);
                return CreateStatusResponseDto<RouteDto?>(StatusCodeResponse.NotFound, $"Route with ID: {routeId} not found", null);
            }
            return CreateStatusResponseDto<RouteDto?>(StatusCodeResponse.Success, null, _mapper.Map<RouteDto>(route));
        }

        public async Task<StatusCodeResponseDto<string?>> CreateRouteAsync(CreateRouteDto createRouteDto, Guid userId)
        {
            _logger.LogInformation("Creating new route.");

            try
            {
                // Pass userId to AutoMapper via context items
                var routeEntity = _mapper.Map<Models.Route>(createRouteDto, opt => opt.Items["UserId"] = userId);

                var createdRoute = await _routeDbQueries.CreateAsync(routeEntity);
                _logger.LogInformation("Route created successfully with ID: {RouteId}", createdRoute.RouteId);

                return CreateStatusResponseDto<string?>(StatusCodeResponse.Success, $"Route created successfully with ID: {createdRoute.RouteId}", createdRoute.RouteId.ToString());
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "Error adding route due to null argument.");
                return CreateStatusResponseDto<string?>(StatusCodeResponse.BadRequest, "Error adding route due to null argument.", null);
            }
            catch (AutoMapperMappingException ex)
            {
                _logger.LogError(ex, "Error adding route due to mapping issue: {ErrorMessage}", ex.Message);
                return CreateStatusResponseDto<string?>(StatusCodeResponse.InternalServerError, $"An error occurred during data mapping: {ex.Message}", null);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error adding route due to JSON processing issue.");
                return CreateStatusResponseDto<string?>(StatusCodeResponse.BadRequest, "Error adding route due to JSON processing issue.", null);
            }
        }

        public async Task<StatusCodeResponseDto<RouteDto?>> UpdateRouteAsync(Guid routeId, UpdateRouteDto updateRouteDto)
        {
            _logger.LogInformation("Updating route with ID: {routeId}", routeId);

            var existingRoute = await _routeDbQueries.GetByIdAsync(routeId);
            if (existingRoute == null)
            {
                _logger.LogWarning("Route with ID: {routeId} not found for update", routeId);
                return CreateStatusResponseDto<RouteDto?>(StatusCodeResponse.NotFound, $"Route with ID: {routeId} not found for update", null);
            }

            // Map the changes from DTO to the existing entity
            _mapper.Map(updateRouteDto, existingRoute);

            existingRoute.UpdatedAt = DateTime.UtcNow;

            var updatedRoute = await _routeDbQueries.UpdateRouteAsync(existingRoute);
            _logger.LogInformation("Route with ID: {routeId} updated successfully", routeId);
            return CreateStatusResponseDto<RouteDto?>(StatusCodeResponse.Success, $"Route with ID: {routeId} updated successfully", _mapper.Map<RouteDto>(updatedRoute));
        }

        public async Task<StatusCodeResponseDto<bool>> DeleteRouteAsync(Guid routeId)
        {
            _logger.LogInformation("Deleting route with ID: {routeId}", routeId);

            bool result = await _routeDbQueries.DeleteRouteAsync(routeId);
            if (result)
            {
                _logger.LogInformation("Route with ID: {routeId} deleted successfully", routeId);
                return CreateStatusResponseDto<bool>(StatusCodeResponse.Success, $"Route with ID: {routeId} deleted successfully", true);
            }
            else
            {
                _logger.LogWarning("Route with ID: {routeId} not found for deletion", routeId);
                return CreateStatusResponseDto<bool>(StatusCodeResponse.NotFound, $"Route with ID: {routeId} not found for deletion", false);
            }
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
