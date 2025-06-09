namespace Routeplanner_API.Models
{
    /// <summary>
    /// Represents a geographic location, including coordinates and related metadata.
    /// </summary>
    public partial class Location
    {
        /// <summary>
        /// Gets the unique identifier for the location.
        /// </summary>
        public Guid LocationId { get; init; }

        /// <summary>
        /// Gets the identifier of the user associated with the location, if any.
        /// </summary>
        public Guid? UserId { get; init; }

        /// <summary>
        /// Gets or sets the name of the location.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the latitude coordinate of the location.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude coordinate of the location.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Gets or sets the optional description of the location.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets the date and time when the location was created.
        /// </summary>
        public DateTime? CreatedAt { get; init; }

        /// <summary>
        /// Gets or sets the date and time when the location was last updated.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the detailed information associated with the location.
        /// </summary>
        public virtual LocationDetail? LocationDetail { get; set; }

        /// <summary>
        /// Gets or sets the collection of routes associated with this location.
        /// </summary>
        public virtual ICollection<LocationRoute> LocationRoutes { get; set; } = new List<LocationRoute>();

        /// <summary>
        /// Gets or sets the collection of opening times for this location.
        /// </summary>
        public virtual ICollection<OpeningTime> OpeningTimes { get; set; } = new List<OpeningTime>();

        /// <summary>
        /// Gets or sets the user associated with the location.
        /// </summary>
        public virtual User? User { get; set; }
    }
}
