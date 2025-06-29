using FluentResults;
using LandRegistryApi.Core.Entities;

namespace LandRegistryApi.Core.Interfaces
{
    public interface IRankingService
    {
        Task<Result<SearchResult>> CheckRankingAsync(string searchQuery, string targetUrl);
        Task<Result<List<SearchResult>>> GetRankingHistoryAsync(string targetUrl, string searchQuery, int days = 30);
    }
}
