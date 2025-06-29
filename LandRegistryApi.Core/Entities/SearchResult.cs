namespace LandRegistryApi.Core.Entities
{
    public class SearchResult
    {
        public int Id { get; set; }
        public required string SearchQuery { get; set; }
        public required string TargetUrl { get; set; }
        public required List<int> Positions { get; set; }
        public required DateTime SearchDate { get; set; }
        public required string SearchEngine { get; set; }
    
    }
}
