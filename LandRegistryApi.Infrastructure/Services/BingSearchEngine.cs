using LandRegistryApi.Core.Interfaces;
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

                    var response = await _httpClient.SendAsync(request);
                    var content = await response.Content.ReadAsStringAsync();

                    var pagePositions = ParseSearchResults(content, targetUrl, i);

                    if (pagePositions.IsSuccess)
                    {
                        positions.AddRange(pagePositions.Value);
                    }
                }

                return Result.Ok(positions);
            }
            catch (Exception ex)
            {
                return Result.Fail($"An unexpected error {ex.Message} occurred while fetching search results");
            }
        }

        private Result<List<int>> ParseSearchResults(string htmlContent, string targetUrl, int startIndex)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            var tpttDivs = doc.DocumentNode.SelectNodes("//div[@class='tptt']");
            if (tpttDivs == null)
            {
                return Result.Fail("No search results found");
            }

            var positions = new List<int>();

            for (int i = 0; i < tpttDivs.Count; i++)
            {
                var div = tpttDivs[i];

                var ancestorLink = div.Ancestors("a").FirstOrDefault(a => !string.IsNullOrEmpty(a.GetAttributeValue("href", "")));
                if (ancestorLink == null)
                {
                    continue;
                }
                var href = ancestorLink.GetAttributeValue("href", "");
                // bing stores it urls as redirects, so we need to resolve the final URL
                var urlResult = ResolveRedirect(href);
                if (urlResult.IsFailed)
                {
                    continue;
                }

                var url = urlResult.Value;
                if (string.IsNullOrEmpty(url))
                {
                    continue;
                }
                var decodedUrl = Uri.UnescapeDataString(url);
                if (decodedUrl.Contains(targetUrl, StringComparison.OrdinalIgnoreCase))
                {
                    positions.Add(startIndex + i + 1);
                }
            }

            return positions;
        }

        private Result<string> ResolveRedirect(string redirectUrl)
        {
            var decodedUrl = Uri.UnescapeDataString(redirectUrl);
            if (!IsBingRedirectUrl(decodedUrl))
            {
                return decodedUrl;
            }

            var actualUrlResult = ExtractUrlFromBingRedirect(decodedUrl);
            if (!actualUrlResult.IsSuccess)
            {
                return Result.Fail(actualUrlResult.Errors);
            }

            var actualUrl = actualUrlResult.Value;
            return Result.Ok(actualUrl);
        }

        private static Result<string> ExtractUrlFromBingRedirect(string bingRedirectUrl)
        {
            if (!bingRedirectUrl.Contains("/ck/a?"))
            {
                return Result.Fail("Could not extract redirect url");
            }

            try
            {
                var uri = new Uri(HttpUtility.HtmlDecode(bingRedirectUrl));
                var queryParams = HttpUtility.ParseQueryString(uri.Query);

                var uParam = queryParams["u"];
                if (string.IsNullOrEmpty(uParam))
                {
                    return Result.Fail("Could not extract redirect url");
                }

                // Copied from: https://greasyfork.org/en/scripts/474035-bing-link-redirect-decoder/code
                var base64String = uParam.StartsWith("a1") ? uParam.Substring(2) : uParam;
                base64String = base64String.Replace('-', '+').Replace('_', '/');

                var paddingNeeded = base64String.Length % 4;
                if (paddingNeeded > 0)
                {
                    base64String += new string('=', 4 - paddingNeeded);
                }

                var decodedBytes = Convert.FromBase64String(base64String);
                var decodedUrl = System.Text.Encoding.UTF8.GetString(decodedBytes);
                return Result.Ok(decodedUrl);
            }
            catch (Exception e)
            {
                return Result.Fail($"Could not extract redirect url - {e.Message}");
            }
        }

        private static bool IsBingRedirectUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return false;
            }

            return url.StartsWith("https://www.bing.com/ck/a?", StringComparison.OrdinalIgnoreCase) ||
                   url.StartsWith("https://www.bing.com/aclk?", StringComparison.OrdinalIgnoreCase);
        }
    }
}