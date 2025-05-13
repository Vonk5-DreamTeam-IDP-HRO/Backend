using Microsoft.IdentityModel.Tokens;
using Routeplanner_API.DTO;
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

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: "tempdomain.com",
                audience: "tempdomain.com",
                claims: claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateAdminJwtToken(UserDto user)
        {
            Claim[] claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Admin") // 👈 Admin role here
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("super_secret_key"));
            SigningCredentials credentials = new (key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: "tempdomain.com",
                audience: "tempdomain.com",
                claims: claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
