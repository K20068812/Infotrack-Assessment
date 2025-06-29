using FluentResults;

namespace LandRegistryApi.Core.Interfaces
{
    public interface ISearchEngine
    {
        public string BaseUrl { get; }

        Task<Result<List<int>>> GetRankingPositionsAsync(string searchQuery, string targetUrl);

        public string SearchEngineName { get; }
    }
}
