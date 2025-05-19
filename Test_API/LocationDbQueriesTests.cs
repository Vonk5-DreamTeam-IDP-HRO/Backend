using Microsoft.EntityFrameworkCore;
using Routeplanner_API.Models;
using Routeplanner_API.UoWs;
using Moq;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Routeplanner_API.Database_Queries;
using Routeplanner_API.DTO.Location;

namespace Test_API.Tests
{
    public class LocationDbQueriesTests
    {
        private DbContextOptions<RouteplannerDbContext> GetInMemoryDbContextOptions(string dbName)
        {
            return new DbContextOptionsBuilder<RouteplannerDbContext>()
                .UseInMemoryDatabase(
                    databaseName: dbName) // This makes sure that every test gets a fresh filled Database
                .Options;
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnLocation_WhenLocationExist()
        {
            // Arrange
            const string dnName = nameof(GetByIdAsync_ShouldReturnLocation_WhenLocationExist);
            var options = GetInMemoryDbContextOptions(dnName);
            var  newLocationId = Guid.NewGuid();

            var existingLocation = new Location
                { LocationId = newLocationId, Name = "EuromastTest", Latitude = 51.903663052, Longitude = 4.459498162, Description = "" };

            await using (var context = new RouteplannerDbContext(options))
            {
                context.Locations.Add(existingLocation);
                await context.SaveChangesAsync();
            }

            //create mock dependencies
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

            // Create the service with the mock DbQueries
            var locationUoW =
                new LocationUoW(mockLocationDbQueries.Object, mockMapper.Object, mockLogger.Object);


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
            const string dnName = nameof(GetByIdAsync_ShouldReturnNull_WhenLocationDoesNotExist);
            var options = GetInMemoryDbContextOptions(dnName);
            // TODO: Needs to be changed if we use UUID
            var nonExistingLocationId = new Guid();

            //create mock dependencies
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILogger<LocationUoW>>();

            await using var context = new RouteplannerDbContext(options);

            // Create a repository using the context
            var locationDbQueries = new LocationDbQueries(context);
            var locationUoW = new LocationUoW(locationDbQueries, mockMapper.Object, mockLogger.Object);

            // Act
            var result = await locationUoW.GetLocationByIdAsync(nonExistingLocationId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsyncLocation_ShouldReturnPositive_WhenLocationIsSuccesfullCreated()
        {
            // Arrange
            const string dnName = nameof(CreateAsyncLocation_ShouldReturnPositive_WhenLocationIsSuccesfullCreated);
            var options = GetInMemoryDbContextOptions(dnName);
            var newLocation = new CreateLocationDto
            {
                Name = "EuromastTest",
                Latitude = 51.903663052,
                Longitude = 4.459498162,
                Description = "Test description for euromast",
                UserId = Guid.NewGuid()
            };

            var mappedLocation = new Location
            {
                // will be generated automatically by the database because of the AutoMapper profile
                // LocationId = Guid.NewGuid(),
                Name = newLocation.Name,
                Latitude = newLocation.Latitude,
                Longitude = newLocation.Longitude,
                Description = newLocation.Description
            };

            var returnedLocationDto = new LocationDto
            {
                LocationId = Guid.NewGuid(),
                Name = newLocation.Name,
                Latitude = newLocation.Latitude,
                Longitude = newLocation.Longitude,
                Description = newLocation.Description
            };

            //create mock dependencies
            var mockMapper = new Mock<IMapper>();
            var mockLogger = new Mock<ILogger<LocationUoW>>();

            // Setup the mapper to return the mapped location
            mockMapper
                .Setup(m => m.Map<Location>(It.IsAny<CreateLocationDto>()))
                .Returns(mappedLocation);

            // Setupthe mapper to return LocationDTo when mapping from Location
            mockMapper
                .Setup(m => m.Map<LocationDto>(It.IsAny<Location>()))
                .Returns(returnedLocationDto);

            await using var context = new RouteplannerDbContext(options);

            // Create a repository using the context
            var locationDbQueries = new LocationDbQueries(context);
            var locationUoW = new LocationUoW(locationDbQueries, mockMapper.Object, mockLogger.Object);

            // Act
            var result = await locationUoW.CreateLocationAsync(newLocation);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newLocation.Name, result.Name);
            Assert.Equal(newLocation.Latitude, result.Latitude);
            Assert.Equal(newLocation.Longitude, result.Longitude);
        }
    }

    /*LocationDTO:

    public int LocationId { get; set; }
    public string Name { get; set; } = null!;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Description { get; set; }*/
}