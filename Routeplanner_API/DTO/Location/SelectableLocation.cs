namespace Routeplanner_API.DTO.Location
{
    public class SelectableLocationDto
    {
        public Guid LocationId { get; set; }
        public string Name { get; set; }
        public string? Category { get; set; }
    }
}