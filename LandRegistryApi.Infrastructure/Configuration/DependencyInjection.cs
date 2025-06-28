using LandRegistryApi.Core.Interfaces;
using LandRegistryApi.Infrastructure.Data;
using LandRegistryApi.Infrastructure.Repositories;
using LandRegistryApi.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LandRegistryApi.Infrastructure.Configuration
{
    // todo: make sure this only references core project, shouldnt reference anything else! 
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options => 
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<ISearchResultRepository, SearchResultRepository>();
            services.AddScoped<ISearchEngine, GoogleSearchEngine>(); // todo - what about the others. factory pattern? search engine factory TODO!
            // todo with the factory, can do this, bing and another "generic" factory that tries it?? can inform on frontend that wont be accurate??
            services.AddHttpClient<ISearchEngine, GoogleSearchEngine>((serviceProvider, client) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<GoogleSearchOptions>>().Value;
                client.Timeout = TimeSpan.FromSeconds(30);

                if (!string.IsNullOrEmpty(options.UserAgent))
                {
                    client.DefaultRequestHeaders.Add("User-Agent", options.UserAgent); // todo
                }
            });
            services.Configure<GoogleSearchOptions>(configuration.GetSection(GoogleSearchOptions.SectionName));

            //todo: memory caching? and set the caching on their end too? or is that excessive
            // todo: background service for daily? or hourly? TODO!

            return services;
        }
    }
}
