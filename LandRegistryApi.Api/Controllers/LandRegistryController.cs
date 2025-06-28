using LandRegistryApi.Api.Models;
using LandRegistryApi.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
            // todo: do we want a try catch here? or in the service?
            try
            {
                var result = await _rankingService.CheckRankingAsync(request.SearchQuery, request.TargetUrl);
                return Ok(new SearchResult
                {
                    SearchQuery = result.SearchQuery,
                    TargetUrl = result.TargetUrl,
                    Positions = result.Positions,
                    SearchDate = result.SearchDate,
                    FoundPositions = result.Positions != "0" ?
                        result.Positions.Split(',').Select(int.Parse).ToList() :
                        []
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An error occurred while checking the ranking.", Details = ex.Message });
            }
        }

        [HttpGet("api/ranking-history/{targetUrl}")]

        public async Task<IActionResult> GetRankingHistory(string targetUrl, [FromQuery] int days = 30)
        {
            try
            {
                var unescapedUrl = Uri.UnescapeDataString(targetUrl);
                var history = await _rankingService.GetRankingHistoryAsync(unescapedUrl, days);

                return Ok(history.Select(sr => new SearchResult
                {
                    SearchQuery = sr.SearchQuery,
                    TargetUrl = sr.TargetUrl,
                    Positions = sr.Positions,
                    SearchDate = sr.SearchDate,
                    FoundPositions = sr.Positions != "0" ?
                        sr.Positions.Split(',').Select(int.Parse).ToList() :
                        []
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An error occurred while retrieving the ranking history.", Details = ex.Message });
            }
        }
    }
}
