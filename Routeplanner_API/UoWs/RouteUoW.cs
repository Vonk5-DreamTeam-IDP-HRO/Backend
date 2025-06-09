using AutoMapper;
using Routeplanner_API.Database_Queries;
using Routeplanner_API.DTO;
using Routeplanner_API.DTO.Route;
using Routeplanner_API.Enums;
using System.Text.Json;

namespace Routeplanner_API.UoWs
{
    /// <summary>
    /// Unit of Work for route-related operations.
    /// Handles CRUD and mapping for routes.
    /// </summary>
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

        /// <summary>
        /// Retrieves all routes asynchronously.
        /// </summary>
        /// <returns>A collection of route DTOs.</returns>
        public async Task<IEnumerable<RouteDto>> GetRoutesAsync()
        {
            _logger.LogInformation("Getting all routes");
            var routes = await _routeDbQueries.GetAllAsync();
            return _mapper.Map<IEnumerable<RouteDto>>(routes);
        }

        /// <summary>
        /// Retrieves a route by its ID asynchronously.
        /// </summary>
        /// <param name="routeId">The route's unique identifier.</param>
        /// <returns>Status response DTO containing the route DTO or null if not found.</returns>
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

        /// <summary>
        /// Creates a new route asynchronously.
        /// </summary>
        /// <param name="createRouteDto">Data transfer object for route creation.</param>
        /// <param name="userId">ID of the user creating the route.</param>
        /// <returns>Status response DTO containing the created route ID or error message.</returns>
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

        /// <summary>
        /// Updates an existing route asynchronously.
        /// </summary>
        /// <param name="routeId">The ID of the route to update.</param>
        /// <param name="updateRouteDto">Data transfer object with updated route information.</param>
        /// <returns>Status response DTO containing the updated route DTO or error message.</returns>
        public async Task<StatusCodeResponseDto<RouteDto?>> UpdateRouteAsync(Guid routeId, UpdateRouteDto updateRouteDto)
        {
            _logger.LogInformation("Updating route with ID: {routeId}", routeId);

            var existingRoute = await _routeDbQueries.GetByIdAsync(routeId);
            if (existingRoute == null)
            {
                _logger.LogWarning("Route with ID: {routeId} not found for update", routeId);
                return CreateStatusResponseDto<RouteDto?>(StatusCodeResponse.NotFound, $"Route with ID: {routeId} not found for update", null);
            }

            _mapper.Map(updateRouteDto, existingRoute);

            existingRoute.UpdatedAt = DateTime.UtcNow;

            var updatedRoute = await _routeDbQueries.UpdateRouteAsync(existingRoute);
            _logger.LogInformation("Route with ID: {routeId} updated successfully", routeId);
            return CreateStatusResponseDto<RouteDto?>(StatusCodeResponse.Success, $"Route with ID: {routeId} updated successfully", _mapper.Map<RouteDto>(updatedRoute));
        }

        /// <summary>
        /// Deletes a route asynchronously.
        /// </summary>
        /// <param name="routeId">The ID of the route to delete.</param>
        /// <returns>Status response DTO indicating success or failure of the deletion.</returns>
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

        /// <summary>
        /// Creates a standard status response DTO.
        /// </summary>
        /// <typeparam name="T">Type of the data payload.</typeparam>
        /// <param name="statusCodeResponse">Status code of the response.</param>
        /// <param name="message">Optional message.</param>
        /// <param name="data">Data payload.</param>
        /// <returns>A new <see cref="StatusCodeResponseDto{T}"/> instance.</returns>
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
