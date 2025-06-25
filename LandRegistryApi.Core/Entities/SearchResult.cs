namespace LandRegistryApi.Core.Entities
{
    public class SearchResult
    {
        //todo: these should be init possibly?
        public int Id { get; set; }
        public string SearchQuery { get; set; }
        public string TargetUrl { get; set; }
        public string Positions { get; set; }
        public DateTime SearchDate { get; set; }
        public int TotalResults { get; set; }
    }
}
