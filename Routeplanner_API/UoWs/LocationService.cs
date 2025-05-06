using AutoMapper;
using Routeplanner_API.Data.Repositories;
using Routeplanner_API.DTO;
using Routeplanner_API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Routeplanner_API.UoWs
{
    public class LocationService
    {
        private readonly ILocationRepository _locationRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<LocationService> _logger;

        public LocationService(ILocationRepository locationRepository, IMapper mapper, ILogger<LocationService> logger)
        {
            _locationRepository = locationRepository ?? throw new ArgumentNullException(nameof(locationRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<LocationDto>> GetLocationsAsync()
        {
            _logger.LogInformation("Getting all locations");
            var locations = await _locationRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<LocationDto>>(locations);
        }

        public async Task<LocationDto?> GetLocationByIdAsync(int locationId)
        {
            _logger.LogInformation("Getting location with ID: {LocationId}", locationId);
            var location = await _locationRepository.GetByIdAsync(locationId);
            if (location == null)
            {
                _logger.LogWarning("Location with ID: {LocationId} not found", locationId);
                return null;
            }
            return _mapper.Map<LocationDto>(location);
        }

        public async Task<LocationDto> CreateLocationAsync(CreateLocationDto createLocationDto)
        {
            _logger.LogInformation("Creating new location");

            try
            {
                var locationEntity = _mapper.Map<Location>(createLocationDto);

                // Potentially set UserId if applicable and not directly from DTO
                // locationEntity.UserId = ...; 

                var createdLocation = await _locationRepository.CreateAsync(locationEntity);
                _logger.LogInformation("Location created successfully with ID: {LocationId}", createdLocation.LocationId);
                return _mapper.Map<LocationDto>(createdLocation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating location: {ErrorMessage}", ex.Message);
                throw;
            }
        }

        public async Task<LocationDto?> UpdateLocationAsync(int locationId, UpdateLocationDto updateLocationDto)
        {
            _logger.LogInformation("Updating location with ID: {LocationId}", locationId);

            var existingLocation = await _locationRepository.GetByIdAsync(locationId);
            if (existingLocation == null)
            {
                _logger.LogWarning("Location with ID: {LocationId} not found for update", locationId);
                return null;
            }

            try
            {
                // Map the changes from DTO to the existing entity
                _mapper.Map(updateLocationDto, existingLocation);

                // Ensure UpdatedAt is set (AutoMapper profile also does this, but explicit here is fine too)
                // existingLocation.UpdatedAt = DateTime.UtcNow;

                var updatedLocation = await _locationRepository.UpdateAsync(existingLocation);
                _logger.LogInformation("Location with ID: {LocationId} updated successfully", locationId);
                return _mapper.Map<LocationDto>(updatedLocation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating location with ID: {LocationId}: {ErrorMessage}", locationId, ex.Message);
                throw;
            }
        }

        public async Task<bool> DeleteLocationAsync(int locationId)
        {
            _logger.LogInformation("Deleting location with ID: {LocationId}", locationId);
            try
            {
                var result = await _locationRepository.DeleteAsync(locationId);
                if (result)
                {
                    _logger.LogInformation("Location with ID: {LocationId} deleted successfully", locationId);
                }
                else
                {
                    _logger.LogWarning("Location with ID: {LocationId} not found for deletion", locationId);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting location with ID: {LocationId}: {ErrorMessage}", locationId, ex.Message);
                throw;
            }
        }
    }
}
