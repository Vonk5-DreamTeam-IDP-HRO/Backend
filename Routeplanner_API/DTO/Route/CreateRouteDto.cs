using System.ComponentModel.DataAnnotations;

namespace Routeplanner_API.DTO.Route
{
    public class CreateRouteDto
    {
        [Required]
        [StringLength(255)]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        // IsPrivate is optional with default value
        public bool IsPrivate { get; set; } = false;

        // CreatedBy can be set via the service layer from the authenticated user
        // or explicitly provided in certain scenarios
        public int? CreatedBy { get; set; }
    }
}
