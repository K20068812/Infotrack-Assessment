namespace LandRegistryApi.Infrastructure.Configuration
{
    public class GoogleSearchOptions
    {
        public const string SectionName = "GoogleSearch";
        public string SocsCookie { get; set; } = string.Empty;
        public string SecureEnidCookie { get; set; } = string.Empty;
        public int TimeoutSeconds { get; set; } = 30;
        public string UserAgent { get; set; } = string.Empty;
    }
}
