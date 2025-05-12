using System.ComponentModel.DataAnnotations;

namespace Routeplanner_API.DTO.Route
{
    public class UpdateRouteDto
    {
        [StringLength(255)]
        public string? Name { get; set; }

        public string? Description { get; set; }
        public bool IsPrivate { get; set; }
    }
}
