using LandRegistryApi.Core.Interfaces;
using LandRegistryApi.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using HtmlAgilityPack;
using System.Web;
using FluentResults;

namespace LandRegistryApi.Infrastructure.Services
{
    public class BingSearchEngine : ISearchEngine
    {
        private readonly HttpClient _httpClient;

        public BingSearchEngine(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public string BaseUrl => "https://www.bing.com/search?q=";
        public string SearchEngineName => "Bing";

        public async Task<Result<List<int>>> GetRankingPositionsAsync(string searchQuery, string targetUrl)
        {
            try
            {
                var encodedQuery = Uri.EscapeDataString(searchQuery);
                var positions = new List<int>();

                for (int i = 0; i <= 90; i += 10)
                {
                    var searchUrl = $"{BaseUrl}{encodedQuery}&first={i}&setmkt=en-GB&cc=GB";
                    var request = new HttpRequestMessage(HttpMethod.Get, searchUrl);

                    // TODO: add this to appsettings.json?
                    request.Headers.Add("Cookie", "_edge_cd=m=en-gb; usrloc=HS=1&ELOC=LAT=53.4808|LON=-2.2426|N=Manchester, England|ELT=6|&BLOCK=TS=250629132037; RCHLANG=en&PV=10.0.0&DM=1&BRW=NOTP&BRH=M&CW=712&CH=929&SCW=1164&SCH=1841&DPR=1.0&UTC=60&EXLTT=3; SRCHD=AF=NOFORM; SRCHUSR=DOB=20220101");
                    request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
                    request.Headers.Add("Accept-Language", "en-GB,en;q=0.9");
                    request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");

                    var response = await _httpClient.SendAsync(request);
                    var content = await response.Content.ReadAsStringAsync();

                    var pagePositions = await ParseSearchResults(content, targetUrl, i);
                    positions.AddRange(pagePositions);
                }

                return Result.Ok(positions);
            }
            catch (Exception ex)
            {
                return Result.Fail($"An unexpected error {ex.Message} occurred while fetching search results");
            }
        }

        private static async Task<List<int>> ParseSearchResults(string htmlContent, string targetUrl, int startIndex)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            var tpttDivs = doc.DocumentNode.SelectNodes("//div[@class='tptt']");
            if (tpttDivs == null) return new List<int>();

            var positions = new List<int>();

            for (int i = 0; i < tpttDivs.Count; i++)
            {
                var div = tpttDivs[i];
                string href = null;

                var ancestorLink = div.Ancestors("a").FirstOrDefault(a => !string.IsNullOrEmpty(a.GetAttributeValue("href", "")));
                if (ancestorLink != null)
                {
                    href = ancestorLink.GetAttributeValue("href", "");
                }

                var url = await ResolveRedirectAsync(href);


                if (!string.IsNullOrEmpty(href))
                {
                    var decodedHref = HttpUtility.HtmlDecode(href);
                    if (decodedHref.Contains(targetUrl, StringComparison.OrdinalIgnoreCase))
                    {
                        positions.Add(startIndex + i + 1);
                    }
                }
            }

            return positions;
        }

        private static async Task<string> ResolveRedirectAsync(string redirectUrl)
        {
            // TODO: FIND BETTER WAY TO DO THIS, VERY SLOW
            using var handler = new HttpClientHandler
            {
                AllowAutoRedirect = false
            };

            using var client = new HttpClient(handler);
            var response = await client.GetAsync(redirectUrl);

            if ((int)response.StatusCode >= 300 && (int)response.StatusCode < 400)
            {
                return response.Headers.Location?.ToString() ?? redirectUrl;
            }

            return redirectUrl;
        }

    }
}