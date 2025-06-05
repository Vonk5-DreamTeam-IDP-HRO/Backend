using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Routeplanner_API.Database_Queries;
using Routeplanner_API.DTO;
using Routeplanner_API.DTO.User;
using Routeplanner_API.Enums;
using Routeplanner_API.Helpers;
using Routeplanner_API.Models;
using Routeplanner_API.UoWs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_API.UoWs_Tests
{
    public class UserUoWTests
    {
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

        // To do: Create user, Login, Update

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
