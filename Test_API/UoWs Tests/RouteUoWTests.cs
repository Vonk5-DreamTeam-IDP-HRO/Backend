using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Routeplanner_API.Database_Queries;
using Routeplanner_API.DTO;
using Routeplanner_API.DTO.Route;
using Routeplanner_API.Enums;
using Routeplanner_API.Models;
using Routeplanner_API.UoWs;
using System.Text.Json;

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
        public async Task CreateRouteAsync_ReturnsSuccess_WhenRouteIsCreated()
        {
            Guid userId = Guid.NewGuid();
            Guid routeId = Guid.NewGuid();

            CreateRouteDto createRouteDto = new CreateRouteDto
            {
                Name = "Test Route",
                Description = "A valid route",
                IsPrivate = true,
                LocationIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
            };

            _mockMapper
                .Setup(m => m.Map<Route>(
                    It.IsAny<CreateRouteDto>(),
                    It.IsAny<Action<IMappingOperationOptions>>()))
                .Returns<CreateRouteDto, Action<IMappingOperationOptions>>((dto, opts) =>
                {
                    opts?.Invoke(Mock.Of<IMappingOperationOptions>());
                    return new Route
                    {
                        RouteId = routeId,
                        Name = dto.Name,
                        Description = dto.Description,
                        IsPrivate = dto.IsPrivate,
                        CreatedBy = userId
                    };
                });

            _mockRouteDbQueries
                .Setup(repo => repo.CreateAsync(It.IsAny<Route>()))
                .ReturnsAsync((Route r) => r);

            StatusCodeResponseDto<string?> result = await _routeUoW.CreateRouteAsync(createRouteDto, userId);

            Assert.Equal(StatusCodeResponse.Success, result.StatusCodeResponse);
            Assert.Equal(routeId.ToString(), result.Data);
        }


        [Fact]
        public async Task CreateRouteAsync_ReturnsBadRequest_WhenArgumentNullExceptionThrown()
        {
            Guid userId = Guid.NewGuid();

            CreateRouteDto createRouteDto = new CreateRouteDto
            {
                Name = "Invalid Route",
                IsPrivate = true,
                LocationIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
            };

            StatusCodeResponseDto<string?> result = await _routeUoW.CreateRouteAsync(createRouteDto, userId);

            Assert.Equal(StatusCodeResponse.BadRequest, result.StatusCodeResponse);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task CreateRouteAsync_ReturnsInternalServerError_WhenMappingFails()
        {
            Guid userId = Guid.NewGuid();

            CreateRouteDto createRouteDto = new CreateRouteDto
            {
                Name = "Mapping Failure",
                IsPrivate = false,
                LocationIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
            };

            _mockMapper
                .Setup(mapper => mapper.Map<Route>(It.IsAny<CreateRouteDto>(), It.IsAny<Action<IMappingOperationOptions>>()))
                .Throws(new AutoMapperMappingException("Mapping failed"));

            StatusCodeResponseDto<string?> result = await _routeUoW.CreateRouteAsync(createRouteDto, userId);

            Assert.Equal(StatusCodeResponse.InternalServerError, result.StatusCodeResponse);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task CreateRouteAsync_ReturnsBadRequest_WhenJsonExceptionThrown()
        {
            Guid userId = Guid.NewGuid();

            CreateRouteDto createRouteDto = new CreateRouteDto
            {
                Name = "Json Error Route",
                IsPrivate = true,
                LocationIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
            };

            _mockMapper
                .Setup(mapper => mapper.Map<Route>(It.IsAny<CreateRouteDto>(), It.IsAny<Action<IMappingOperationOptions>>()))
                .Throws(new JsonException("JSON error"));

            StatusCodeResponseDto<string?> result = await _routeUoW.CreateRouteAsync(createRouteDto, userId);

            Assert.Equal(StatusCodeResponse.BadRequest, result.StatusCodeResponse);
            Assert.Null(result.Data);
        }
    }
}
