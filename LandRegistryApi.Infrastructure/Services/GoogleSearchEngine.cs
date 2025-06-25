using LandRegistryApi.Core.Interfaces;

namespace LandRegistryApi.Infrastructure.Services
{
    public class GoogleSearchEngine : ISearchEngine
    {
        private readonly HttpClient _httpClient;

        public string BaseUrl => "https://www.google.co.uk/search?num=100&q=";

        // todo: if an error happens, do we want to log it and display it on the frontend? 

        public async Task<List<int>> GetRankingPositionsAsync(string searchQuery, string targetUrl)
        {
            try
            {
                var encodedQuery = Uri.EscapeDataString(searchQuery);
                var searchUrl = $"{BaseUrl}{encodedQuery}";

                var response = await _httpClient.GetStringAsync(searchUrl);

                return new List<int>(); // Todo: need to process the response


            }
            catch (Exception ex)
            {
                // Todo: log
                return new List<int>();
            }
        }
    }
}
