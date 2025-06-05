using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Routeplanner_API.DTO.User;
using Routeplanner_API.Helpers;
using Routeplanner_API.JWT;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Test_API.Helpers_Tests
{
    public class UserHelperTests
    {
        private readonly Mock<ILogger<UserHelper>> _loggerMock = new();

        [Fact]
        public void GenerateUserJwtToken_Returns_Valid_JwtToken()
        {
            // Arrange
            var userHelper = CreateUserHelper();
            var user = new UserDto
            {
                UserId = Guid.NewGuid(),
                Username = "testuser",
                Email = "test@example.com",
                Password = "password123"
            };

            // Act
            string tokenString = userHelper.GenerateUserJwtToken(user);

            // Assert
            Assert.False(string.IsNullOrEmpty(tokenString));

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenString);

            Assert.Equal(_jwtSettings.Issuer, token.Issuer);
            Assert.Contains(_jwtSettings.Audience, token.Audiences);

            var nameIdClaim = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            Assert.NotNull(nameIdClaim);
            Assert.Equal(user.UserId.ToString(), nameIdClaim.Value);

            var roleClaim = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            Assert.NotNull(roleClaim);
            Assert.Equal("User", roleClaim.Value);

            var jtiClaim = token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti);
            Assert.NotNull(jtiClaim);

            Assert.True(token.ValidTo > DateTime.UtcNow);
            Assert.True(token.ValidTo <= DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes + 1));
        }

        [Fact]
        public void Constructor_Throws_ArgumentNullException_When_JwtOptions_Null()
        {
            // Arrange
            IOptions<JwtSettings> nullOptions = null;

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new UserHelper(nullOptions, _loggerMock.Object));
            Assert.Equal("jwtOptions", ex.ParamName);
        }

        [Fact]
        public void Constructor_Throws_ArgumentNullException_When_Logger_Null()
        {
            var optionsMock = new Mock<IOptions<JwtSettings>>();
            optionsMock.Setup(x => x.Value).Returns(_jwtSettings);

            ILogger<UserHelper> nullLogger = null;

            Assert.Throws<ArgumentNullException>(() => new UserHelper(optionsMock.Object, nullLogger));
        }

        private UserHelper CreateUserHelper()
        {
            var optionsMock = new Mock<IOptions<JwtSettings>>();
            optionsMock.Setup(x => x.Value).Returns(_jwtSettings);

            return new UserHelper(optionsMock.Object, _loggerMock.Object);
        }

        private readonly JwtSettings _jwtSettings = new JwtSettings
        {
            Secret = "ThisIsASecretKeyForTesting1234567890", // must be sufficiently long
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpiryMinutes = 60
        };
    }
}
