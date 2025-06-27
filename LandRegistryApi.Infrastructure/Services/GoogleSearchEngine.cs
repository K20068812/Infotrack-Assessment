using Azure.Core;
using LandRegistryApi.Core.Interfaces;
using System.Net;

namespace LandRegistryApi.Infrastructure.Services
{
    public class GoogleSearchEngine : ISearchEngine
    {
        //todo: add logging?
        private readonly HttpClient _httpClient;

        public GoogleSearchEngine(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public string BaseUrl => "https://www.google.co.uk/search?num=100&q=";

        // todo: if an error happens, do we want to log it and display it on the frontend? 

        public async Task<List<int>> GetRankingPositionsAsync(string searchQuery, string targetUrl)
        {
            try
            {
                var encodedQuery = Uri.EscapeDataString(searchQuery);
                var searchUrl = $"{BaseUrl}{encodedQuery}";

                var requestMessage = new HttpRequestMessage(HttpMethod.Get, searchUrl);

                var cookieContainer = new CookieContainer();

                var handler = new HttpClientHandler { CookieContainer = cookieContainer };
                var client = new HttpClient(handler);

                var response = await client.GetAsync("https://www.google.co.uk/search?num=100&q=land%20registry");
                string content = await response.Content.ReadAsStringAsync();

                //var response = await _httpClient.SendAsync(requestMessage);
                //var body = await response.Content.ReadAsStringAsync();

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
