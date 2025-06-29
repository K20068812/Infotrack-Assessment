using FluentResults;

namespace LandRegistryApi.Core.Interfaces
{
    public interface ISearchEngineFactory
    {
        Result<ISearchEngine> Create(string engineName);
    }
}
