using FluentResults;
using LandRegistryApi.Core.Interfaces;
namespace LandRegistryApi.Infrastructure.Services
{
    public class SearchEngineFactory : ISearchEngineFactory
    {
        private readonly IEnumerable<ISearchEngine> _engines;

        public SearchEngineFactory(IEnumerable<ISearchEngine> engines)
        {
            _engines = engines;
        }

        public Result<ISearchEngine> Create(string engineName)
        {
            var engines = _engines.Where(e =>
                e.SearchEngineName.Equals(engineName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (engines.Count == 0 || engines.Count > 1)
            {
                return Result.Fail($"Search engine '{engineName}' not found or ambiguous");
            };

            return Result.Ok(engines.Single());
        }
    }
}
