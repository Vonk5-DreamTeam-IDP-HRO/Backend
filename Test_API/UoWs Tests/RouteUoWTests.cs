using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Routeplanner_API.Database_Queries;
using Routeplanner_API.DTO.Route;
using Routeplanner_API.Enums;
using Routeplanner_API.Models;
using Routeplanner_API.UoWs;
using Xunit;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Test_API.UoWs_Tests
{
    public class RouteUoWTests
    {
        private readonly Mock<IRouteDbQueries> _mockRouteDbQueries;
        private readonly Mock<ILogger<RouteUoW>> _mockLogger;
        private readonly Mock<IMapper> _mockMapper;
        private readonly RouteUoW _routeUoW;

        public RouteUoWTests()
        {
            _mockRouteDbQueries = new Mock<IRouteDbQueries>();
            _mockLogger = new Mock<ILogger<RouteUoW>>();
            _mockMapper = new Mock<IMapper>();
            _routeUoW = new RouteUoW(_mockRouteDbQueries.Object, _mockLogger.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetRoutesAsync_ReturnsRoutes_WhenRoutesExist()
        {
            var routes = new List<Route>
            {
                new Route
                {
                    RouteId = Guid.NewGuid(),
                    Name = "Route 1",
                    Description = "Description 1",
                    IsPrivate = false,
                    CreatedBy = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Route
                {
                    RouteId = Guid.NewGuid(),
                    Name = "Route 2",
                    Description = "Description 2",
                    IsPrivate = true,
                    CreatedBy = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            var routeDtos = routes.Select(r => new RouteDto
            {
                RouteId = r.RouteId,
                Name = r.Name,
                Description = r.Description,
                IsPrivate = r.IsPrivate,
                CreatedBy = r.CreatedBy,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt,
                LocationIds = new List<Guid>()
            });

            _mockRouteDbQueries.Setup(x => x.GetAllAsync()).ReturnsAsync(routes);
            _mockMapper.Setup(x => x.Map<IEnumerable<RouteDto>>(routes)).Returns(routeDtos);

            var result = await _routeUoW.GetRoutesAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockRouteDbQueries.Verify(x => x.GetAllAsync(), Times.Once);
            _mockMapper.Verify(x => x.Map<IEnumerable<RouteDto>>(routes), Times.Once);
        }

        [Fact]
        public async Task GetRoutesAsync_ReturnsEmptyList_WhenNoRoutesExist()
        {
            var emptyRoutes = new List<Route>();
            var emptyRouteDtos = new List<RouteDto>();

            _mockRouteDbQueries.Setup(x => x.GetAllAsync()).ReturnsAsync(emptyRoutes);
            _mockMapper.Setup(x => x.Map<IEnumerable<RouteDto>>(emptyRoutes)).Returns(emptyRouteDtos);

            var result = await _routeUoW.GetRoutesAsync();

            Assert.NotNull(result);
            Assert.Empty(result);
            _mockRouteDbQueries.Verify(x => x.GetAllAsync(), Times.Once);
            _mockMapper.Verify(x => x.Map<IEnumerable<RouteDto>>(emptyRoutes), Times.Once);
        }

        [Fact]
        public async Task GetRouteByIdAsync_ReturnsRoute_WhenRouteExists()
        {
            var routeId = Guid.NewGuid();
            var route = new Route
            {
                RouteId = routeId,
                Name = "Test Route",
                Description = "Test Description",
                IsPrivate = false,
                CreatedBy = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var routeDto = new RouteDto
            {
                RouteId = route.RouteId,
                Name = route.Name,
                Description = route.Description,
                IsPrivate = route.IsPrivate,
                CreatedBy = route.CreatedBy,
                CreatedAt = route.CreatedAt,
                UpdatedAt = route.UpdatedAt,
                LocationIds = new List<Guid>()
            };

            _mockRouteDbQueries.Setup(x => x.GetByIdAsync(routeId)).ReturnsAsync(route);
            _mockMapper.Setup(x => x.Map<RouteDto>(route)).Returns(routeDto);

            var result = await _routeUoW.GetRouteByIdAsync(routeId);

            Assert.NotNull(result);
            Assert.Equal(StatusCodeResponse.Success, result.StatusCodeResponse);
            Assert.NotNull(result.Data);
            Assert.Equal(routeId, result.Data.RouteId);
            Assert.Null(result.Message);
            _mockRouteDbQueries.Verify(x => x.GetByIdAsync(routeId), Times.Once);
            _mockMapper.Verify(x => x.Map<RouteDto>(route), Times.Once);
        }

        [Fact]
        public async Task GetRouteByIdAsync_ReturnsNotFound_WhenRouteDoesNotExist()
        {
            var routeId = Guid.NewGuid();
            _mockRouteDbQueries.Setup(x => x.GetByIdAsync(routeId)).ReturnsAsync((Route)null);

            var result = await _routeUoW.GetRouteByIdAsync(routeId);

            Assert.NotNull(result);
            Assert.Equal(StatusCodeResponse.NotFound, result.StatusCodeResponse);
            Assert.Null(result.Data);
            Assert.Equal($"Route with ID: {routeId} not found", result.Message);
            _mockRouteDbQueries.Verify(x => x.GetByIdAsync(routeId), Times.Once);
            _mockMapper.Verify(x => x.Map<RouteDto>(It.IsAny<Route>()), Times.Never);
        }

        [Fact]
        public async Task CreateRouteAsync_ReturnsSuccess_WhenRouteIsCreated()
        {
            var _contextMock = new Mock<RouteplannerDbContext>();
            var userId = Guid.NewGuid();
            var routeId = Guid.NewGuid();

            var user = new User
            {
                Id = userId,
                UserName = "testuser",
                Email = "test@example.com",
                PasswordHash = "hashedpassword"
            };

            var mockDbSet = new Mock<DbSet<User>>();
            _contextMock.Setup(c => c.Users).Returns(mockDbSet.Object);
            _contextMock.Setup(c => c.Users.FindAsync(userId)).ReturnsAsync(user);

            var createRouteDto = new CreateRouteDto
            {
                Name = "Test Route",
                Description = "A test route",
                IsPrivate = true,
                LocationIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
            };

            var routeEntity = new Route
            {
                RouteId = routeId,
                Name = createRouteDto.Name,
                Description = createRouteDto.Description,
                IsPrivate = createRouteDto.IsPrivate,
                CreatedBy = user.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var routeDto = new RouteDto
            {
                RouteId = routeEntity.RouteId,
                Name = routeEntity.Name,
                Description = routeEntity.Description,
                IsPrivate = routeEntity.IsPrivate,
                CreatedBy = user.Id,
                CreatedAt = routeEntity.CreatedAt,
                UpdatedAt = routeEntity.UpdatedAt,
                LocationIds = createRouteDto.LocationIds
            };

            _mockMapper
                .Setup(m => m.Map<Route>(
                    It.IsAny<CreateRouteDto>(),
                    It.IsAny<Action<IMappingOperationOptions<object, Route>>>()))
                .Returns(routeEntity);
            _mockMapper.Setup(m => m.Map<RouteDto>(routeEntity)).Returns(routeDto);

            _mockRouteDbQueries
                .Setup(repo => repo.CreateAsync(It.IsAny<Route>()))
                .ReturnsAsync(routeEntity);

            var result = await _routeUoW.CreateRouteAsync(createRouteDto, userId);

            Assert.NotNull(result);
            Assert.Equal(StatusCodeResponse.Success, result.StatusCodeResponse);
            Assert.Equal(routeId.ToString(), result.Data);
            Assert.Contains("Route created successfully", result.Message);

            _mockMapper.Verify(m => m.Map<Route>(createRouteDto, It.IsAny<Action<IMappingOperationOptions<object, Route>>>()), Times.Once);
            _mockRouteDbQueries.Verify(repo => repo.CreateAsync(It.Is<Route>(r => r.RouteId == routeId)), Times.Once);
        }

        [Fact]
        public async Task CreateRouteAsync_ReturnsBadRequest_WhenArgumentNullExceptionThrown()
        {
            var userId = Guid.NewGuid();
            var createRouteDto = new CreateRouteDto
            {
                Name = "Test Route",
                Description = "A test route",
                IsPrivate = true,
                LocationIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
            };

            _mockMapper
                .Setup(m => m.Map<Route>(
                    It.IsAny<CreateRouteDto>(),
                    It.IsAny<Action<IMappingOperationOptions<object, Route>>>()))
                .Throws(new ArgumentNullException("dto"));

            var result = await _routeUoW.CreateRouteAsync(createRouteDto, userId);

            Assert.NotNull(result);
            Assert.Equal(StatusCodeResponse.BadRequest, result.StatusCodeResponse);
            Assert.Null(result.Data);
            Assert.Equal("Error adding route due to null argument.", result.Message);

            _mockMapper.Verify(m => m.Map<Route>(createRouteDto, It.IsAny<Action<IMappingOperationOptions<object, Route>>>()), Times.Once);
            _mockRouteDbQueries.Verify(repo => repo.CreateAsync(It.IsAny<Route>()), Times.Never);
        }

        [Fact]
        public async Task CreateRouteAsync_ReturnsInternalServerError_WhenAutoMapperMappingExceptionThrown()
        {
            var userId = Guid.NewGuid();
            var createRouteDto = new CreateRouteDto
            {
                Name = "Test Route",
                Description = "A test route",
                IsPrivate = true,
                LocationIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
            };

            var mappingException = new AutoMapperMappingException("Mapping failed");
            _mockMapper
                .Setup(m => m.Map<Route>(
                    It.IsAny<CreateRouteDto>(),
                    It.IsAny<Action<IMappingOperationOptions<object, Route>>>()))
                .Throws(mappingException);

            var result = await _routeUoW.CreateRouteAsync(createRouteDto, userId);

            Assert.NotNull(result);
            Assert.Equal(StatusCodeResponse.InternalServerError, result.StatusCodeResponse);
            Assert.Null(result.Data);
            Assert.Contains("An error occurred during data mapping", result.Message);
            Assert.Contains("Mapping failed", result.Message);

            _mockMapper.Verify(m => m.Map<Route>(createRouteDto, It.IsAny<Action<IMappingOperationOptions<object, Route>>>()), Times.Once);
            _mockRouteDbQueries.Verify(repo => repo.CreateAsync(It.IsAny<Route>()), Times.Never);
        }

        [Fact]
        public async Task CreateRouteAsync_ReturnsBadRequest_WhenJsonExceptionThrown()
        {
            var userId = Guid.NewGuid();
            var createRouteDto = new CreateRouteDto
            {
                Name = "Test Route",
                Description = "A test route",
                IsPrivate = true,
                LocationIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
            };

            _mockMapper
                .Setup(m => m.Map<Route>(
                    It.IsAny<CreateRouteDto>(),
                    It.IsAny<Action<IMappingOperationOptions<object, Route>>>()))
                .Throws(new JsonException("JSON processing failed"));

            var result = await _routeUoW.CreateRouteAsync(createRouteDto, userId);

            Assert.NotNull(result);
            Assert.Equal(StatusCodeResponse.BadRequest, result.StatusCodeResponse);
            Assert.Null(result.Data);
            Assert.Equal("Error adding route due to JSON processing issue.", result.Message);

            _mockMapper.Verify(m => m.Map<Route>(createRouteDto, It.IsAny<Action<IMappingOperationOptions<object, Route>>>()), Times.Once);
            _mockRouteDbQueries.Verify(repo => repo.CreateAsync(It.IsAny<Route>()), Times.Never);
        }

        [Fact]
        public async Task UpdateRouteAsync_ReturnsSuccess_WhenRouteIsUpdated()
        {
            var routeId = Guid.NewGuid();
            var existingRoute = new Route
            {
                RouteId = routeId,
                Name = "Original Route",
                Description = "Original Description",
                IsPrivate = false,
                CreatedBy = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            };

            var updateRouteDto = new UpdateRouteDto
            {
                RouteId = routeId,
                Name = "Updated Route",
                Description = "Updated Description",
                IsPrivate = true,
                LocationIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
            };

            var updatedRoute = new Route
            {
                RouteId = routeId,
                Name = updateRouteDto.Name,
                Description = updateRouteDto.Description,
                IsPrivate = updateRouteDto.IsPrivate,
                CreatedBy = existingRoute.CreatedBy,
                CreatedAt = existingRoute.CreatedAt,
                UpdatedAt = DateTime.UtcNow
            };

            var updatedRouteDto = new RouteDto
            {
                RouteId = updatedRoute.RouteId,
                Name = updatedRoute.Name,
                Description = updatedRoute.Description,
                IsPrivate = updatedRoute.IsPrivate,
                CreatedBy = updatedRoute.CreatedBy,
                CreatedAt = updatedRoute.CreatedAt,
                UpdatedAt = updatedRoute.UpdatedAt,
                LocationIds = updateRouteDto.LocationIds
            };

            _mockRouteDbQueries.Setup(x => x.GetByIdAsync(routeId)).ReturnsAsync(existingRoute);
            _mockMapper.Setup(x => x.Map(updateRouteDto, existingRoute));
            _mockRouteDbQueries.Setup(x => x.UpdateRouteAsync(existingRoute)).ReturnsAsync(updatedRoute);
            _mockMapper.Setup(x => x.Map<RouteDto>(updatedRoute)).Returns(updatedRouteDto);

            var result = await _routeUoW.UpdateRouteAsync(routeId, updateRouteDto);

            Assert.NotNull(result);
            Assert.Equal(StatusCodeResponse.Success, result.StatusCodeResponse);
            Assert.NotNull(result.Data);
            Assert.Equal(routeId, result.Data.RouteId);
            Assert.Contains("updated successfully", result.Message);

            _mockRouteDbQueries.Verify(x => x.GetByIdAsync(routeId), Times.Once);
            _mockMapper.Verify(x => x.Map(updateRouteDto, existingRoute), Times.Once);
            _mockRouteDbQueries.Verify(x => x.UpdateRouteAsync(existingRoute), Times.Once);
            _mockMapper.Verify(x => x.Map<RouteDto>(updatedRoute), Times.Once);
        }

        [Fact]
        public async Task UpdateRouteAsync_ReturnsNotFound_WhenRouteDoesNotExist()
        {
            var routeId = Guid.NewGuid();
            var updateRouteDto = new UpdateRouteDto
            {
                RouteId = routeId,
                Name = "Updated Route",
                Description = "Updated Description",
                IsPrivate = true,
                LocationIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
            };

            _mockRouteDbQueries.Setup(x => x.GetByIdAsync(routeId)).ReturnsAsync((Route)null);

            var result = await _routeUoW.UpdateRouteAsync(routeId, updateRouteDto);

            Assert.NotNull(result);
            Assert.Equal(StatusCodeResponse.NotFound, result.StatusCodeResponse);
            Assert.Null(result.Data);
            Assert.Equal($"Route with ID: {routeId} not found for update", result.Message);

            _mockRouteDbQueries.Verify(x => x.GetByIdAsync(routeId), Times.Once);
            _mockMapper.Verify(x => x.Map(It.IsAny<UpdateRouteDto>(), It.IsAny<Route>()), Times.Never);
            _mockRouteDbQueries.Verify(x => x.UpdateRouteAsync(It.IsAny<Route>()), Times.Never);
        }

        [Fact]
        public async Task DeleteRouteAsync_ReturnsSuccess_WhenRouteIsDeleted()
        {
            var routeId = Guid.NewGuid();
            _mockRouteDbQueries.Setup(x => x.DeleteRouteAsync(routeId)).ReturnsAsync(true);

            var result = await _routeUoW.DeleteRouteAsync(routeId);

            Assert.NotNull(result);
            Assert.Equal(StatusCodeResponse.Success, result.StatusCodeResponse);
            Assert.True(result.Data);
            Assert.Contains("deleted successfully", result.Message);

            _mockRouteDbQueries.Verify(x => x.DeleteRouteAsync(routeId), Times.Once);
        }

        [Fact]
        public async Task DeleteRouteAsync_ReturnsNotFound_WhenRouteDoesNotExist()
        {
            var routeId = Guid.NewGuid();
            _mockRouteDbQueries.Setup(x => x.DeleteRouteAsync(routeId)).ReturnsAsync(false);

            var result = await _routeUoW.DeleteRouteAsync(routeId);

            Assert.NotNull(result);
            Assert.Equal(StatusCodeResponse.NotFound, result.StatusCodeResponse);
            Assert.False(result.Data);
            Assert.Contains("not found for deletion", result.Message);

            _mockRouteDbQueries.Verify(x => x.DeleteRouteAsync(routeId), Times.Once);
        }
    }
}
