using LandRegistryApi.Core.Interfaces;
using LandRegistryApi.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using HtmlAgilityPack;
using System.Web;
using FluentResults;

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

        public async Task<Result<List<int>>> GetRankingPositionsAsync(string searchQuery, string targetUrl)
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
                var content = await response.Content.ReadAsStringAsync();

                return ParseSearchResults(content, targetUrl);

            }
            catch (Exception ex)
            {
                return Result.Fail($"An unexpected error {ex.Message} occurred while fetching search results");
            }
        }
        private static Result<List<int>> ParseSearchResults(string htmlContent, string targetUrl)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            // todo should we have a backup in case this is null - to just go off the <a> tags?
            var anchorTags = doc.DocumentNode.SelectNodes($"//a[contains(@class, 'zReHs')]");
            if (anchorTags == null)
            {
                return Result.Fail("No search results found");
            }

            var occurences = anchorTags
                .Select((anchor, index) => new { Anchor = anchor, Index = index + 1 }) // Index starts from 1
                .Where(x =>
                {
                    var href = x.Anchor.GetAttributeValue("href", string.Empty);
                    var decodedHref = HttpUtility.HtmlDecode(href);
                    return decodedHref.Contains(targetUrl, StringComparison.OrdinalIgnoreCase);
                })
                .Select(x => x.Index)
                .ToList();

            return Result.Ok(occurences);
        }
    }
}