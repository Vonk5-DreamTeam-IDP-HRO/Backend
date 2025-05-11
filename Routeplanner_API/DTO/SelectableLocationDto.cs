namespace Routeplanner_API.DTO
{
    public class SelectableLocationDto
    {
        public int LocationId { get; set; }
        public string Name { get; set; } = null!;
        public string Category { get; set; } = null!;
    }
}
