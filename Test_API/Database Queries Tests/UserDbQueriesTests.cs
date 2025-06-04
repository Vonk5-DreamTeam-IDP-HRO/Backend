using Microsoft.EntityFrameworkCore;
using Routeplanner_API.Database_Queries;
using Routeplanner_API.Models;

namespace Test_API.Database_Queries_Tests
{
    public class UserDbQueriesTests
    {
        [Fact]
        public async Task CreateAsync_ValidUser_AddsUserAndReturnsIt()
        {
            RouteplannerDbContext? context = await GetInMemoryDbContext();
            UserDbQueries userDbQueries = new UserDbQueries(context);
            User? user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "testuser",
                PasswordHash = "hashed_password",
                CreatedAt = DateTime.UtcNow,
                UserRightId = Guid.NewGuid()
            };

            User? result = await userDbQueries.CreateAsync(user);

            Assert.NotNull(result);
            Assert.Equal(user.UserName, result.UserName);

            User? userInDb = await context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
            Assert.NotNull(userInDb);
            Assert.Equal(user.UserName, userInDb.UserName);
        }

        [Fact]
        public async Task CreateAsync_NullUser_ThrowsArgumentNullException()
        {
            RouteplannerDbContext? context = await GetInMemoryDbContext();
            UserDbQueries? userDbQueries = new UserDbQueries(context);

            await Assert.ThrowsAsync<ArgumentNullException>(() => userDbQueries.CreateAsync(null));
        }

        [Fact]
        public async Task UpdateAsync_ExistingUser_UpdatesAndReturnsUser()
        {
            RouteplannerDbContext context = await GetInMemoryDbContext();
            UserDbQueries userDbQueries = new UserDbQueries(context);

            User originalUser = new User
            {
                Id = Guid.NewGuid(),
                UserName = "original_user",
                PasswordHash = "original_hash",
                UserRightId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow
            };

            await context.Users.AddAsync(originalUser);
            await context.SaveChangesAsync();

            User updatedUser = new User
            {
                Id = originalUser.Id,
                UserName = "updated_user",
                PasswordHash = "updated_hash",
                UserRightId = Guid.NewGuid(),
                CreatedAt = originalUser.CreatedAt
            };

            User? result = await userDbQueries.UpdateAsync(updatedUser);

            Assert.NotNull(result);
            Assert.Equal("updated_user", result.UserName);
            Assert.Equal("updated_hash", result.PasswordHash);

            User? userInDb = await context.Users.FirstOrDefaultAsync(u => u.Id == originalUser.Id);
            Assert.NotNull(userInDb);
            Assert.Equal("updated_user", userInDb.UserName);
        }

        [Fact]
        public async Task UpdateAsync_NonExistingUser_ThrowsArgumentNullException()
        {
            RouteplannerDbContext context = await GetInMemoryDbContext();
            UserDbQueries userDbQueries = new UserDbQueries(context);

            User nonExistingUser = new User
            {
                Id = Guid.NewGuid(),
                UserName = "ghost",
                PasswordHash = "ghost_hash",
                UserRightId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow
            };

            await Assert.ThrowsAsync<ArgumentNullException>(() => userDbQueries.UpdateAsync(nonExistingUser));
        }


        [Fact]
        public async Task DeleteAsync_ExistingUser_ReturnsTrueAndRemovesUser()
        {
            RouteplannerDbContext context = await GetInMemoryDbContext();
            UserDbQueries userDbQueries = new UserDbQueries(context);

            User user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "to_delete",
                PasswordHash = "password",
                UserRightId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow
            };

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            bool result = await userDbQueries.DeleteAsync(user.Id);

            Assert.True(result);

