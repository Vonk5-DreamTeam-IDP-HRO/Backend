using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Routeplanner_API.Database_Queries;
using Routeplanner_API.DTO;
using Routeplanner_API.DTO.User;
using Routeplanner_API.Enums;
using Routeplanner_API.Helpers;
using Routeplanner_API.Models;
using Routeplanner_API.UoWs;
using System.Reflection;

namespace Test_API.UoWs_Tests
{
    public class UserUoWTests
    {
        private readonly Mock<IUserDbQueries> userDbQueriesMock;
        private readonly Mock<IUserHelper> userHelperMock;
        private readonly Mock<IMapper> mapperMock;
        private readonly Mock<ILogger<UserUoW>> loggerMock;
        private readonly UserUoW userUoW;

        public UserUoWTests()
        {
            userDbQueriesMock = new Mock<IUserDbQueries>();
            userHelperMock = new Mock<IUserHelper>();
            mapperMock = new Mock<IMapper>();
            loggerMock = new Mock<ILogger<UserUoW>>();

            userUoW = new UserUoW(userDbQueriesMock.Object, userHelperMock.Object, mapperMock.Object, loggerMock.Object);
        }

        [Fact]
        public async Task GetUsersAsync_WhenUsersExist_ReturnsMappedUserDtos()
        {
            List<User> users = new List<User>
            {
                new User
                {
                    Id = Guid.NewGuid(),
                    UserName = "testuser1",
                    Email = "test1@example.com",
                    PasswordHash = "hashedpassword1"
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    UserName = "testuser2",
                    Email = "test2@example.com",
                    PasswordHash = "hashedpassword2"
                }
            };

            List<UserDto> userDtos = new List<UserDto>
            {
                new UserDto
                {
                    UserId = users[0].Id,
                    Username = users[0].UserName!,
                    Email = users[0].Email!,
                    Password = users[0].PasswordHash!
                },
                new UserDto
                {
                    UserId = users[1].Id,
                    Username = users[1].UserName!,
                    Email = users[1].Email!,
                    Password = users[1].PasswordHash!
                }
            };

            Mock<IUserDbQueries> userDbQueriesMock = new Mock<IUserDbQueries>();
            userDbQueriesMock.Setup(x => x.GetAllAsync()).ReturnsAsync(users);

            Mock<IUserHelper> userHelperMock = new Mock<IUserHelper>();
            Mock<IMapper> mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<IEnumerable<UserDto>>(users)).Returns(userDtos);

            Mock<ILogger<UserUoW>> loggerMock = new Mock<ILogger<UserUoW>>();

            UserUoW userUoW = new UserUoW(userDbQueriesMock.Object, userHelperMock.Object, mapperMock.Object, loggerMock.Object);

            IEnumerable<UserDto> result = await userUoW.GetUsersAsync();

            List<UserDto> resultList = result.ToList();
            Assert.Equal(2, resultList.Count);

            Assert.Equal(users[0].Id, resultList[0].UserId);
            Assert.Equal(users[0].UserName, resultList[0].Username);
            Assert.Equal(users[0].Email, resultList[0].Email);
            Assert.Equal(users[0].PasswordHash, resultList[0].Password);

            Assert.Equal(users[1].Id, resultList[1].UserId);
        }

        [Fact]
        public async Task GetUsersAsync_WhenNoUsersExist_ReturnsEmptyList()
        {
            // Arrange
            List<User> users = new List<User>(); // empty user list
            List<UserDto> userDtos = new List<UserDto>(); // corresponding empty DTO list

            Mock<IUserDbQueries> userDbQueriesMock = new Mock<IUserDbQueries>();
            userDbQueriesMock.Setup(x => x.GetAllAsync()).ReturnsAsync(users);

            Mock<IUserHelper> userHelperMock = new Mock<IUserHelper>();
            Mock<IMapper> mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<IEnumerable<UserDto>>(users)).Returns(userDtos);

            Mock<ILogger<UserUoW>> loggerMock = new Mock<ILogger<UserUoW>>();

            UserUoW userUoW = new UserUoW(userDbQueriesMock.Object, userHelperMock.Object, mapperMock.Object, loggerMock.Object);

