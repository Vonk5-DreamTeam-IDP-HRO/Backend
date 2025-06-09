using System.ComponentModel.DataAnnotations;

namespace Routeplanner_API.DTO.Route
{
    /// <summary>
    /// Data Transfer Object for creating a new Route.
    /// </summary>
    public sealed class CreateRouteDto
    {
        /// <summary>
        /// Gets or sets the name of the route.
        /// </summary>
        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the optional description of the route.
        /// </summary>
        [StringLength(1000)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets whether the route is private.
        /// Defaults to true.
        /// </summary>
        [Required]
        public bool IsPrivate { get; set; } = true;

        /// <summary>
        /// Gets or sets the user ID of the creator.
        /// CreatedBy will be set via the service layer from the authenticated user (which is a Guid)
        /// </summary>
        public Guid? CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the collection of location IDs that make up the route.
        /// Must contain at least two locations.
        /// </summary>
        [Required]
        [MinLength(2)]
        public required ICollection<Guid> LocationIds { get; set; } = new List<Guid>();
    }
}
