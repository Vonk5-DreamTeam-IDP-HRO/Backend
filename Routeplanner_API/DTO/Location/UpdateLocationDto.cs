using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace Routeplanner_API.DTO.Location
{
    public class UpdateLocationDto
    {
        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(-90, 90)]
        public double Latitude { get; set; }

        [Required]
        [Range(-180, 180)]
        public double? Longitude { get; set; }

        public string? Description { get; set; }

        // Note: Typically, you wouldn't update UserId this way.
        // If UserId needs to be changed, it might be a separate, more privileged operation.
    }
}
