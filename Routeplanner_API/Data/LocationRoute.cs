namespace Routeplanner_API.Models
{
    /// <summary>
    /// Represents the many-to-many relationship between locations and routes.
    /// </summary>
    public partial class LocationRoute
    {
        /// <summary>
        /// Gets the unique identifier for the location-route relationship.
        /// </summary>
        public Guid LocationRouteId { get; init; }

        /// <summary>
        /// Gets or sets the identifier of the associated route.
        /// </summary>
        public Guid RouteId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the associated location.
        /// </summary>
        public Guid LocationId { get; set; }

        /// <summary>
        /// Gets or sets the location associated with this relationship.
        /// </summary>
        public virtual Location Location { get; set; } = null!;

        /// <summary>
        /// Gets or sets the route associated with this relationship.
        /// </summary>
        public virtual Route Route { get; set; } = null!;
    }
}
