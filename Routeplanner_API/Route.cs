namespace Routeplanner_API
{
    public class Route
    {
        public string name { get; set; }
        public string description { get; set; }
        public Location[]? locations { get; set; }
    }
}
