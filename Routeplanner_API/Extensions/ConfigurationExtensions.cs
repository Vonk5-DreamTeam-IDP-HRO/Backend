using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace Routeplanner_API.Extensions
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Gets the connection string for the specified key from configuration and throws an exception if it's null or empty.
        /// </summary>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="key">The connection string key (defaults to "DefaultConnection").</param>
        /// <returns>The validated connection string.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the connection string is not found or is empty.</exception>
        public static string GetValidatedConnectionString(this IConfiguration configuration, ILogger logger, string key = "DefaultConnection")
        {
            ArgumentNullException.ThrowIfNull(configuration);
            ArgumentNullException.ThrowIfNull(logger);

            const string connectionStringText = "ConnectionStrings";
            logger.LogDebug("Attempting to retrieve connection string with key: {Key}", key);

            var connectionString = configuration.GetConnectionString((connectionStringText + key));

            if (string.IsNullOrEmpty(connectionString))
            {
                logger.LogWarning("Primary connection string '{Key}' not found, attempting to use fallback connection", key);
                var temporaryConnectionString = configuration[$"{connectionStringText}:ThijsHROConnection"];

                if (string.IsNullOrEmpty(temporaryConnectionString))
                {
                    logger.LogError("Both primary and fallback connection strings are missing");
                    throw new InvalidOperationException($"Database Connection string '{key}' not found or is empty in configuration.");
                }

                logger.LogInformation("Using fallback connection string 'ThijsHROConnection'");
                return temporaryConnectionString;
            }

            logger.LogInformation("Successfully retrieved connection string for key: {Key}", key);
            return connectionString;
        }
    }
}
