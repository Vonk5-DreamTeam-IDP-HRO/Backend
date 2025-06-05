using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Routeplanner_API.Database_Queries;
using Routeplanner_API.DTO;
using Routeplanner_API.DTO.Location;
using Routeplanner_API.Enums;
using Routeplanner_API.Models;
using Routeplanner_API.UoWs;

namespace Test_API.UoWs_Tests
{
    public class LocationUoWTests
    {
        private readonly Mock<RouteplannerDbContext> _contextMock = new Mock<RouteplannerDbContext>();
        private readonly Mock<ILocationDbQueries> _locationDbQueriesMock = new Mock<ILocationDbQueries>();
        private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();
        private readonly Mock<ILogger<LocationUoW>> _loggerMock = new Mock<ILogger<LocationUoW>>();

        [Fact]
        public async Task GetLocationsAsync_Returns_MappedLocations()
        {
            LocationUoW unitOfWork = new LocationUoW(_contextMock.Object, _locationDbQueriesMock.Object, _mapperMock.Object, _loggerMock.Object);

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
            LocationUoW unitOfWork = new LocationUoW(_contextMock.Object, _locationDbQueriesMock.Object, _mapperMock.Object, _loggerMock.Object);

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
            LocationUoW unitOfWork = new LocationUoW(_contextMock.Object, _locationDbQueriesMock.Object, _mapperMock.Object, _loggerMock.Object);

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
            LocationUoW unitOfWork = new LocationUoW(_contextMock.Object, _locationDbQueriesMock.Object, _mapperMock.Object, _loggerMock.Object);

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
            LocationUoW unitOfWork = new LocationUoW(_contextMock.Object, _locationDbQueriesMock.Object, _mapperMock.Object, _loggerMock.Object);

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
        public async Task DeleteLocationAsync_ReturnsSuccess_WhenDeletionSucceeds()
        {
            Guid locationId = Guid.NewGuid();
            LocationUoW unitOfWork = new LocationUoW(_contextMock.Object, _locationDbQueriesMock.Object, _mapperMock.Object, _loggerMock.Object);

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
            LocationUoW unitOfWork = new LocationUoW(_contextMock.Object, _locationDbQueriesMock.Object, _mapperMock.Object, _loggerMock.Object);

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
            LocationUoW unitOfWork = new LocationUoW(_contextMock.Object, _locationDbQueriesMock.Object, _mapperMock.Object, _loggerMock.Object);

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
            LocationUoW unitOfWork = new LocationUoW(_contextMock.Object, _locationDbQueriesMock.Object, _mapperMock.Object, _loggerMock.Object);

            Exception expectedException = new Exception("Database failure");

            _locationDbQueriesMock
                .Setup(q => q.GetSelectableLocationsAsync())
                .ThrowsAsync(expectedException);

            Exception actualException = await Assert.ThrowsAsync<Exception>(() => unitOfWork.GetSelectableLocationsAsync());

            Assert.Equal(expectedException.Message, actualException.Message);
        }

        [Fact]
        public async Task GetGroupedSelectableLocationsAsync_ReturnsGroupedLocations()
        {
            LocationUoW unitOfWork = new LocationUoW(_contextMock.Object, _locationDbQueriesMock.Object, _mapperMock.Object, _loggerMock.Object);

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

            StatusCodeResponseDto<Dictionary<string, List<SelectableLocationDto>>> result = await unitOfWork.GetGroupedSelectableLocationsAsync();

            Assert.Equal(StatusCodeResponse.Success, result.StatusCodeResponse);
            Assert.Equal($"Successfully retrieved and grouped 4 categories.", result.Message);

            Dictionary<string, List<SelectableLocationDto>> expectedGroups = new Dictionary<string, List<SelectableLocationDto>>
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
            LocationUoW unitOfWork = new LocationUoW(_contextMock.Object, _locationDbQueriesMock.Object, _mapperMock.Object, _loggerMock.Object);

            List<SelectableLocationDto> emptyList = new List<SelectableLocationDto>();

            _locationDbQueriesMock
                .Setup(q => q.GetSelectableLocationsAsync())
                .ReturnsAsync(emptyList);

            StatusCodeResponseDto<Dictionary<string, List<SelectableLocationDto>>> result = await unitOfWork.GetGroupedSelectableLocationsAsync();

            Assert.Equal(StatusCodeResponse.Success, result.StatusCodeResponse);
            Assert.Equal("Successfully retrieved and grouped 0 categories.", result.Message);
            Assert.Empty(result.Data);
        }

        [Fact]
        public async Task GetAllLocationFromOneCategory_ReturnsLocationsMatchingCategory()
        {
            LocationUoW unitOfWork = new LocationUoW(_contextMock.Object, _locationDbQueriesMock.Object, _mapperMock.Object, _loggerMock.Object);

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

            StatusCodeResponseDto<IEnumerable<SelectableLocationDto>> result = await unitOfWork.GetAllLocationFromOneCategory(targetCategory);

            Assert.Equal(StatusCodeResponse.Success, result.StatusCodeResponse);
            Assert.Equal("Sucessfully retrieved and sorted all location that contains the given category", result.Message);

            IEnumerable<SelectableLocationDto> expected = selectableLocations.Where(l => l.Category == targetCategory);
            Assert.Equal(expected, result.Data);
        }

        [Fact]
        public async Task GetAllLocationFromOneCategory_ReturnsEmpty_WhenNoMatch()
        {
            LocationUoW unitOfWork = new LocationUoW(_contextMock.Object, _locationDbQueriesMock.Object, _mapperMock.Object, _loggerMock.Object);

            string targetCategory = "Zoo";

            List<SelectableLocationDto> selectableLocations = new List<SelectableLocationDto>
            {
                new SelectableLocationDto { Category = "Park" },
                new SelectableLocationDto { Category = "Museum" }
            };

            _locationDbQueriesMock
                .Setup(q => q.GetSelectableLocationsAsync())
                .ReturnsAsync(selectableLocations);

            StatusCodeResponseDto<IEnumerable<SelectableLocationDto>> result = await unitOfWork.GetAllLocationFromOneCategory(targetCategory);

            Assert.Equal(StatusCodeResponse.Success, result.StatusCodeResponse);
            Assert.Equal("Sucessfully retrieved and sorted all location that contains the given category", result.Message);
            Assert.Empty(result.Data);
        }
    }
}
