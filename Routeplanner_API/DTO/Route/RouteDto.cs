namespace Routeplanner_API.DTO.Route
{
    /// <summary>
    /// Data Transfer Object representing a Route.
    /// </summary>
    public sealed class RouteDto
    {
        /// <summary>
        /// Gets the unique identifier of the route.
        /// </summary>
        public Guid RouteId { get; init; }

        /// <summary>
        /// Gets or sets the name of the route.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the optional description of the route.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets the user ID of the creator of the route.
        /// </summary>
        public Guid? CreatedBy { get; init; }

        /// <summary>
        /// Gets or sets whether the route is private.
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
        /// Gets or sets the collection of location IDs associated with this route.
        /// </summary>
        public ICollection<Guid> LocationIds { get; set; }
    }
}
