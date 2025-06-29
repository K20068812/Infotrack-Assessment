namespace LandRegistryApi.Core.Entities
{
    public enum GroupingPeriod
    {
        Day,
        Week,
    }

    public class GroupedSearchResult
    {
        public required string SearchQuery { get; set; }
        public required string TargetUrl { get; set; }
        public required DateTime PeriodStart { get; set; }
        public required DateTime PeriodEnd { get; set; }
        public required List<int> AllPositions { get; set; } = [];
        public required double AveragePosition { get; set; }
        public required int BestPosition { get; set; }
        public required int WorstPosition { get; set; }
        public required int TotalSearches { get; set; }
        public required string GroupingPeriod { get; set; }
    }
}
