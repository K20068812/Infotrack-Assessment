using FluentResults;
using LandRegistryApi.Core.Entities;
using LandRegistryApi.Core.Interfaces;

namespace LandRegistryApi.Core.Services
{
    public class RankingService : IRankingService
    {
        private readonly ISearchEngine _searchEngine;
        private readonly ISearchResultRepository _searchResultRepository;

        public RankingService(ISearchEngine searchEngine, ISearchResultRepository searchResultRepository)
        {
            _searchEngine = searchEngine ?? throw new ArgumentNullException(nameof(searchEngine));
            _searchResultRepository = searchResultRepository ?? throw new ArgumentNullException(nameof(searchResultRepository));
        }

        public async Task<Result<SearchResult>> CheckRankingAsync(string searchQuery, string targetUrl)
        {
            var positions = await _searchEngine.GetRankingPositionsAsync(searchQuery, targetUrl);
            if (!positions.IsSuccess)
            {
                return Result.Fail(positions.Errors);
            }

            var result = new SearchResult
            {
                SearchQuery = searchQuery,
                TargetUrl = targetUrl,
                Positions = positions,
                SearchDate = DateTime.UtcNow,
            };

            return await _searchResultRepository.SaveSearchResultAsync(result);
        }

        public async Task<Result<List<SearchResult>>> GetRankingHistoryAsync(string targetUrl, int days = 30)
        {
            return await _searchResultRepository.GetAllSearchResultsAsync(targetUrl, days);
        }
    }
}
