namespace Routeplanner_API.JWT
{
    /// <summary>
    /// Represents the configuration settings for generating and validating JWT tokens.
    /// </summary>
    public class JwtSettings
    {
        /// <summary>
        /// Gets or sets the secret key used to sign the JWT token.
        /// </summary>
        public string Secret { get; set; } = null!;

        /// <summary>
        /// Gets or sets the issuer of the JWT token.
        /// </summary>
        public string Issuer { get; set; } = null!;

        /// <summary>
        /// Gets or sets the intended audience for the JWT token.
        /// </summary>
        public string Audience { get; set; } = null!;

        /// <summary>
        /// Gets or sets the expiration time (in minutes) for the JWT token.
        /// </summary>
        public int ExpiryMinutes { get; set; }
    }
}
