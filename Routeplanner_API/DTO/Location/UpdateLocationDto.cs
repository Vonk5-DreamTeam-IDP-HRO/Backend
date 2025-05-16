using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace Routeplanner_API.DTO.Location
{
    public sealed class UpdateLocationDto
    {
        [Required]
        public Guid LocationId { get; init; }

        [Required]
        [StringLength(255)]
        public string? Name { get; set; }

        [Required]
        [Range(51.80, 52.00)]
        public double? Latitude { get; set; }

        [Required]
        [Range(4.40, 4.60)]
        public double? Longitude { get; set; }

        public string? Description { get; set; }
    }
}
