using System.ComponentModel.DataAnnotations;

namespace Routeplanner_API.DTO
{
    public class CreateLocationDto
    {
        [Required]
        [StringLength(255)]
        public string Name { get; set; } = null!;

        [Required]
        [Range(-90, 90)] // Standard latitude range
        public double Latitude { get; set; }

        [Required]
        [Range(-180, 180)] // Standard longitude range
        public double Longitude { get; set; }

        public string? Description { get; set; }

        // Optional: If a user creates a location, you might want to include UserId
        // public int? UserId { get; set; } 
    }
}
