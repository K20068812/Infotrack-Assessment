namespace LandRegistryApi.Api.Models
{
    public class SearchResult
    {
        public string SearchQuery { get; set; }
        public string TargetUrl { get; set; }
        public List<int> Positions { get; set; }
        public DateTime SearchDate { get; set; }
    }
}
