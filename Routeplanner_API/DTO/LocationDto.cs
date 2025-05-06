namespace Routeplanner_API.DTO
{
    public class LocationDto
    {
        public int LocationId { get; set; }
        public string Name { get; set; } = null!;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? Description { get; set; }
        // Consider adding other relevant properties from LocationDetail or OpeningTimes if needed for display
    }
}
