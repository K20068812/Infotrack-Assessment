using LandRegistryApi.Api.Models;
using LandRegistryApi.Core.Entities;
using LandRegistryApi.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SearchResult = LandRegistryApi.Api.Models.WebSearchResult;

namespace LandRegistryApi.Api.Controllers
{
    public class LandRegistryController : ControllerBase
    {
        private readonly IRankingService _rankingService;

        public LandRegistryController(IRankingService rankingService)
        {
            _rankingService = rankingService ?? throw new ArgumentNullException(nameof(rankingService));
        }

        [HttpPost("api/check-ranking")]
        public async Task<IActionResult> CheckRanking([FromBody] SearchRequest request)
        {
            var result = await _rankingService.CheckRankingAsync(request.SearchQuery, request.TargetUrl);
            if (!result.IsSuccess)
            {
                return StatusCode(500, result.Errors);
            }

            var rankings = result.Value;
            return Ok(new SearchResult
            {
                SearchQuery = rankings.SearchQuery,
                TargetUrl = rankings.TargetUrl,
                Positions = rankings.Positions,
                SearchDate = rankings.SearchDate
            });
        }

        [HttpGet("api/ranking-history/{targetUrl}")]

        public async Task<IActionResult> GetRankingHistory([FromQuery] RankingHistoryRequest request)
        {
            var unescapedUrl = Uri.UnescapeDataString(request.TargetUrl);
            var result = await _rankingService.GetRankingHistoryAsync(unescapedUrl, request.SearchQuery, request.Days);
            if (!result.IsSuccess)
            {
                return StatusCode(500, result.Errors);
            }

            var history = result.Value;
            return Ok(history.Select(sr => new SearchResult
            {
                SearchQuery = sr.SearchQuery,
                TargetUrl = sr.TargetUrl,
                Positions = sr.Positions,
                SearchDate = sr.SearchDate,
            }));
        }

        [HttpGet("api/ranking-history/{targetUrl}/grouped")]
        public async Task<IActionResult> GetGroupedRankingHistory(
        string targetUrl,
        [FromQuery] GroupingPeriod groupingPeriod,
        [FromQuery] int days = 30)
        {
            var unescapedUrl = Uri.UnescapeDataString(targetUrl);
            var result = await _rankingService.GetGroupedRankingHistoryAsync(unescapedUrl, groupingPeriod, days);

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.Errors);
            }

            return Ok(result.Value);
        }
    }
}
