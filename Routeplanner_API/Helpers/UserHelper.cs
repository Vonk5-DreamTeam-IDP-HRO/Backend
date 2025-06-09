using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Routeplanner_API.DTO.User;
using Routeplanner_API.JWT;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Routeplanner_API.Helpers
{
    /// <summary>
    /// Provides helper methods related to user authentication and JWT token generation.
    /// </summary>
    public class UserHelper : IUserHelper
    {
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<UserHelper> _logger;

        public UserHelper(IOptions<JwtSettings> jwtOptions, ILogger<UserHelper> logger)
        {
            if (jwtOptions == null)
            {
                throw new ArgumentNullException(nameof(jwtOptions));
            }
            _jwtSettings = jwtOptions.Value ?? throw new ArgumentNullException(nameof(jwtOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Generates a JWT token for the specified user.
        /// </summary>
        /// <param name="user">The user DTO containing user information.</param>
        /// <returns>A JWT token string.</returns>
        public string GenerateUserJwtToken(UserDto user)
        {
            Claim[] claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Role, "User")
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
