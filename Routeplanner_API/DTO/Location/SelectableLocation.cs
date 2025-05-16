namespace Routeplanner_API.DTO.Location
{
    public class SelectableLocationDto
    {
        public guid LocationId { get; set; }
        public string Name { get; set; }
        public string? Category { get; set; }
    }
}