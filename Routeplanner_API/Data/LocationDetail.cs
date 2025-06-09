namespace Routeplanner_API.Models
{
    /// <summary>
    /// Contains detailed information about a specific location.
    /// </summary>
    public partial class LocationDetail
    {
        /// <summary>
        /// Gets the unique identifier for the location details.
        /// </summary>
        public Guid LocationDetailsId { get; init; }

        /// <summary>
        /// Gets the identifier of the associated location.
        /// </summary>
        public Guid LocationId { get; init; }

        /// <summary>
        /// Gets or sets the address of the location.
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// Gets or sets the city where the location is situated.
        /// </summary>
        public string? City { get; set; }

        /// <summary>
        /// Gets or sets the country where the location is situated.
        /// </summary>
        public string? Country { get; set; }

        /// <summary>
        /// Gets or sets the zip or postal code of the location.
        /// </summary>
        public string? ZipCode { get; set; }

        /// <summary>
        /// Gets or sets the phone number associated with the location.
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the website URL of the location.
        /// </summary>
        public string? Website { get; set; }

        /// <summary>
        /// Gets or sets the category of the location (e.g., restaurant, museum).
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// Gets or sets accessibility information for the location.
        /// </summary>
        public string? Accessibility { get; set; }

        /// <summary>
        /// Gets or sets the associated location entity.
        /// </summary>
        public virtual Location Location { get; set; } = null!;
    }
}
