using System.ComponentModel.DataAnnotations;

namespace LandRegistryApi.Api.Models
{
    public class RankingHistoryRequest
    {
        [Required]
        [MinLength(1)]
        public required string SearchQuery { get; set; }

        [Required]
        [Url]
        public required string TargetUrl { get; set; }

        [Required]
        public required string SearchEngine { get; set; }
    }
}
