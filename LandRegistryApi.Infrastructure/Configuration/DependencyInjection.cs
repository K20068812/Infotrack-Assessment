using LandRegistryApi.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace LandRegistryApi.Infrastructure.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(OptionsBuilderExtensions.))

            return services;
        }
    }
}
