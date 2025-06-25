using LandRegistryApi.Core.Entities;

namespace LandRegistryApi.Core.Interfaces
{
    public interface ISearchResultRepository
    {
        Task<SearchResult> SaveSearchResultAsync(SearchResult result);
        Task<List<SearchResult>> GetAllSearchResultsAsync(string targetUrl, int days);
    }
}
