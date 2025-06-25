using LandRegistryApi.Core.Interfaces;

namespace LandRegistryApi.Infrastructure.Services
{
    public class GoogleSearchEngine : ISearchEngine
    {
        public Task<List<int>> GetRankingPositionsAsync(string searchQuery, string targetUrl)
        {
            throw new NotImplementedException();
        }
    }
}
