namespace Routeplanner_API.DTO.Location
{
    public sealed class LocationDto
    {
        public Guid LocationId { get; init; }
        public string Name { get; init; } = string.Empty;
        public double Latitude { get; init; }
        public double Longitude { get; init; }
        public string? Description { get; init; }
        public LocationDetailDto? LocationDetail { get; set; }
    }
}
