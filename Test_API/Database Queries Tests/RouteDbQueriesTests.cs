using Microsoft.EntityFrameworkCore;
using Routeplanner_API.Database_Queries;
using Routeplanner_API.Models;

namespace Test_API.Database_Queries_Tests
{
    public class RouteDbQueriesTests
    {
        [Fact]
        public async Task GetByIdAsync_ExistingRouteId_ReturnsRoute()
        {
            RouteplannerDbContext context = await GetInMemoryDbContext();
            RouteDbQueries routeDbQueries = new RouteDbQueries(context);

            Route route = new Route
            {
                RouteId = Guid.NewGuid(),
                // Initialize other properties if required
            };

            await context.Routes.AddAsync(route);
            await context.SaveChangesAsync();

            Route? result = await routeDbQueries.GetByIdAsync(route.RouteId);

            Assert.NotNull(result);
            Assert.Equal(route.RouteId, result.RouteId);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingRouteId_ReturnsNull()
        {
            RouteplannerDbContext context = await GetInMemoryDbContext();
            RouteDbQueries routeDbQueries = new RouteDbQueries(context);

            Guid nonExistingRouteId = Guid.NewGuid();

            Route? result = await routeDbQueries.GetByIdAsync(nonExistingRouteId);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_WhenRoutesExist_ReturnsAllRoutes()
        {
            RouteplannerDbContext context = await GetInMemoryDbContext();
            RouteDbQueries routeDbQueries = new RouteDbQueries(context);

            Route route1 = new Route
            {
                RouteId = Guid.NewGuid(),
                // Initialize other required properties if any
            };

            Route route2 = new Route
            {
                RouteId = Guid.NewGuid(),
                // Initialize other required properties if any
            };

            await context.Routes.AddAsync(route1);
            await context.Routes.AddAsync(route2);
            await context.SaveChangesAsync();

            IEnumerable<Route?> result = await routeDbQueries.GetAllAsync();

            List<Route?> resultList = result.ToList();

            Assert.Equal(2, resultList.Count);
            Assert.Contains(resultList, r => r != null && r.RouteId == route1.RouteId);
            Assert.Contains(resultList, r => r != null && r.RouteId == route2.RouteId);
        }

        [Fact]
        public async Task GetAllAsync_WhenNoRoutesExist_ReturnsEmptyList()
        {
            RouteplannerDbContext context = await GetInMemoryDbContext();
            RouteDbQueries routeDbQueries = new RouteDbQueries(context);

            IEnumerable<Route?> result = await routeDbQueries.GetAllAsync();

            List<Route?> resultList = result.ToList();

            Assert.Empty(resultList);
        }

        


        private async Task<RouteplannerDbContext> GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<RouteplannerDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            RouteplannerDbContext? context = new RouteplannerDbContext(options);
            await context.Database.EnsureCreatedAsync();
            return context;
        }
    }
}
