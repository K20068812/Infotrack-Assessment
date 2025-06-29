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
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options => 
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<ISearchResultRepository, SearchResultRepository>();
            services.Configure<GoogleSearchOptions>(configuration.GetSection(GoogleSearchOptions.SectionName));
            services.Configure<BingSearchOptions>(configuration.GetSection(BingSearchOptions.SectionName));

            services.AddHttpClient<ISearchEngine, GoogleSearchEngine>((serviceProvider, client) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<GoogleSearchOptions>>().Value;
                client.Timeout = TimeSpan.FromSeconds(30);

                if (!string.IsNullOrEmpty(options.UserAgent))
                {
                    client.DefaultRequestHeaders.Add("User-Agent", options.UserAgent);
                }
            });

            services.AddHttpClient<ISearchEngine, BingSearchEngine>((serviceProvider, client) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<BingSearchOptions>>().Value;
                client.Timeout = TimeSpan.FromSeconds(30);

                if (!string.IsNullOrEmpty(options.UserAgent))
                {
                    client.DefaultRequestHeaders.Add("User-Agent", options.UserAgent);
                }
            });

            services.AddScoped<ISearchEngineFactory, SearchEngineFactory>();


            return services;
        }
    }
}
