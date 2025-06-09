using System.ComponentModel.DataAnnotations;

namespace Routeplanner_API.DTO.Route
{
    /// <summary>
    /// DTO for updating an existing Route.
    /// </summary>
    public sealed class UpdateRouteDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the route.
        /// </summary>
        public Guid RouteId { get; init; }

        /// <summary>
        /// Gets or sets the name of the route.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the route.
        /// </summary>
        [StringLength(1000)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the route is private.
        /// </summary>
        public bool? IsPrivate { get; set; }

        /// <summary>
        /// Gets or sets the collection of location IDs associated with the route.
        /// </summary>
        [Required, MinLength(2)]
        public ICollection<Guid> LocationIds { get; set; }
    }
}
