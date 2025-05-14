using Microsoft.IdentityModel.Tokens;
using Routeplanner_API.DTO;
using Routeplanner_API.JWT;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Routeplanner_API.Helpers
{
    public class UserHelper : IUserHelper
    {       
        private readonly ILogger<UserHelper> _logger;

        public UserHelper(ILogger<UserHelper> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string GenerateUserJwtToken(UserDto user)
        {
            Claim[] claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "User")
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("super_secret_key"));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSettings jwtSetting = CreateJwtSettingsObject();

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: jwtSetting.Issuer,
                audience: jwtSetting.Audience,
                claims: claims,
                expires: jwtSetting.ExpiryMinutes,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateAdminJwtToken(UserDto user)
        {
            Claim[] claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Admin") 
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("super_secret_key"));
            SigningCredentials credentials = new (key, SecurityAlgorithms.HmacSha256);

            JwtSettings jwtSetting = CreateJwtSettingsObject();

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: jwtSetting.Issuer,
                audience: jwtSetting.Audience,
                claims: claims,
                expires: jwtSetting.ExpiryMinutes,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private JwtSettings CreateJwtSettingsObject()
        {
            return new JwtSettings()
            {
                Secret = "to do", 
                Issuer = "RoutplannerAPI.com",
                Audience = "RoutplannerAPI.com",
                ExpiryMinutes = DateTime.Now.AddHours(3),
            };
        }
    }
}
