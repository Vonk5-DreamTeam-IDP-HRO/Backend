namespace Routeplanner_API.Models
{
    /// <summary>
    /// Represents a route that consists of one or more locations.
    /// </summary>
    public partial class Route
    {
        /// <summary>
        /// Gets the unique identifier for the route.
        /// </summary>
        public Guid RouteId { get; init; }

        /// <summary>
        /// Gets or sets the name of the route.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets an optional description of the route.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets the identifier of the user who created the route.
        /// </summary>
        public Guid? CreatedBy { get; init; }

        /// <summary>
        /// Indicates whether the route is private.
        /// </summary>
        public bool? IsPrivate { get; set; }

        /// <summary>
        /// Gets the date and time when the route was created.
        /// </summary>
        public DateTime? CreatedAt { get; init; }

        /// <summary>
        /// Gets or sets the date and time when the route was last updated.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the user who created the route.
        /// </summary>
        public virtual User? CreatedByNavigation { get; set; }

        /// <summary>
        /// Gets or sets the collection of location-route relationships associated with this route.
        /// </summary>
        public virtual ICollection<LocationRoute> LocationRoutes { get; set; } = new List<LocationRoute>();
    }
}
