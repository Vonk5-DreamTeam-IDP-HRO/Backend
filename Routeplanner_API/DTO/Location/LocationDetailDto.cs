namespace Routeplanner_API.DTO.Location
{
    /// <summary>
    /// Data transfer object representing detailed information about a location.
    /// </summary>
    public sealed class LocationDetailDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the location details.
        /// </summary>
        public Guid LocationDetailsId { get; set; }

        /// <summary>
        /// Gets or sets the address of the location.
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// Gets or sets the city where the location is situated.
        /// </summary>
        public string? City { get; set; }

        /// <summary>
        /// Gets or sets the country of the location.
        /// </summary>
        public string? Country { get; set; }

        /// <summary>
        /// Gets or sets the postal code of the location.
        /// </summary>
        public string? ZipCode { get; set; }

        /// <summary>
        /// Gets or sets the phone number associated with the location.
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the website URL for the location.
        /// </summary>
        public string? Website { get; set; }

        /// <summary>
        /// Gets or sets the category of the location.
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// Gets or sets the accessibility information of the location.
        /// </summary>
        public string? Accessibility { get; set; }
    }
}
