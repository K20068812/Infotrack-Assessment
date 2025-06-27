using LandRegistryApi.Core.Interfaces;
using LandRegistryApi.Infrastructure.Data;
using LandRegistryApi.Infrastructure.Repositories;
using LandRegistryApi.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LandRegistryApi.Infrastructure.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options => 
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<ISearchResultRepository, SearchResultRepository>();
            services.AddScoped<ISearchEngine, GoogleSearchEngine>(); // todo - what about the others. factory pattern? search engine factory TODO!
            services.AddHttpClient<ISearchEngine, GoogleSearchEngine>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36"); // todo
            });

            //todo: memory caching? 
            // todo: background service for daily? or hourly? TODO!

            return services;
        }
    }
}
