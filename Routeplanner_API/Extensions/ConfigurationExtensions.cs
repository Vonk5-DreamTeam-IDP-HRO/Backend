using Microsoft.Extensions.Configuration;
using System;

namespace Routeplanner_API.Extensions
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Gets the connection string for the specified key from configuration and throws an exception if it's null or empty.
        /// </summary>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="key">The connection string key (defaults to "DefaultConnection").</param>
        /// <returns>The validated connection string.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the connection string is not found or is empty.</exception>
        public static string GetValidatedConnectionString(this IConfiguration configuration, string key = "DefaultConnection")
        {
            ArgumentNullException.ThrowIfNull(configuration); // Ensure configuration itself isn't null

            var connectionString = configuration.GetConnectionString(key);

            if (string.IsNullOrEmpty(connectionString))
            {
                // TODO: Consider injecting and using ILogger here for better diagnostics
                throw new InvalidOperationException($"Database Connection string '{key}' not found or is empty in configuration.");
            }

            return connectionString;
        }
    }
}
