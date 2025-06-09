using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace Routeplanner_API.DTO.Location
{
    /// <summary>
    /// Data transfer object for updating an existing location.
    /// </summary>
    public sealed class UpdateLocationDto
    {
        /// <summary>
        /// Gets the unique identifier of the location to update.
        /// </summary>
        [Required]
        public Guid LocationId { get; init; }

        /// <summary>
        /// Gets or sets the name of the location.
        /// </summary>
        [Required]
        [StringLength(255)]
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the latitude coordinate of the location.
        /// </summary>
        [Required]
        [Range(51.80, 52.00)]
        public double? Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude coordinate of the location.
        /// </summary>
        [Required]
        [Range(4.40, 4.60)]
        public double? Longitude { get; set; }

        /// <summary>
        /// Gets or sets an optional description of the location.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the detailed information of the location.
        /// </summary>
        public LocationDetailDto? LocationDetail { get; set; }
    }
}
