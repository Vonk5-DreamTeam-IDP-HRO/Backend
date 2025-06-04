using Routeplanner_API.Models;
using System.ComponentModel.DataAnnotations;

namespace Routeplanner_API.DTO.Route
{
    public sealed class UpdateRouteDto
    {
        [StringLength(255)]
        public Guid RouteId { get; init; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool? IsPrivate { get; set; }

        public required ICollection<LocationRoute> LocationRoutes { get; set; }
    }
}
