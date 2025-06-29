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
            services.AddScoped<ISearchEngine, GoogleSearchEngine>();
            services.AddScoped<ISearchEngine, BingSearchEngine>();
            services.AddScoped<SearchEngineFactory>();

            services.AddHttpClient<ISearchEngine, GoogleSearchEngine>((serviceProvider, client) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<GoogleSearchOptions>>().Value;
                client.Timeout = TimeSpan.FromSeconds(30);

                if (!string.IsNullOrEmpty(options.UserAgent))
                {
                    client.DefaultRequestHeaders.Add("User-Agent", options.UserAgent);
                }
            });
            services.Configure<GoogleSearchOptions>(configuration.GetSection(GoogleSearchOptions.SectionName));

            return services;
        }
    }
}
