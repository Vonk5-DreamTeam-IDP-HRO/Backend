using System.Text.Json;
using AutoMapper;
using Routeplanner_API.Database_Queries;
using Routeplanner_API.DTO.Route;
using Routeplanner_API.DTO.User;
using Routeplanner_API.Models;

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

        public async Task<RouteDto?> GetRouteByIdAsync(Guid routeId)
        {
            _logger.LogInformation("Getting route with ID: {RouteId}", routeId);
            var route = await _routeDbQueries.GetByIdAsync(routeId);
            if (route == null)
            {
                _logger.LogWarning("Route with ID: {RouteId} not found", routeId);
                return null;
            }
            return _mapper.Map<RouteDto>(route);
        }

        public async Task<RouteDto> CreateRouteAsync(CreateRouteDto createRouteDto, Guid userId)
        {
            _logger.LogInformation("Creating new route from DTO for UserId: {UserId}", userId);
            try
            {
                if (createRouteDto == null)
                {
                    throw new ArgumentNullException(nameof(createRouteDto));
                }
                // Pass userId to AutoMapper via context items
                var routeEntity = _mapper.Map<Routeplanner_API.Models.Route>(createRouteDto, opt => opt.Items["UserId"] = userId);

                var createdRoute = await _routeDbQueries.CreateAsync(routeEntity);
                _logger.LogInformation("Route created successfully with ID: {RouteId}", createdRoute.RouteId);
                return _mapper.Map<RouteDto>(createdRoute);
            }
            catch (AutoMapperMappingException ex)
            {
                _logger.LogError(ex, "Error mapping CreateRouteDto to Route entity: {ErrorMessage}", ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating route: {ErrorMessage}", ex.Message);
                throw;
            }
        }

        public async Task<RouteDto?> UpdateRouteAsync(Guid routeId, UpdateRouteDto updateRouteDto)
        {
            _logger.LogInformation("Updating route with ID: {routeId}", routeId);

            var existingRoute = await _routeDbQueries.GetByIdAsync(routeId);
            if (existingRoute == null)
            {
                _logger.LogWarning("Route with ID: {routeId} not found for update", routeId);
                return null;
            }

            try
            {
                // Map the changes from DTO to the existing entity
                _mapper.Map(updateRouteDto, existingRoute);

                existingRoute.UpdatedAt = DateTime.UtcNow;

                var updatedRoute = await _routeDbQueries.UpdateRouteAsync(existingRoute);
                _logger.LogInformation("Route with ID: {routeId} updated successfully", routeId);
                return _mapper.Map<RouteDto>(updatedRoute);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating route with ID: {routeId}: {ErrorMessage}", routeId, ex.Message);
                throw;
            }
        }

        public async Task<bool> DeleteRouteAsync(Guid routeId)
        {
            _logger.LogInformation("Deleting route with ID: {routeId}", routeId);
            try
            {
                var result = await _routeDbQueries.DeleteRouteAsync(routeId);
                if (result)
                {
                    _logger.LogInformation("Route with ID: {routeId} deleted successfully", routeId);
                }
                else
                {
                    _logger.LogWarning("Route with ID: {routeId} not found for deletion", routeId);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting route with ID: {routeId}: {ErrorMessage}", routeId, ex.Message);
                throw;
            }
        }
    }
}
