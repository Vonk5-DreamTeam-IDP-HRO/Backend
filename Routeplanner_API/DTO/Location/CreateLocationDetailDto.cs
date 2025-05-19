using System.ComponentModel.DataAnnotations;

namespace Routeplanner_API.DTO.Location
{
    public sealed class CreateLocationDetailDto
    {
        [StringLength(255)]
        public string? Address { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }

        [StringLength(20)]
        public string? ZipCode { get; set; }

        [StringLength(20)]
        [Phone]
        public string? PhoneNumber { get; set; }

        [StringLength(2048)]
        [Url]
        public string? Website { get; set; }

        [StringLength(100)]
        public string? Category { get; set; }

        [StringLength(500)]
        public string? Accessibility { get; set; }
    }
}
