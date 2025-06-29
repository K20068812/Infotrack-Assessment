using FluentResults;
using LandRegistryApi.Core.Entities;
using LandRegistryApi.Core.Interfaces;

namespace LandRegistryApi.Core.Services
{
    public class RankingService : IRankingService
    {
        private readonly ISearchEngineFactory _searchEngineFactory;
        private readonly ISearchResultRepository _searchResultRepository;

        public RankingService(ISearchEngineFactory searchEngineFactory, ISearchResultRepository searchResultRepository)
        {
            _searchEngineFactory = searchEngineFactory ?? throw new ArgumentNullException(nameof(searchEngineFactory));
            _searchResultRepository = searchResultRepository ?? throw new ArgumentNullException(nameof(searchResultRepository));
        }

        public async Task<Result<SearchResult>> CheckRankingAsync(string searchQuery, string targetUrl, string searchEngineName)
        {
            var searchEngineCreationResult = _searchEngineFactory.Create(searchEngineName);
            if (!searchEngineCreationResult.IsSuccess)
            {
                return Result.Fail(searchEngineCreationResult.Errors);
            }
            var searchEngine = searchEngineCreationResult.Value;

            var positions = await searchEngine.GetRankingPositionsAsync(searchQuery, targetUrl);
            if (!positions.IsSuccess)
            {
                return Result.Fail(positions.Errors);
            }

            var result = new SearchResult
            {
                SearchQuery = searchQuery,
                TargetUrl = targetUrl,
                Positions = positions.Value,
                SearchDate = DateTime.UtcNow,
                SearchEngine = searchEngineName
            };

            return await _searchResultRepository.SaveSearchResultAsync(result);
        }

        public async Task<Result<List<SearchResult>>> GetRankingHistoryAsync(string targetUrl, string searchQuery, string searchEngineName)
        {
            return await _searchResultRepository.GetAllSearchResultsAsync(targetUrl, searchQuery, searchEngineName);
        }

        public async Task<Result<List<GroupedSearchResult>>> GetGroupedRankingHistoryAsync(
            string targetUrl,
            string searchQuery,
            string searchEngineName,
            GroupingPeriod groupBy)
        {
            var searchResults = await _searchResultRepository.GetAllSearchResultsAsync(targetUrl, searchQuery, searchEngineName);
            var groupedResults = new List<GroupedSearchResult>();

            var queryGroups = searchResults.GroupBy(sr => sr.SearchQuery);

            foreach (var queryGroup in queryGroups)
            {
                var timeGroups = GroupByTimePeriod(queryGroup, groupBy);

                foreach (var timeGroup in timeGroups)
                {
                    var positions = timeGroup.SelectMany(sr => sr.Positions).Where(p => p > 0).ToList();
                    if (positions.Count == 0)
                    {
                        continue;
                    }

                    var (periodStart, periodEnd) = GetPeriodBounds(timeGroup.Key, groupBy);

                    groupedResults.Add(new GroupedSearchResult
                    {
                        SearchQuery = queryGroup.Key,
                        TargetUrl = targetUrl,
                        PeriodStart = periodStart,
                        PeriodEnd = periodEnd,
                        AllPositions = positions,
                        AveragePosition = Math.Round(positions.Average(), 2),
                        BestPosition = positions.Min(),
                        WorstPosition = positions.Max(),
                        TotalSearches = timeGroup.Count(),
                        GroupingPeriod = groupBy.ToString()
                    });
                }
            }

            return Result.Ok(groupedResults.OrderBy(gr => gr.PeriodStart).ToList());
        }

        private static IEnumerable<IGrouping<string, SearchResult>> GroupByTimePeriod(
            IEnumerable<SearchResult> results,
            GroupingPeriod period)
        {
            switch (period)
            {
                case GroupingPeriod.Day:
                    return results.GroupBy(sr => sr.SearchDate.Date.ToString("yyyy-MM-dd"));

                case GroupingPeriod.Week:
                    return results.GroupBy(sr => sr.SearchDate.AddDays(-(int)sr.SearchDate.DayOfWeek).ToString("yyyy-MM-dd"));

                default:
                    throw new NotImplementedException($"Grouping period '{period}' is not implemented.");
            }
        }

        private static (DateTime start, DateTime end) GetPeriodBounds(string periodKey, GroupingPeriod groupBy)
        {
            switch (groupBy)
            {
                case GroupingPeriod.Day:
                    var date = DateTime.Parse(periodKey);
                    return (date, date.AddDays(1).AddTicks(-1));

                case GroupingPeriod.Week:
                    var weekStart = DateTime.Parse(periodKey);
                    return (weekStart, weekStart.AddDays(7).AddTicks(-1));

                default:
                    throw new NotImplementedException($"Grouping period '{groupBy}' is not implemented.");
            }
        }

    }
}