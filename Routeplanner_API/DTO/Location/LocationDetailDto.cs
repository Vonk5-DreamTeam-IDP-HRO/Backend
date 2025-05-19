namespace Routeplanner_API.DTO.Location
{
    public sealed class LocationDetailDto
    {
        public Guid LocationDetailsId { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? ZipCode { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Website { get; set; }
        public string? Category { get; set; }
        public string? Accessibility { get; set; }
    }
}
