namespace Routeplanner_API
{
    public class Route
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Location[]? Locations { get; set; }
    }
}
