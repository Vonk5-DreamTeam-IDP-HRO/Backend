using Routeplanner_API.Models;
using System.ComponentModel.DataAnnotations;

namespace Routeplanner_API.DTO.Route
{
    public sealed class CreateRouteDto
    {
        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;
        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        public bool IsPrivate { get; set; } = true;

        // CreatedBy will be set via the service layer from the authenticated user (which is a Guid)
        public Guid? CreatedBy { get; set; }

        [Required, MinLength(2)] 
        public required ICollection<Guid> LocationIds { get; set; } = new List<Guid>();
    }
}
