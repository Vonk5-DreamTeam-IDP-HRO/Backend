using System;

namespace Routeplanner_API.DTO.Route
{
    public sealed class RouteDto
    {
        public Guid RouteId { get; init; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid? CreatedBy { get; init; }
        public bool? IsPrivate { get; set; }
        public DateTime? CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; set; }
    }
}