            User? userInDb = await context.Users.FindAsync(user.Id);
            Assert.Null(userInDb);
        }

        [Fact]
        public async Task DeleteAsync_NonExistingUser_ReturnsFalse()
        {
            RouteplannerDbContext context = await GetInMemoryDbContext();
            UserDbQueries userDbQueries = new UserDbQueries(context);

            Guid nonExistingUserId = Guid.NewGuid();

            bool result = await userDbQueries.DeleteAsync(nonExistingUserId);

            Assert.False(result);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingUserId_ReturnsUser()
        {
            RouteplannerDbContext context = await GetInMemoryDbContext();
            UserDbQueries userDbQueries = new UserDbQueries(context);

            User user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "existing_user",
                PasswordHash = "password",
                UserRightId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow
            };

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            User? result = await userDbQueries.GetByIdAsync(user.Id);

            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal("existing_user", result.UserName);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingUserId_ReturnsNull()
        {
            RouteplannerDbContext context = await GetInMemoryDbContext();
            UserDbQueries userDbQueries = new UserDbQueries(context);

            Guid nonExistingUserId = Guid.NewGuid();

            User? result = await userDbQueries.GetByIdAsync(nonExistingUserId);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_WhenUsersExist_ReturnsAllUsers()
        {
            RouteplannerDbContext context = await GetInMemoryDbContext();
            UserDbQueries userDbQueries = new UserDbQueries(context);

            User user1 = new User
            {
                Id = Guid.NewGuid(),
                UserName = "user1",
                PasswordHash = "hash1",
                UserRightId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow
            };

            User user2 = new User
            {
                Id = Guid.NewGuid(),
                UserName = "user2",
                PasswordHash = "hash2",
                UserRightId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow
            };

            await context.Users.AddAsync(user1);
            await context.Users.AddAsync(user2);
            await context.SaveChangesAsync();

            IEnumerable<User> result = await userDbQueries.GetAllAsync();

            List<User> resultList = result.ToList();

            Assert.Equal(2, resultList.Count);
            Assert.Contains(resultList, u => u.UserName == "user1");
            Assert.Contains(resultList, u => u.UserName == "user2");
        }

        [Fact]
        public async Task GetAllAsync_WhenNoUsersExist_ReturnsEmptyList()
        {
            RouteplannerDbContext context = await GetInMemoryDbContext();
            UserDbQueries userDbQueries = new UserDbQueries(context);

            IEnumerable<User> result = await userDbQueries.GetAllAsync();

            List<User> resultList = result.ToList();

            Assert.Empty(resultList);
        }

        [Fact]
        public async Task FindUserByUsername_ExistingUsername_ReturnsUser()
        {
            RouteplannerDbContext context = await GetInMemoryDbContext();
            UserDbQueries userDbQueries = new UserDbQueries(context);

            User user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "existing_user",
                PasswordHash = "password",
                UserRightId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow
            };

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            User? result = await userDbQueries.FindUserByUsername("existing_user");

            Assert.NotNull(result);
            Assert.Equal("existing_user", result.UserName);
        }

        [Fact]
        public async Task FindUserByUsername_NonExistingUsername_ReturnsNull()
        {
            RouteplannerDbContext context = await GetInMemoryDbContext();
            UserDbQueries userDbQueries = new UserDbQueries(context);

            User? result = await userDbQueries.FindUserByUsername("ghost_user");

            Assert.Null(result);
        }

        [Fact]
        public async Task FindUserByEmail_ExistingEmail_ReturnsUser()
        {
            RouteplannerDbContext context = await GetInMemoryDbContext();
            UserDbQueries userDbQueries = new UserDbQueries(context);

            User user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "email_user",
                Email = "test@example.com",
                PasswordHash = "password",
                UserRightId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow
            };

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            User? result = await userDbQueries.FindUserByEmail("test@example.com");

            Assert.NotNull(result);
            Assert.Equal("test@example.com", result.Email);
        }

        [Fact]
        public async Task FindUserByEmail_NonExistingEmail_ReturnsNull()
        {
            RouteplannerDbContext context = await GetInMemoryDbContext();
            UserDbQueries userDbQueries = new UserDbQueries(context);

            User? result = await userDbQueries.FindUserByEmail("nonexistent@example.com");

            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserRight_UserRightExists_ReturnsUserPermission()
        {
            RouteplannerDbContext context = await GetInMemoryDbContext();
            UserDbQueries userDbQueries = new UserDbQueries(context);

            UserPermission userPermission = new UserPermission
            {
                Id = Guid.NewGuid(),
                Name = "user"
                // Initialize other properties if needed
            };

            await context.UserRights.AddAsync(userPermission);
            await context.SaveChangesAsync();

            UserPermission? result = await userDbQueries.GetUserRight();

            Assert.NotNull(result);
            Assert.Equal("user", result.Name);
        }

        [Fact]
        public async Task GetUserRight_UserRightDoesNotExist_ReturnsNull()
        {
            RouteplannerDbContext context = await GetInMemoryDbContext();
            UserDbQueries userDbQueries = new UserDbQueries(context);

            UserPermission? result = await userDbQueries.GetUserRight();

            Assert.Null(result);
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
