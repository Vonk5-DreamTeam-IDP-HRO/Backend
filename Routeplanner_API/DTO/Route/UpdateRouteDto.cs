using Routeplanner_API.Models;
using System.ComponentModel.DataAnnotations;

namespace Routeplanner_API.DTO.Route
{
    public sealed class UpdateRouteDto
    {
        [StringLength(255)]
        public Guid RouteId { get; init; }
        public string Name { get; set; } = string.Empty;
        [StringLength(1000)]
        public string? Description { get; set; }
        public bool? IsPrivate { get; set; }
        [Required, MinLength(2)]
        public ICollection<Guid> LocationIds { get; set; }
    }
}
