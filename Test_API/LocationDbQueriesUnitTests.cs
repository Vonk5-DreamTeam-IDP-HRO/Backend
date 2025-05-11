using Moq;
using Xunit;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Routeplanner_API.DTO;
using Routeplanner_API.Models;
using Routeplanner_API.UoWs;
using Routeplanner_API.Database_Queries;

namespace Test_API.Tests
{
    public class LocationDbQueriesUnitTests
    {
        [Fact]
        public async Task GetByIdAsync_ShouldReturnLocation_WhenLocationExists()
        {
            // Arrange
            var existingLocation = new Location
            {
                LocationId = 1,
                Name = "Test Location",
                Latitude = 10.0,
                Longitude = 20.0,
                Description = ""
            };

            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILogger<LocationUoW>>();
            var mockLocationDbQueries = new Mock<ILocationDbQueries>();

            mockMapper
                .Setup(m => m.Map<LocationDto>(It.IsAny<Location>()))
                .Returns(new LocationDto
                {
                    LocationId = existingLocation.LocationId,
                    Name = existingLocation.Name,
                    Latitude = existingLocation.Latitude,
                    Longitude = existingLocation.Longitude
                });

            mockLocationDbQueries
                .Setup(m => m.GetByIdAsync(existingLocation.LocationId))
                .ReturnsAsync(existingLocation);

            var locationUoW = new LocationUoW(mockLocationDbQueries.Object, mockMapper.Object, mockLogger.Object);

            // Act
            var result = await locationUoW.GetLocationByIdAsync(existingLocation.LocationId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(existingLocation.LocationId, result.LocationId);
            Assert.Equal(existingLocation.Name, result.Name);
            Assert.Equal(existingLocation.Latitude, result.Latitude);
            Assert.Equal(existingLocation.Longitude, result.Longitude);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenLocationDoesNotExist()
        {
            // Arrange
            const int nonExistingLocationId = -100;

            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILogger<LocationUoW>>();
            var mockLocationDbQueries = new Mock<ILocationDbQueries>();

            mockLocationDbQueries
                .Setup(m => m.GetByIdAsync(nonExistingLocationId))
                .ReturnsAsync((Location)null);

            var locationUoW = new LocationUoW(mockLocationDbQueries.Object, mockMapper.Object, mockLogger.Object);

            // Act
            var result = await locationUoW.GetLocationByIdAsync(nonExistingLocationId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsyncLocation_ShouldReturnPositive_WhenLocationIsSuccessfullyCreated()
        {
            // Arrange
            var newLocationDto = new CreateLocationDto
            {
                Name = "New Location",
                Latitude = 10.0,
                Longitude = 20.0,
                Description = "Test description"
            };

            var createdLocation = new Location
            {
                LocationId = 1,
                Name = newLocationDto.Name,
                Latitude = newLocationDto.Latitude,
                Longitude = newLocationDto.Longitude,
                Description = newLocationDto.Description
            };

            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILogger<LocationUoW>>();
            var mockLocationDbQueries = new Mock<ILocationDbQueries>();

            mockMapper
                .Setup(m => m.Map<Location>(It.IsAny<CreateLocationDto>()))
                .Returns(createdLocation);

            mockMapper
                .Setup(m => m.Map<LocationDto>(It.IsAny<Location>()))
                .Returns(new LocationDto
                {
                    LocationId = createdLocation.LocationId,
                    Name = createdLocation.Name,
                    Latitude = createdLocation.Latitude,
                    Longitude = createdLocation.Longitude,
                    Description = createdLocation.Description
                });

            mockLocationDbQueries
                .Setup(m => m.CreateAsync(It.IsAny<Location>()))
                .ReturnsAsync(createdLocation);

            var locationUoW = new LocationUoW(mockLocationDbQueries.Object, mockMapper.Object, mockLogger.Object);

            // Act
            var result = await locationUoW.CreateLocationAsync(newLocationDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newLocationDto.Name, result.Name);
            Assert.Equal(newLocationDto.Latitude, result.Latitude);
            Assert.Equal(newLocationDto.Longitude, result.Longitude);
        }
    }
}
