using LandRegistryApi.Core.Interfaces;
using LandRegistryApi.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LandRegistryApi.Core.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddScoped<IRankingService, RankingService>();
            return services;
        }
    }
}
