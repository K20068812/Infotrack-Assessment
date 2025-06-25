using System.ComponentModel.DataAnnotations;

namespace LandRegistryApi.Api.Models
{
    public class SearchRequest
    {
        [Required]
        [StringLength(500)]
        public string SearchQuery { get; set; }

        [Required]
        [StringLength(500)]
        [Url]
        public string TargetUrl { get; set; }
    }
}
