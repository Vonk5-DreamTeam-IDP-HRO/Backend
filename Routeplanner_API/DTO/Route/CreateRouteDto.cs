using Routeplanner_API.Models;
using System.ComponentModel.DataAnnotations;

namespace Routeplanner_API.DTO.Route
{
    public sealed class CreateRouteDto
    {
        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public bool IsPrivate { get; set; } = true;

        // CreatedBy will be set via the service layer from the authenticated user (which is a Guid)
        public Guid? CreatedBy { get; set; }

        [Required] 
        public required ICollection<LocationRoute> LocationRoutes { get; set; }
    }
}
