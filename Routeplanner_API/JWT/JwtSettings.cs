namespace Routeplanner_API.JWT
{
    public class JwtSettings
    {
        public string Secret { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public DateTime ExpiryMinutes { get; set; }
    }
}
