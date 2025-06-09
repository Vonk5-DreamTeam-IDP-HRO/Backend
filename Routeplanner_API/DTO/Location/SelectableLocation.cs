namespace Routeplanner_API.DTO.Location
{
    /// <summary>
    /// Data transfer object representing a selectable location with minimal details.
    /// </summary>
    public class SelectableLocationDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the location.
        /// </summary>
        public Guid LocationId { get; set; }

        /// <summary>
        /// Gets or sets the name of the location.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the category of the location.
        /// </summary>
        public string? Category { get; set; }
    }
}
