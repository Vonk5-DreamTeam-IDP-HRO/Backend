using System.ComponentModel.DataAnnotations;

namespace Routeplanner_API.DTO.Location
{
    /// <summary>
    /// Data transfer object for creating detailed information about a location.
    /// </summary>
    public sealed class CreateLocationDetailDto
    {
        /// <summary>
        /// Gets or sets the address of the location.
        /// </summary>
        [StringLength(255)]
        public string? Address { get; set; }

        /// <summary>
        /// Gets or sets the city of the location.
        /// </summary>
        [StringLength(100)]
        public string? City { get; set; }

        /// <summary>
        /// Gets or sets the country of the location.
        /// </summary>
        [StringLength(100)]
        public string? Country { get; set; }

        /// <summary>
        /// Gets or sets the zip code of the location.
        /// </summary>
        [StringLength(20)]
        public string? ZipCode { get; set; }

        /// <summary>
        /// Gets or sets the phone number of the location.
        /// </summary>
        [StringLength(20)]
        [Phone]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the website URL of the location.
        /// </summary>
        [StringLength(2048)]
        [Url]
        public string? Website { get; set; }

        /// <summary>
        /// Gets or sets the category of the location.
        /// </summary>
        [StringLength(100)]
        public string? Category { get; set; }

        /// <summary>
        /// Gets or sets the accessibility information of the location.
        /// </summary>
        [StringLength(500)]
        public string? Accessibility { get; set; }
    }
}