            // Act
            IEnumerable<UserDto> result = await userUoW.GetUsersAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetUsersByIdAsync_WhenUserExists_ReturnsSuccessWithUserDto()
        {
            Guid userId = Guid.NewGuid();

            User user = new User
            {
                Id = userId,
                UserName = "testuser",
                Email = "test@example.com",
                PasswordHash = "hashed"
            };

            UserDto userDto = new UserDto
            {
                UserId = userId,
                Username = "testuser",
                Email = "test@example.com",
                Password = "hashed"
            };

            Mock<IUserDbQueries> userDbQueriesMock = new Mock<IUserDbQueries>();
            userDbQueriesMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);

            Mock<IUserHelper> userHelperMock = new Mock<IUserHelper>();
            Mock<IMapper> mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

            Mock<ILogger<UserUoW>> loggerMock = new Mock<ILogger<UserUoW>>();

            UserUoW userUoW = new UserUoW(userDbQueriesMock.Object, userHelperMock.Object, mapperMock.Object, loggerMock.Object);

            StatusCodeResponseDto<UserDto?> response = await userUoW.GetUsersByIdAsync(userId);

            Assert.Equal(StatusCodeResponse.Success, response.StatusCodeResponse);
            Assert.Equal(userDto, response.Data);
        }

        [Fact]
        public async Task GetUsersByIdAsync_WhenUserDoesNotExist_ReturnsNotFound()
        {
            Guid userId = Guid.NewGuid();

            Mock<IUserDbQueries> userDbQueriesMock = new Mock<IUserDbQueries>();
            userDbQueriesMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync((User?)null);

            Mock<IUserHelper> userHelperMock = new Mock<IUserHelper>();
            Mock<IMapper> mapperMock = new Mock<IMapper>();

            Mock<ILogger<UserUoW>> loggerMock = new Mock<ILogger<UserUoW>>();

            UserUoW userUoW = new UserUoW(userDbQueriesMock.Object, userHelperMock.Object, mapperMock.Object, loggerMock.Object);

            StatusCodeResponseDto<UserDto?> response = await userUoW.GetUsersByIdAsync(userId);

            Assert.Equal(StatusCodeResponse.NotFound, response.StatusCodeResponse);
            Assert.Null(response.Data);
        }

        [Fact]
        public async Task CreateUserAsync_ReturnsCreated_WhenUserIsSuccessfullyCreated()
        {
            CreateUserDto createUserDto = new CreateUserDto
            {
                Username = "newuser",
                Email = "newuser@example.com",
                Password = "SecurePassword123"
            };

            User mappedUser = new User();
            UserPermission userPermission = new UserPermission { Id = Guid.NewGuid() };
            User createdUser = new User { Id = Guid.NewGuid(), UserName = "newuser", UserRightId = userPermission.Id };

            mapperMock.Setup(m => m.Map<User>(createUserDto)).Returns(mappedUser);
            mapperMock.Setup(m => m.Map<UserDto>(It.IsAny<User>())).Returns(new UserDto { Username = "newuser" });

            userDbQueriesMock.Setup(h => h.FindUserByUsername(createUserDto.Username)).ReturnsAsync((User?)null);
            userDbQueriesMock.Setup(h => h.FindUserByEmail(createUserDto.Email)).ReturnsAsync((User?)null);
            userDbQueriesMock.Setup(h => h.GetUserRight()).ReturnsAsync(userPermission);

            userDbQueriesMock.Setup(db => db.CreateAsync(It.IsAny<User>())).ReturnsAsync(createdUser);

            UserUoW userUoW = new UserUoW(userDbQueriesMock.Object, userHelperMock.Object, mapperMock.Object, loggerMock.Object);

            StatusCodeResponseDto<string?> result = await userUoW.CreateUserAsync(createUserDto);

            Assert.Equal(StatusCodeResponse.Created, result.StatusCodeResponse);
            Assert.Contains("User created successfully", result.Message);
        }


        [Fact]
        public async Task CreateUserAsync_ReturnsBadRequest_WhenUsernameOrEmailAlreadyExists()
        {
            CreateUserDto createUserDto = new CreateUserDto
            {
                Username = "takenUser",
                Email = "takenEmail@example.com",
                Password = "pass"
            };

            // 1. Username taken, Email not taken
            userDbQueriesMock.Setup(h => h.FindUserByUsername(createUserDto.Username)).ReturnsAsync(new User());
            userDbQueriesMock.Setup(h => h.FindUserByEmail(createUserDto.Email)).ReturnsAsync((User?)null);

            StatusCodeResponseDto<string?> result1 = await userUoW.CreateUserAsync(createUserDto);
            Assert.Equal(StatusCodeResponse.BadRequest, result1.StatusCodeResponse);
            Assert.Equal("Username is already taken.", result1.Message);
            Assert.Null(result1.Data);

            // 2. Username not taken, Email taken
            userDbQueriesMock.Setup(h => h.FindUserByUsername(createUserDto.Username)).ReturnsAsync((User?)null);
            userDbQueriesMock.Setup(h => h.FindUserByEmail(createUserDto.Email)).ReturnsAsync(new User());

            StatusCodeResponseDto<string?> result2 = await userUoW.CreateUserAsync(createUserDto);
            Assert.Equal(StatusCodeResponse.BadRequest, result2.StatusCodeResponse);
            Assert.Equal("Email is already taken.", result2.Message);
            Assert.Null(result2.Data);

            // 3. Both Username and Email taken
            userDbQueriesMock.Setup(h => h.FindUserByUsername(createUserDto.Username)).ReturnsAsync(new User());
            userDbQueriesMock.Setup(h => h.FindUserByEmail(createUserDto.Email)).ReturnsAsync(new User());

            StatusCodeResponseDto<string?> result3 = await userUoW.CreateUserAsync(createUserDto);
            Assert.Equal(StatusCodeResponse.BadRequest, result3.StatusCodeResponse);
            Assert.Equal("A user with this username and email already exists.", result3.Message);
            Assert.Null(result3.Data);
        }

        [Fact]
        public async Task CreateUserAsync_ReturnsInternalServerError_WhenUserPermissionIsNull()
        {
            CreateUserDto createUserDto = new CreateUserDto
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "password123"
            };

            User mappedUser = new User();

            userDbQueriesMock.Setup(h => h.FindUserByUsername(createUserDto.Username)).ReturnsAsync((User?)null);
            userDbQueriesMock.Setup(h => h.FindUserByEmail(createUserDto.Email)).ReturnsAsync((User?)null);
            userDbQueriesMock.Setup(h => h.GetUserRight()).ReturnsAsync((UserPermission?)null);

            mapperMock.Setup(m => m.Map<User>(createUserDto)).Returns(mappedUser);

            UserUoW userUoW = new UserUoW(userDbQueriesMock.Object, userHelperMock.Object, mapperMock.Object, loggerMock.Object);

            StatusCodeResponseDto<string?> result = await userUoW.CreateUserAsync(createUserDto);

            Assert.Equal(StatusCodeResponse.InternalServerError, result.StatusCodeResponse);
            Assert.Contains("no userRightId", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task LoginUserAsync_ReturnsSuccess_WhenCredentialsAreValid()
        {
            UserUoW userUoW = new UserUoW(userDbQueriesMock.Object, userHelperMock.Object, mapperMock.Object, loggerMock.Object);

            string rawPassword = "CorrectPassword";
            UserDto userDto = new UserDto { Username = "validuser", Password = rawPassword };

            User foundUser = new User { UserName = "validuser" };
            PasswordHasher<User> passwordHasher = new PasswordHasher<User>();
            foundUser.PasswordHash = passwordHasher.HashPassword(foundUser, rawPassword);

            FieldInfo hasherField = typeof(UserUoW).GetField("_passwordHasher", BindingFlags.NonPublic | BindingFlags.Instance)!;
            hasherField.SetValue(userUoW, passwordHasher);

            userDbQueriesMock.Setup(h => h.FindUserByUsername(userDto.Username)).ReturnsAsync(foundUser);
            mapperMock.Setup(m => m.Map<UserDto>(foundUser)).Returns(userDto);

            StatusCodeResponseDto<string?> result = await userUoW.LoginUserAsync(userDto);

            Assert.Equal(StatusCodeResponse.Success, result.StatusCodeResponse);
            Assert.Equal("Login successful", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task LoginUserAsync_ReturnsBadRequest_WhenUsernameIsInvalid()
        {
            UserDto userDto = new UserDto { Username = "unknownuser", Password = "any" };

            userDbQueriesMock.Setup(h => h.FindUserByUsername(userDto.Username)).ReturnsAsync((User?)null);

            UserUoW userUoW = new UserUoW(userDbQueriesMock.Object, userHelperMock.Object, mapperMock.Object, loggerMock.Object);

            StatusCodeResponseDto<string?> result = await userUoW.LoginUserAsync(userDto);

            Assert.Equal(StatusCodeResponse.BadRequest, result.StatusCodeResponse);
            Assert.Equal("Invalid username", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task LoginUserAsync_ReturnsBadRequest_WhenPasswordIsInvalid()
        {
            UserUoW userUoW = new UserUoW(userDbQueriesMock.Object, userHelperMock.Object, mapperMock.Object, loggerMock.Object);

            string correctPassword = "CorrectPassword";
            string wrongPassword = "WrongPassword";

            UserDto userDto = new UserDto { Username = "user1", Password = wrongPassword };

            User foundUser = new User { UserName = "user1" };
            PasswordHasher<User> passwordHasher = new PasswordHasher<User>();
            foundUser.PasswordHash = passwordHasher.HashPassword(foundUser, correctPassword);

            FieldInfo hasherField = typeof(UserUoW).GetField("_passwordHasher", BindingFlags.NonPublic | BindingFlags.Instance)!;
            hasherField.SetValue(userUoW, passwordHasher);

            userDbQueriesMock.Setup(h => h.FindUserByUsername(userDto.Username)).ReturnsAsync(foundUser);

            StatusCodeResponseDto<string?> result = await userUoW.LoginUserAsync(userDto);

            Assert.Equal(StatusCodeResponse.BadRequest, result.StatusCodeResponse);
            Assert.Equal("Invalid password", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task UpdateUserAsync_ReturnsSuccess_WhenUserExistsAndValidationPasses()
        {
            Guid userId = Guid.NewGuid();
            UpdateUserDto updateUserDto = new UpdateUserDto { Username = "updateduser", Email = "updated@example.com" };
            User existingUser = new User { Id = userId, UserName = "olduser", Email = "old@example.com" };
            User updatedUser = new User { Id = userId, UserName = "updateduser", Email = "updated@example.com" };
            UserDto mappedUserDto = new UserDto { Username = "updateduser", Email = "updated@example.com" };

            userDbQueriesMock.Setup(d => d.GetByIdAsync(userId)).ReturnsAsync(existingUser);
            userDbQueriesMock.Setup(h => h.FindUserByUsername(updateUserDto.Username)).ReturnsAsync((User?)null);
            userDbQueriesMock.Setup(h => h.FindUserByEmail(updateUserDto.Email)).ReturnsAsync((User?)null);
            mapperMock.Setup(m => m.Map(updateUserDto, existingUser)).Returns(updatedUser);
            userDbQueriesMock.Setup(d => d.UpdateAsync(existingUser)).ReturnsAsync(updatedUser);
            mapperMock.Setup(m => m.Map<UserDto>(updatedUser)).Returns(mappedUserDto);

            UserUoW userUoW = new UserUoW(userDbQueriesMock.Object, userHelperMock.Object, mapperMock.Object, loggerMock.Object);

            StatusCodeResponseDto<UserDto?> result = await userUoW.UpdateUserAsync(userId, updateUserDto);

            Assert.Equal(StatusCodeResponse.Success, result.StatusCodeResponse);
            Assert.Equal("User with ID: {userId} updated successfully", result.Message);
            Assert.Equal("updateduser", result.Data?.Username);
            Assert.Equal("updated@example.com", result.Data?.Email);
        }

        [Fact]
        public async Task UpdateUserAsync_ReturnsNotFound_WhenUserDoesNotExist()
        {
            Guid userId = Guid.NewGuid();
            UpdateUserDto updateUserDto = new UpdateUserDto { Username = "any", Email = "any@example.com" };

            userDbQueriesMock.Setup(d => d.GetByIdAsync(userId)).ReturnsAsync((User?)null);

            UserUoW userUoW = new UserUoW(userDbQueriesMock.Object, userHelperMock.Object, mapperMock.Object, loggerMock.Object);

            StatusCodeResponseDto<UserDto?> result = await userUoW.UpdateUserAsync(userId, updateUserDto);

            Assert.Equal(StatusCodeResponse.NotFound, result.StatusCodeResponse);
            Assert.Equal($"User with ID: {userId} not found for update", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task UpdateUserAsync_ReturnsBadRequest_WhenUsernameOrEmailNotUnique()
        {
            Guid userId = Guid.NewGuid();
            UpdateUserDto updateUserDto = new UpdateUserDto { Username = "takenuser", Email = "taken@example.com" };
            User existingUser = new User { Id = userId, UserName = "olduser", Email = "old@example.com" };

            userDbQueriesMock.Setup(d => d.GetByIdAsync(userId)).ReturnsAsync(existingUser);
            userDbQueriesMock.Setup(h => h.FindUserByUsername(updateUserDto.Username)).ReturnsAsync(new User { Id = Guid.NewGuid() });

            UserUoW userUoW = new UserUoW(userDbQueriesMock.Object, userHelperMock.Object, mapperMock.Object, loggerMock.Object);

            StatusCodeResponseDto<UserDto?> result = await userUoW.UpdateUserAsync(userId, updateUserDto);

            Assert.Equal(StatusCodeResponse.BadRequest, result.StatusCodeResponse);
            Assert.Equal("Username is already taken.", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task DeleteUserAsync_WhenUserExists_ReturnsSuccessResponse()
        {
            Guid userId = Guid.NewGuid();

            Mock<IUserDbQueries> userDbQueriesMock = new Mock<IUserDbQueries>();
            userDbQueriesMock.Setup(x => x.DeleteAsync(userId)).ReturnsAsync(true);

            Mock<IUserHelper> userHelperMock = new Mock<IUserHelper>();
            Mock<IMapper> mapperMock = new Mock<IMapper>();
            Mock<ILogger<UserUoW>> loggerMock = new Mock<ILogger<UserUoW>>();

            UserUoW userUoW = new UserUoW(userDbQueriesMock.Object, userHelperMock.Object, mapperMock.Object, loggerMock.Object);

            StatusCodeResponseDto<bool> result = await userUoW.DeleteUserAsync(userId);

            Assert.Equal(StatusCodeResponse.Success, result.StatusCodeResponse);
            Assert.Equal($"User with ID: {userId} deleted successfully", result.Message);
            Assert.True(result.Data);
        }

        [Fact]
        public async Task DeleteUserAsync_WhenUserDoesNotExist_ReturnsNotFoundResponse()
        {
            Guid userId = Guid.NewGuid();

            Mock<IUserDbQueries> userDbQueriesMock = new Mock<IUserDbQueries>();
            userDbQueriesMock.Setup(x => x.DeleteAsync(userId)).ReturnsAsync(false);

            Mock<IUserHelper> userHelperMock = new Mock<IUserHelper>();
            Mock<IMapper> mapperMock = new Mock<IMapper>();
            Mock<ILogger<UserUoW>> loggerMock = new Mock<ILogger<UserUoW>>();

            UserUoW userUoW = new UserUoW(userDbQueriesMock.Object, userHelperMock.Object, mapperMock.Object, loggerMock.Object);

            StatusCodeResponseDto<bool> result = await userUoW.DeleteUserAsync(userId);

            Assert.Equal(StatusCodeResponse.NotFound, result.StatusCodeResponse);
            Assert.Equal($"User with ID: {userId} not found for deletion", result.Message);
            Assert.False(result.Data);
        }
    }
}
