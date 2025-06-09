using Routeplanner_API.JWT;

namespace Routeplanner_API.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="IConfiguration"/> to retrieve and validate JWT settings.
    /// </summary>
    public static class JwtSettingsExtensions
    {
        /// <summary>
        /// Retrieves and validates JWT settings from configuration, throwing exceptions if required values are missing or invalid.
        /// </summary>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="logger">The logger instance.</param>
        /// <returns>A validated <see cref="JwtSettings"/> instance.</returns>
        /// <exception cref="InvalidOperationException">Thrown when required JWT settings are missing or invalid.</exception>
        public static JwtSettings GetValidatedJwtSettings(this IConfiguration configuration, ILogger logger)
        {
            ArgumentNullException.ThrowIfNull(configuration);
            ArgumentNullException.ThrowIfNull(logger);

            var jwtSecret = configuration["Jwt__Secret"] ?? configuration["Jwt:Secret"];
            var jwtIssuer = configuration["Jwt__Issuer"] ?? configuration["Jwt:Issuer"];
            var jwtAudience = configuration["Jwt__Audience"] ?? configuration["Jwt:Audience"];
            var expiryMinutesString = configuration["Jwt__ExpiryMinutes"] ?? configuration["Jwt:ExpiryMinutes"];
            logger.LogDebug("Attempting to retrieve JWT settings from configuration.");

            if (string.IsNullOrWhiteSpace(jwtSecret))
            {
                logger.LogError("JWT Secret is missing or empty in configuration.");
                throw new InvalidOperationException("JWT Secret must be provided.");
            }

            if (string.IsNullOrWhiteSpace(jwtIssuer))
            {
                logger.LogError("JWT Issuer is missing or empty in configuration.");
                throw new InvalidOperationException("JWT Issuer must be provided.");
            }

            if (string.IsNullOrWhiteSpace(jwtAudience))
            {
                logger.LogError("JWT Audience is missing or empty in configuration.");
                throw new InvalidOperationException("JWT Audience must be provided.");
            }

            int expiryMinutes;
            if (!int.TryParse(expiryMinutesString, out expiryMinutes) || expiryMinutes <= 0)
            {
                logger.LogWarning("JWT ExpiryMinutes not set or invalid. Defaulting to 180 minutes.");
                expiryMinutes = 180;
            }

            logger.LogInformation("JwtSettings successfully validated.");

            return new JwtSettings
            {
                Secret = jwtSecret,
                Issuer = jwtIssuer,
                Audience = jwtAudience,
                ExpiryMinutes = expiryMinutes
            };
        }
    }
}
