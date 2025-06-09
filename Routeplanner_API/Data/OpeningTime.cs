namespace Routeplanner_API.Models
{
    /// <summary>
    /// Represents the opening hours for a specific location.
    /// </summary>
    public partial class OpeningTime
    {
        /// <summary>
        /// Gets the unique identifier for the opening time entry.
        /// </summary>
        public Guid OpeningId { get; init; }

        /// <summary>
        /// Gets the identifier of the associated location.
        /// </summary>
        public Guid LocationId { get; init; }

        /// <summary>
        /// Gets or sets the day of the week for the opening time.
        /// </summary>
        public string DayOfWeek { get; set; } = null!;

        /// <summary>
        /// Gets or sets the opening time for the location on the specified day.
        /// </summary>
        public TimeOnly? OpenTime { get; set; }

        /// <summary>
        /// Gets or sets the closing time for the location on the specified day.
        /// </summary>
        public TimeOnly? CloseTime { get; set; }

        /// <summary>
        /// Indicates whether the location is open 24 hours on the specified day.
        /// </summary>
        public bool? Is24Hours { get; set; }

        /// <summary>
        /// Gets or sets the time zone for the opening hours.
        /// </summary>
        public string? Timezone { get; set; }

        /// <summary>
        /// Gets or sets the location associated with this opening time.
        /// </summary>
        public virtual Location Location { get; set; } = null!;
    }
}
