using Routeplanner_API.Models;
using System.ComponentModel.DataAnnotations;

namespace Routeplanner_API.DTO.Location
{
    public sealed class CreateLocationDto
    {
        [Required]
        [StringLength(255)]
        public string Name { get; init; } = string.Empty;

        // Latitude and Longitude are set to Rotterdam + all deelgemeentes.
        // THey are both given as double, but they cannot be nullable.
        [Required]
        [Range(51.80, 52.00)]
        public double Latitude { get; init; }

        [Required]
        [Range(4.40, 4.60)]
        public double Longitude { get; init; }

        public string? Description { get; init; }

        [Required]
        public Guid UserId { get; init; }

        public CreateLocationDetailDto? LocationDetail { get; init; }
    }
}
