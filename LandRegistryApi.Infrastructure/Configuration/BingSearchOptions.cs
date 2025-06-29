using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandRegistryApi.Infrastructure.Configuration
{
    public class BingSearchOptions
    {
        public const string SectionName = "BingSearch";
        public int TimeoutSeconds { get; set; } = 30;
        public string UserAgent { get; set; } = string.Empty;
    }
}
