namespace Routeplanner_API.DTO.Location
{
    /// <summary>
    /// Data transfer object representing a location.
    /// </summary>
    public sealed class LocationDto
    {
        /// <summary>
        /// Gets the unique identifier for the location.
        /// </summary>
        public Guid LocationId { get; init; }

        /// <summary>
        /// Gets the name of the location.
        /// </summary>
        public string Name { get; init; } = string.Empty;

        /// <summary>
        /// Gets the latitude coordinate of the location.
        /// </summary>
        public double Latitude { get; init; }

        /// <summary>
        /// Gets the longitude coordinate of the location.
        /// </summary>
        public double Longitude { get; init; }

        /// <summary>
        /// Gets an optional description of the location.
        /// </summary>
        public string? Description { get; init; }

        /// <summary>
        /// Gets or sets detailed information about the location.
        /// </summary>
        public LocationDetailDto? LocationDetail { get; set; }
    }
}
