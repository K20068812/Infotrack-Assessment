using LandRegistryApi.Core.Entities;

namespace LandRegistryApi.Core.Interfaces
{
    public interface IRankingService
    {
        Task<SearchResult> CheckRankingAsync(string searchQuery, string targetUrl);
        Task<List<SearchResult>> GetRankingHistoryAsync(string targetUrl, int days = 30);
    }
}
