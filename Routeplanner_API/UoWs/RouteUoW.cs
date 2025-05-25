using System.Text.Json;
using AutoMapper;
using Routeplanner_API.Database_Queries;
using Routeplanner_API.DTO.Route;
using Routeplanner_API.Models;

namespace Routeplanner_API.UoWs
{
    public class RouteUoW
    {
        private readonly IRouteDbQueries _routeDbQueries;
        private readonly ILogger<RouteUoW> _logger;
        private readonly IMapper _mapper;
        // private IConfiguration configuration; // Removed as it's part of an unused constructor

        public RouteUoW(IRouteDbQueries routeDbQueries, ILogger<RouteUoW> logger, IMapper mapper)
        {
            _routeDbQueries = routeDbQueries ?? throw new ArgumentNullException(nameof(routeDbQueries));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        // Removed unused constructor:
        // public RouteUoW(IConfiguration configuration)
        // {
        //     this.configuration = configuration;
        // }

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

        public async Task<RouteDto> CreateRouteAsync(CreateRouteDto createRouteDto)
        {
            _logger.LogInformation("Creating new route from DTO");
            try
            {
                if (createRouteDto == null)
                {
                    throw new ArgumentNullException(nameof(createRouteDto));
                }
                var routeEntity = _mapper.Map<Routeplanner_API.Models.Route>(createRouteDto);
                
                // Potentially set UserId if applicable and not directly from DTO, e.g. from authenticated user
                // routeEntity.UserId = ...; 
                // routeEntity.CreatedAt = DateTime.UtcNow; // Handled by DB or EF Core config
                // routeEntity.UpdatedAt = DateTime.UtcNow; // Handled by DB or EF Core config

                var createdRoute = await _routeDbQueries.CreateAsync(routeEntity);
                _logger.LogInformation("Route created successfully with ID: {RouteId}", createdRoute.RouteId); // Assuming RouteId exists
                return _mapper.Map<RouteDto>(createdRoute);
            }
            catch (AutoMapperMappingException ex)
            {
                _logger.LogError(ex, "Error mapping CreateRouteDto to Route entity: {ErrorMessage}", ex.Message);
                // Consider re-throwing a more specific application exception or handling appropriately
                throw; 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating route: {ErrorMessage}", ex.Message);
                throw;
            }
        }

        public async Task<RouteDto?> UpdateRouteAsync(RouteDto routeDto) // Should ideally be UpdateRouteDto
        {
            _logger.LogInformation("Updating route with ID: {RouteId}", routeDto.RouteId);
            // Consider using a specific UpdateRouteDto and mapping from that
            var routeEntity = _mapper.Map<Routeplanner_API.Models.Route>(routeDto); 
            var updatedRoute = await _routeDbQueries.UpdateAsync(routeEntity);
            if (updatedRoute == null)
            {
                _logger.LogWarning("Route with ID: {RouteId} not found for update", routeDto.RouteId);
                return null;
            }
            return _mapper.Map<RouteDto>(updatedRoute);
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
