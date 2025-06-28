using LandRegistryApi.Core.Interfaces;
using LandRegistryApi.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using System.Net;
namespace LandRegistryApi.Infrastructure.Services
{
    public class GoogleSearchEngine : ISearchEngine
    {
        //todo: add logging?
        private readonly HttpClient _httpClient; 
        // TODO: add toggle between SOCS cookie and hardcoded response in case it expires?????!!
        private readonly GoogleSearchOptions _options;
        public GoogleSearchEngine(HttpClient httpClient, IOptions<GoogleSearchOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        // todo: if an error happens, do we want to log it and display it on the frontend?
        // theres also gotta be a better way than hardcoding the url like this below
        public string BaseUrl => "https://www.google.co.uk/search?num=100&q=";
        public async Task<List<int>> GetRankingPositionsAsync(string searchQuery, string targetUrl)
        {
            try
            {
                var encodedQuery = Uri.EscapeDataString(searchQuery);
                var searchUrl = $"{BaseUrl}{encodedQuery}";

                var request = new HttpRequestMessage(HttpMethod.Get, searchUrl);

                if (!string.IsNullOrEmpty(_options.SocsCookie) && !string.IsNullOrEmpty(_options.SecureEnidCookie))
                {
                    request.Headers.Add("Cookie", $"SOCS={_options.SocsCookie}; __Secure-ENID={_options.SecureEnidCookie}");
                }

                var response = await _httpClient.SendAsync(request);
                string content = await response.Content.ReadAsStringAsync();
                if (content.Contains("Before you continue to Google"))
                {
                    return new List<int>();
                }

                var positions = ParseSearchResults(content, targetUrl);
                return positions;
            }
            catch (Exception ex)
            { // Todo: log the exception
                return new List<int>();
            }
        }
        private List<int> ParseSearchResults(string htmlContent, string targetUrl)
        {
            var positions = new List<int>();
            return positions;
        }
    }
}