namespace LandRegistryApi.Api.Models
{
    public class WebSearchResult
    {
        public required string SearchQuery { get; set; }
        public required string TargetUrl { get; set; }
        public required List<int> Positions { get; set; }
        public required DateTime SearchDate { get; set; }
    }
}
