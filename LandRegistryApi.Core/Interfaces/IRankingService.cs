using FluentResults;
using LandRegistryApi.Core.Entities;

namespace LandRegistryApi.Core.Interfaces
{
    public interface IRankingService
    {
        Task<Result<SearchResult>> CheckRankingAsync(string searchQuery, string targetUrl, string searchEngine);
        Task<Result<List<SearchResult>>> GetRankingHistoryAsync(string targetUrl, string searchQuery, string searchEngine, int days);
        Task<Result<List<GroupedSearchResult>>> GetGroupedRankingHistoryAsync(string targetUrl, string searchQuery, string searchEngine, GroupingPeriod groupBy, int days);
    }
}
