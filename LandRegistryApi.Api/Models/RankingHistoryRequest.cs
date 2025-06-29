using System.ComponentModel.DataAnnotations;

namespace LandRegistryApi.Api.Models
{
    public class RankingHistoryRequest
    {
        [Required]
        [Url]
        public string TargetUrl { get; set; }

        [Required]
        [MinLength(1)]
        public string SearchQuery { get; set; }
    }

}
