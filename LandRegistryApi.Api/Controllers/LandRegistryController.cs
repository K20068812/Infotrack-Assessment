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
            var result = await _rankingService.CheckRankingAsync(request.SearchQuery, request.TargetUrl, request.SearchEngine);
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

        [HttpGet("api/ranking-history")]

        public async Task<IActionResult> GetRankingHistory([FromQuery] RankingHistoryRequest request)
        {
            var unescapedUrl = Uri.UnescapeDataString(request.TargetUrl);
            var result = await _rankingService.GetRankingHistoryAsync(unescapedUrl, request.SearchQuery, request.SearchEngine);
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

        [HttpGet("api/ranking-history/grouped")]
        public async Task<IActionResult> GetGroupedRankingHistory(
        [FromQuery] RankingHistoryRequest request,
        [FromQuery] string groupingPeriod)
        {
            if (!Enum.TryParse<GroupingPeriod>(groupingPeriod, true, out var groupingPeriodValue))
            {
                return StatusCode(400, "Invalid grouping period");
            }

            var unescapedUrl = Uri.UnescapeDataString(request.TargetUrl);
            var result = await _rankingService.GetGroupedRankingHistoryAsync(unescapedUrl, request.SearchQuery, request.SearchEngine, groupingPeriodValue);

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.Errors);
            }

            return Ok(result.Value);
        }
    }
}
