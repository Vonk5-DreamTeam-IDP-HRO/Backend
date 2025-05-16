using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using DotNetEnv;

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

            logger.LogDebug("Attempting to retrieve connection string with key: {Key} using IConfiguration.GetConnectionString", key);

            // Use IConfiguration.GetConnectionString, which looks for "ConnectionStrings:<key>"
            var connectionString = configuration.GetConnectionString(key);

            if (string.IsNullOrEmpty(connectionString))
            {
                logger.LogWarning("Primary connection string '{Key}' not found using IConfiguration.GetConnectionString, attempting to use fallback 'ThijsHROConnection'", key);
                // The fallback key for GetConnectionString should also be the simple name, e.g., "ThijsHROConnection"
                var temporaryConnectionString = configuration.GetConnectionString("ThijsHROConnection");

                if (string.IsNullOrEmpty(temporaryConnectionString))
                {
                    logger.LogError("Both primary ('{Key}') and fallback ('ThijsHROConnection') connection strings are missing from IConfiguration.", key);
                    throw new InvalidOperationException($"Database Connection string '{key}' or fallback 'ThijsHROConnection' not found or is empty in IConfiguration.");
                }

                logger.LogInformation("Using fallback connection string 'ThijsHROConnection' from IConfiguration");
                return temporaryConnectionString;
            }

            logger.LogInformation("Successfully retrieved connection string for key: {Key} from IConfiguration", key);
            return connectionString;
        }
    }
}
