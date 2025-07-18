﻿using AutoMapper;
using k8s.KubeConfigModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Routeplanner_API.Database_Queries;
using Routeplanner_API.DTO;
using Routeplanner_API.DTO.Location;
using Routeplanner_API.DTO.User;
using Routeplanner_API.Enums;
using Routeplanner_API.Helpers;
using Routeplanner_API.Models;
using Routeplanner_API.UoWs;
using User = Routeplanner_API.Models.User;

namespace Test_API.UoWs_Tests
{
    public class LocationUoWTests
    {
        private readonly Mock<RouteplannerDbContext> _contextMock = new Mock<RouteplannerDbContext>();
        private readonly Mock<ILocationDbQueries> _locationDbQueriesMock = new Mock<ILocationDbQueries>();
        private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();
        private readonly Mock<ILogger<LocationUoW>> _loggerMock = new Mock<ILogger<LocationUoW>>();

        private readonly LocationUoW _locationUoW;

        public LocationUoWTests()
        {
            _locationUoW = new LocationUoW(_contextMock.Object, _locationDbQueriesMock.Object, _mapperMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task GetLocationsAsync_Returns_MappedLocations()
        {
            LocationUoW unitOfWork = new LocationUoW(_contextMock.Object, _locationDbQueriesMock.Object,
                _mapperMock.Object, _loggerMock.Object);

            List<Location> locationsFromDb = new List<Location> { new Location(), new Location() };
            List<LocationDto> mappedDtos = new List<LocationDto> { new LocationDto(), new LocationDto() };

            _locationDbQueriesMock
                .Setup(q => q.GetAllAsync())
                .ReturnsAsync(locationsFromDb);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<LocationDto>>(locationsFromDb))
                .Returns(mappedDtos);

            IEnumerable<LocationDto> result = await unitOfWork.GetLocationsAsync();

            _locationDbQueriesMock.Verify(q => q.GetAllAsync(), Times.Once);
            _mapperMock.Verify(m => m.Map<IEnumerable<LocationDto>>(locationsFromDb), Times.Once);

            Assert.Equal(mappedDtos, result);
        }

        [Fact]
        public async Task GetUniqueCategoriesAsync_ReturnsSuccess_WithFilteredCategories()
        {
            LocationUoW unitOfWork = new LocationUoW(_contextMock.Object, _locationDbQueriesMock.Object,
                _mapperMock.Object, _loggerMock.Object);

            List<string?> categoriesFromDb = new List<string?> { "Park", null, "", "Museum", " " };

            _locationDbQueriesMock
                .Setup(q => q.GetUniqueCategoriesAsync())
                .ReturnsAsync(categoriesFromDb);

            StatusCodeResponseDto<IEnumerable<string?>> result = await unitOfWork.GetUniqueCategoriesAsync();

            Assert.Equal(StatusCodeResponse.Success, result.StatusCodeResponse);
            Assert.Equal("Categories found.", result.Message);

            IEnumerable<string?> expected = categoriesFromDb.Where(c => !string.IsNullOrEmpty(c));
            Assert.Equal(expected, result.Data);
        }

        [Fact]
        public async Task GetUniqueCategoriesAsync_ReturnsNotFound_WhenCategoriesNull()
        {
            LocationUoW unitOfWork = new LocationUoW(_contextMock.Object, _locationDbQueriesMock.Object,
                _mapperMock.Object, _loggerMock.Object);

            _locationDbQueriesMock
                .Setup(q => q.GetUniqueCategoriesAsync())
                .ReturnsAsync((IEnumerable<string?>)null);

            StatusCodeResponseDto<IEnumerable<string?>> result = await unitOfWork.GetUniqueCategoriesAsync();

            Assert.Equal(StatusCodeResponse.NotFound, result.StatusCodeResponse);
            Assert.Equal("No location categories found.", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task GetLocationByIdAsync_ReturnsSuccess_WithMappedLocation()
        {
            Guid locationId = Guid.NewGuid();
            LocationUoW unitOfWork = new LocationUoW(_contextMock.Object, _locationDbQueriesMock.Object,
                _mapperMock.Object, _loggerMock.Object);

            Location locationEntity = new Location();
            LocationDto locationDto = new LocationDto();

            _locationDbQueriesMock
                .Setup(q => q.GetByIdAsync(locationId))
                .ReturnsAsync(locationEntity);

            _mapperMock
                .Setup(m => m.Map<LocationDto?>(locationEntity))
                .Returns(locationDto);

            StatusCodeResponseDto<LocationDto?> result = await unitOfWork.GetLocationByIdAsync(locationId);

            Assert.Equal(StatusCodeResponse.Success, result.StatusCodeResponse);
            Assert.Equal($"Location with ID: {locationId} found.", result.Message);
            Assert.Equal(locationDto, result.Data);
        }

        [Fact]
        public async Task GetLocationByIdAsync_ReturnsNotFound_WhenLocationIsNull()
        {
            Guid locationId = Guid.NewGuid();
            LocationUoW unitOfWork = new LocationUoW(_contextMock.Object, _locationDbQueriesMock.Object,
                _mapperMock.Object, _loggerMock.Object);

            _locationDbQueriesMock
                .Setup(q => q.GetByIdAsync(locationId))
                .ReturnsAsync((Location?)null);

            StatusCodeResponseDto<LocationDto?> result = await unitOfWork.GetLocationByIdAsync(locationId);

            Assert.Equal(StatusCodeResponse.NotFound, result.StatusCodeResponse);
            Assert.Equal($"Location with ID: {locationId} not found", result.Message);
            Assert.Null(result.Data);
        }

        // To do: Create, Update
        [Fact]
        public async Task CreateLocationAsync_ReturnsSuccess_WhenCreatingSucceeds()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var locationId = Guid.NewGuid();

            // Setup mock user
            var user = new User
            {
                Id = userId,
                UserName = "testuser",
                Email = "test@example.com",
                PasswordHash = "hashedpassword"
            };

            // Mock DbSet for Users
            var mockDbSet = new Mock<DbSet<User>>();
            _contextMock.Setup(c => c.Users).Returns(mockDbSet.Object);
            _contextMock.Setup(c => c.Users.FindAsync(userId)).ReturnsAsync(user);

            // Create the DTOs
            var createLocationDetailDto = new CreateLocationDetailDto
            {
                Address = "123 Test St",
                City = "Test City",
                Country = "Test Country",
                ZipCode = "12345",
                PhoneNumber = "123-456-7890",
                Website = "http://test.com",
                Category = "Park",
                Accessibility = "Wheelchair accessible"
            };

            var createLocationDto = new CreateLocationDto
            {
                Name = "Test Location",
                Latitude = 51.9225,
                Longitude = 4.47917,
                Description = "A test location",
                LocationDetail = createLocationDetailDto
            };

            // Mock the mapped entity and created entity
            var locationEntity = new Location
            {
                LocationId = locationId,
                Name = createLocationDto.Name,
                Latitude = createLocationDto.Latitude,
                Longitude = createLocationDto.Longitude,
                Description = createLocationDto.Description,
                UserId = userId
            };

            var locationDto = new LocationDto
            {
                LocationId = locationId,
                Name = createLocationDto.Name,
                Latitude = createLocationDto.Latitude,
                Longitude = createLocationDto.Longitude,
                Description = createLocationDto.Description
            };

            // Setup mapping
            _mapperMock.Setup(m => m.Map<Location>(
                It.IsAny<CreateLocationDto>(),
                It.IsAny<Action<IMappingOperationOptions<object, Location>>>())
            ).Returns(locationEntity);

            _mapperMock.Setup(m => m.Map<LocationDto>(locationEntity))
                .Returns(locationDto);

            // Setup location creation
            _locationDbQueriesMock.Setup(q => q.CreateAsync(It.IsAny<Location>()))
                .ReturnsAsync(locationEntity);

            // Act
            var result = await _locationUoW.CreateLocationAsync(createLocationDto, userId);

            // Assert
            Assert.Equal(StatusCodeResponse.Success, result.StatusCodeResponse);
            Assert.Contains($"Location created successfully with ID: {locationId}", result.Message);
            Assert.Equal(locationDto, result.Data);

            // Verify method calls
            _contextMock.Verify(c => c.Users.FindAsync(userId), Times.Once);
            _locationDbQueriesMock.Verify(q => q.CreateAsync(It.IsAny<Location>()), Times.Once);
            _mapperMock.Verify(m => m.Map<LocationDto>(It.IsAny<Location>()), Times.Once);
        }

        [Fact]
        public async Task CreateLocationAsync_ReturnsUnauthorized_WhenUserNotFound()
        {
            var userId = Guid.NewGuid();

            // Setup Users DbSet mock to return null (user not found)
            _contextMock.Setup(c => c.Users.FindAsync(userId)).ReturnsAsync((User)null);

            var createLocationDto = new CreateLocationDto
            {
                Name = "Test Location",
                Latitude = 51.9225,
                Longitude = 4.47917,
                Description = "A test location"
            };

            var result = await _locationUoW.CreateLocationAsync(createLocationDto, userId);

            Assert.Equal(StatusCodeResponse.Unauthorized, result.StatusCodeResponse);
            Assert.Equal("User does not exist in the database.", result.Message);
            Assert.Null(result.Data);

            _contextMock.Verify(c => c.Users.FindAsync(userId), Times.Once);
            _locationDbQueriesMock.Verify(q => q.CreateAsync(It.IsAny<Location>()), Times.Never);
        }

        [Fact]
        public async Task CreateLocationAsync_ReturnsBadRequest_WhenArgumentExceptionThrown()
        {
            var userId = Guid.NewGuid();

            var user = new User
            {
                Id = userId,
                UserName = "testuser",
                Email = "test@example.com",
                PasswordHash = "hashedpassword"
            };

            _contextMock.Setup(c => c.Users.FindAsync(userId)).ReturnsAsync(user);

            var createLocationDto = new CreateLocationDto
            {
                Name = "Test Location",
                Latitude = 51.9225,
                Longitude = 4.47917,
                Description = "A test location"
            };

            var locationEntity = new Location();

            _mapperMock.Setup(m => m.Map<Location>(
                It.IsAny<CreateLocationDto>(),
                It.IsAny<Action<IMappingOperationOptions<object, Location>>>())
            ).Returns(locationEntity);

            _locationDbQueriesMock.Setup(q => q.CreateAsync(It.IsAny<Location>()))
                .ThrowsAsync(new ArgumentException("Invalid argument"));

            var result = await _locationUoW.CreateLocationAsync(createLocationDto, userId);

            Assert.Equal(StatusCodeResponse.BadRequest, result.StatusCodeResponse);
            Assert.Contains("Invalid argument when creating location", result.Message);
            Assert.Null(result.Data);

            _contextMock.Verify(c => c.Users.FindAsync(userId), Times.Once);
            _locationDbQueriesMock.Verify(q => q.CreateAsync(It.IsAny<Location>()), Times.Once);
            _mapperMock.Verify(m => m.Map<LocationDto>(It.IsAny<Location>()), Times.Never);
        }

        [Fact]
        public async Task CreateLocationAsync_ReturnsInternalServerError_WhenDbUpdateExceptionThrown()
        {
            var userId = Guid.NewGuid();

            var user = new User
            {
                Id = userId,
                UserName = "testuser",
                Email = "test@example.com",
                PasswordHash = "hashedpassword"
            };

            _contextMock.Setup(c => c.Users.FindAsync(userId)).ReturnsAsync(user);

            var createLocationDto = new CreateLocationDto
            {
                Name = "Test Location",
                Latitude = 51.9225,
                Longitude = 4.47917,
                Description = "A test location"
            };

            var locationEntity = new Location();

            _mapperMock.Setup(m => m.Map<Location>(
                It.IsAny<CreateLocationDto>(),
                It.IsAny<Action<IMappingOperationOptions<object, Location>>>())
            ).Returns(locationEntity);

            _locationDbQueriesMock.Setup(q => q.CreateAsync(It.IsAny<Location>()))
                .ThrowsAsync(new DbUpdateException("Database error", new Exception()));

            var result = await _locationUoW.CreateLocationAsync(createLocationDto, userId);

            Assert.Equal(StatusCodeResponse.InternalServerError, result.StatusCodeResponse);
            Assert.Equal("An unexpected database error occurred.", result.Message);
            Assert.Null(result.Data);

            _contextMock.Verify(c => c.Users.FindAsync(userId), Times.Once);
            _locationDbQueriesMock.Verify(q => q.CreateAsync(It.IsAny<Location>()), Times.Once);
            _mapperMock.Verify(m => m.Map<LocationDto>(It.IsAny<Location>()), Times.Never);
        }

        [Fact]
        public async Task UpdateLocationAsync_ReturnsSuccess_WhenUpdateSucceeds()
        {
            var locationId = Guid.NewGuid();
            var updateDto = new UpdateLocationDto
            {
                LocationId = locationId,
                Name = "Updated Name",
                Latitude = 51.9,
                Longitude = 4.5,
                Description = "Updated description"
            };

            var existingLocation = new Location
            {
                LocationId = locationId,
                Name = "Old Name",
                Latitude = 51.8,
                Longitude = 4.4,
                Description = "Old description"
            };

            var updatedLocation = new Location
            {
                LocationId = locationId,
                Name = updateDto.Name,
                Latitude = updateDto.Latitude.Value,
                Longitude = updateDto.Longitude.Value,
                Description = updateDto.Description
            };

            var locationDto = new LocationDto
            {
                LocationId = locationId,
                Name = updateDto.Name,
                Latitude = updateDto.Latitude.Value,
                Longitude = updateDto.Longitude.Value,
                Description = updateDto.Description
            };

            _locationDbQueriesMock.Setup(q => q.GetByIdAsync(locationId)).ReturnsAsync(existingLocation);
            _mapperMock.Setup(m => m.Map(updateDto, existingLocation));
            _locationDbQueriesMock.Setup(q => q.UpdateAsync(existingLocation)).ReturnsAsync(updatedLocation);
            _mapperMock.Setup(m => m.Map<LocationDto>(updatedLocation)).Returns(locationDto);

            var result = await _locationUoW.UpdateLocationAsync(locationId, updateDto);

            Assert.Equal(StatusCodeResponse.Success, result.StatusCodeResponse);
            Assert.Equal($"Location with ID: {locationId} updated successfully", result.Message);
            Assert.Equal(locationDto, result.Data);

            _locationDbQueriesMock.Verify(q => q.GetByIdAsync(locationId), Times.Once);
            _locationDbQueriesMock.Verify(q => q.UpdateAsync(existingLocation), Times.Once);
            _mapperMock.Verify(m => m.Map(updateDto, existingLocation), Times.Once);
            _mapperMock.Verify(m => m.Map<LocationDto>(updatedLocation), Times.Once);
        }

        [Fact]
        public async Task UpdateLocationAsync_ReturnsNotFound_WhenLocationDoesNotExist()
        {
            var locationId = Guid.NewGuid();
            var updateDto = new UpdateLocationDto
            {
                LocationId = locationId,
                Name = "Updated Name",
                Latitude = 51.9,
                Longitude = 4.5,
                Description = "Updated description"
            };

            _locationDbQueriesMock.Setup(q => q.GetByIdAsync(locationId)).ReturnsAsync((Location)null);

            var result = await _locationUoW.UpdateLocationAsync(locationId, updateDto);

            Assert.Equal(StatusCodeResponse.NotFound, result.StatusCodeResponse);
            Assert.Equal($"Location with ID: {locationId} not found for update", result.Message);
            Assert.Null(result.Data);

            _locationDbQueriesMock.Verify(q => q.GetByIdAsync(locationId), Times.Once);
            _locationDbQueriesMock.Verify(q => q.UpdateAsync(It.IsAny<Location>()), Times.Never);
            _mapperMock.Verify(m => m.Map<LocationDto>(It.IsAny<Location>()), Times.Never);
        }

        [Fact]
        public async Task DeleteLocationAsync_ReturnsSuccess_WhenDeletionSucceeds()
        {
            Guid locationId = Guid.NewGuid();
            LocationUoW unitOfWork = new LocationUoW(_contextMock.Object, _locationDbQueriesMock.Object,
                _mapperMock.Object, _loggerMock.Object);

            _locationDbQueriesMock
                .Setup(q => q.DeleteAsync(locationId))
                .ReturnsAsync(true);

            StatusCodeResponseDto<bool> result = await unitOfWork.DeleteLocationAsync(locationId);

            Assert.Equal(StatusCodeResponse.Success, result.StatusCodeResponse);
            Assert.Equal($"Location with ID: {locationId} deleted successfully", result.Message);
            Assert.True(result.Data);
        }

        [Fact]
        public async Task DeleteLocationAsync_ReturnsNotFound_WhenDeletionFails()
        {
            Guid locationId = Guid.NewGuid();
            LocationUoW unitOfWork = new LocationUoW(_contextMock.Object, _locationDbQueriesMock.Object,
                _mapperMock.Object, _loggerMock.Object);

            _locationDbQueriesMock
                .Setup(q => q.DeleteAsync(locationId))
                .ReturnsAsync(false);

            StatusCodeResponseDto<bool> result = await unitOfWork.DeleteLocationAsync(locationId);

            Assert.Equal(StatusCodeResponse.NotFound, result.StatusCodeResponse);
            Assert.Equal($"Location with ID: {locationId} not found for deletion", result.Message);
            Assert.False(result.Data);
        }

        [Fact]
        public async Task GetSelectableLocationsAsync_ReturnsLocations_WhenSuccessful()
        {
            LocationUoW unitOfWork = new LocationUoW(_contextMock.Object, _locationDbQueriesMock.Object,
                _mapperMock.Object, _loggerMock.Object);

            List<SelectableLocationDto> expectedLocations = new List<SelectableLocationDto>
            {
                new SelectableLocationDto(),
                new SelectableLocationDto()
            };

            _locationDbQueriesMock
                .Setup(q => q.GetSelectableLocationsAsync())
                .ReturnsAsync(expectedLocations);

            IEnumerable<SelectableLocationDto> result = await unitOfWork.GetSelectableLocationsAsync();

            Assert.Equal(expectedLocations, result);
        }

        [Fact]
        public async Task GetSelectableLocationsAsync_ThrowsException_WhenDbQueryFails()
        {
            LocationUoW unitOfWork = new LocationUoW(_contextMock.Object, _locationDbQueriesMock.Object,
                _mapperMock.Object, _loggerMock.Object);

            Exception expectedException = new Exception("Database failure");

            _locationDbQueriesMock
                .Setup(q => q.GetSelectableLocationsAsync())
                .ThrowsAsync(expectedException);

            Exception actualException =
                await Assert.ThrowsAsync<Exception>(() => unitOfWork.GetSelectableLocationsAsync());

            Assert.Equal(expectedException.Message, actualException.Message);
        }

        [Fact]
        public async Task GetGroupedSelectableLocationsAsync_ReturnsGroupedLocations()
        {
            LocationUoW unitOfWork = new LocationUoW(_contextMock.Object, _locationDbQueriesMock.Object,
                _mapperMock.Object, _loggerMock.Object);

            List<SelectableLocationDto> selectableLocations = new List<SelectableLocationDto>
            {
                new SelectableLocationDto { Category = "Park" },
                new SelectableLocationDto { Category = "Museum" },
                new SelectableLocationDto { Category = null },
                new SelectableLocationDto { Category = "" },
                new SelectableLocationDto { Category = "Park" }
            };

            _locationDbQueriesMock
                .Setup(q => q.GetSelectableLocationsAsync())
                .ReturnsAsync(selectableLocations);

            StatusCodeResponseDto<Dictionary<string, List<SelectableLocationDto>>> result =
                await unitOfWork.GetGroupedSelectableLocationsAsync();

            Assert.Equal(StatusCodeResponse.Success, result.StatusCodeResponse);
            Assert.Equal($"Successfully retrieved and grouped 4 categories.", result.Message);

            Dictionary<string, List<SelectableLocationDto>> expectedGroups =
                new Dictionary<string, List<SelectableLocationDto>>
                {
                    { "Park", selectableLocations.Where(s => s.Category == "Park").ToList() },
                    { "Museum", selectableLocations.Where(s => s.Category == "Museum").ToList() },
                    { "Uncategorized", selectableLocations.Where(s => s.Category == null).ToList() },
                    { "", selectableLocations.Where(s => s.Category == "").ToList() }
                };

            Assert.Equal(expectedGroups.Keys.OrderBy(k => k), result.Data.Keys.OrderBy(k => k));
            foreach (string key in expectedGroups.Keys)
            {
                Assert.Equal(expectedGroups[key], result.Data[key]);
            }
        }

        [Fact]
        public async Task GetGroupedSelectableLocationsAsync_ReturnsEmpty_WhenNoLocations()
        {
            LocationUoW unitOfWork = new LocationUoW(_contextMock.Object, _locationDbQueriesMock.Object,
                _mapperMock.Object, _loggerMock.Object);

            List<SelectableLocationDto> emptyList = new List<SelectableLocationDto>();

            _locationDbQueriesMock
                .Setup(q => q.GetSelectableLocationsAsync())
                .ReturnsAsync(emptyList);

            StatusCodeResponseDto<Dictionary<string, List<SelectableLocationDto>>> result =
                await unitOfWork.GetGroupedSelectableLocationsAsync();

            Assert.Equal(StatusCodeResponse.Success, result.StatusCodeResponse);
            Assert.Equal("Successfully retrieved and grouped 0 categories.", result.Message);
            Assert.Empty(result.Data);
        }

        [Fact]
        public async Task GetAllLocationFromOneCategory_ReturnsLocationsMatchingCategory()
        {
            LocationUoW unitOfWork = new LocationUoW(_contextMock.Object, _locationDbQueriesMock.Object,
                _mapperMock.Object, _loggerMock.Object);

            string targetCategory = "Park";

            List<SelectableLocationDto> selectableLocations = new List<SelectableLocationDto>
            {
                new SelectableLocationDto { Category = "Park" },
                new SelectableLocationDto { Category = "Museum" },
                new SelectableLocationDto { Category = "Park" }
            };

            _locationDbQueriesMock
                .Setup(q => q.GetSelectableLocationsAsync())
                .ReturnsAsync(selectableLocations);

            StatusCodeResponseDto<IEnumerable<SelectableLocationDto>> result =
                await unitOfWork.GetAllLocationFromOneCategory(targetCategory);

            Assert.Equal(StatusCodeResponse.Success, result.StatusCodeResponse);
            Assert.Equal("Sucessfully retrieved and sorted all location that contains the given category",
                result.Message);

            IEnumerable<SelectableLocationDto> expected = selectableLocations.Where(l => l.Category == targetCategory);
            Assert.Equal(expected, result.Data);
        }

        [Fact]
        public async Task GetAllLocationFromOneCategory_ReturnsEmpty_WhenNoMatch()
        {
            LocationUoW unitOfWork = new LocationUoW(_contextMock.Object, _locationDbQueriesMock.Object,
                _mapperMock.Object, _loggerMock.Object);

            string targetCategory = "Zoo";

            List<SelectableLocationDto> selectableLocations = new List<SelectableLocationDto>
            {
                new SelectableLocationDto { Category = "Park" },
                new SelectableLocationDto { Category = "Museum" }
            };

            _locationDbQueriesMock
                .Setup(q => q.GetSelectableLocationsAsync())
                .ReturnsAsync(selectableLocations);

            StatusCodeResponseDto<IEnumerable<SelectableLocationDto>> result =
                await unitOfWork.GetAllLocationFromOneCategory(targetCategory);

            Assert.Equal(StatusCodeResponse.Success, result.StatusCodeResponse);
            Assert.Equal("Sucessfully retrieved and sorted all location that contains the given category",
                result.Message);
            Assert.Empty(result.Data);
        }
    }
}