using System.ComponentModel.DataAnnotations;

namespace Routeplanner_API.DTO.Location
{
    /// <summary>
    /// Data transfer object for creating a new location.
    /// </summary>
    public sealed class CreateLocationDto
    {
        /// <summary>
        /// Gets the name of the location.
        /// </summary>
        [Required]
        [StringLength(255)]
        public string Name { get; init; } = string.Empty;

        /// <summary>
        /// Gets the latitude of the location.
        /// Latitude is constrained to Rotterdam and all deelgemeentes (51.80 to 52.00).
        /// </summary>
        [Required]
        [Range(51.80, 52.00)]
        public double Latitude { get; init; }

        /// <summary>
        /// Gets the longitude of the location.
        /// Longitude is constrained to Rotterdam and all deelgemeentes (4.40 to 4.60).
        /// </summary>
        [Required]
        [Range(4.40, 4.60)]
        public double Longitude { get; init; }

        /// <summary>
        /// Gets an optional description of the location.
        /// </summary>
        public string? Description { get; init; }

        /// <summary>
        /// Gets optional detailed information about the location.
        /// </summary>
        public CreateLocationDetailDto? LocationDetail { get; init; }
    }
}
