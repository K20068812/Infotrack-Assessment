using FluentResults;

namespace LandRegistryApi.Core.Interfaces
{
    // Todo: Search engine will come from the request. Need a way of resolving the correct search engine based on the request.
    // maybe I can use a factory pattern or dependency injection to resolve the correct search engine.
    public interface ISearchEngine
    {
        public string BaseUrl { get; }

        Task<Result<List<int>>> GetRankingPositionsAsync(string searchQuery, string targetUrl);
    }
}
