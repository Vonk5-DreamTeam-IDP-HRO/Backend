using System;

namespace Routeplanner_API.DTO
{
    public class RouteDto
    {
        public int RouteId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int? CreatedBy { get; set; }
        public bool? IsPrivate { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}