using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Routeplanner_API.JWT;
using System;

namespace Routeplanner_API.Extensions
{
    public static class JwtSettingsExtensions
    {
        public static JwtSettings GetValidatedJwtSettings(this IConfiguration configuration, ILogger logger, string sectionName = "Jwt")
        {
            ArgumentNullException.ThrowIfNull(configuration);
            ArgumentNullException.ThrowIfNull(logger);

            logger.LogDebug("Binding JwtSettings from configuration section '{Section}'", sectionName);

            JwtSettings jwtSettings = new JwtSettings();
            var section = configuration.GetSection(sectionName);
            section.Bind(jwtSettings);

            if (string.IsNullOrWhiteSpace(jwtSettings.Secret))
            {
                logger.LogError("JWT Secret is missing or empty in configuration.");
                throw new InvalidOperationException("JWT Secret must be provided.");
            }

            if (string.IsNullOrWhiteSpace(jwtSettings.Issuer))
            {
                logger.LogError("JWT Issuer is missing or empty in configuration.");
                throw new InvalidOperationException("JWT Issuer must be provided.");
            }

            if (string.IsNullOrWhiteSpace(jwtSettings.Audience))
            {
                logger.LogError("JWT Audience is missing or empty in configuration.");
                throw new InvalidOperationException("JWT Audience must be provided.");
            }

            if (jwtSettings.ExpiryMinutes == null || jwtSettings.ExpiryMinutes <= DateTime.MinValue)
            {
                logger.LogWarning("JWT ExpiryMinutes not set or invalid. Defaulting to 180 minutes from now.");
                jwtSettings.ExpiryMinutes = DateTime.UtcNow.AddMinutes(180);
            }

            logger.LogInformation("JwtSettings successfully validated.");

            return jwtSettings;
        }
    }
}
