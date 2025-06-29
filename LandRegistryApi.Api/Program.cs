using LandRegistryApi.Core.Configuration;
using LandRegistryApi.Infrastructure.Configuration;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;

namespace LandRegistryApi.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddCoreServices();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.MapControllers();
            app.UseAuthorization();

            app.MapWhen(x => !x.Request.Path.Value.Contains("/api"), builder =>
            {
                builder.UseSpa(spa =>
                {
                    spa.Options.SourcePath = "ReactApp/infotrackui";
                    if (app.Environment.IsDevelopment())
                    {
                        spa.UseReactDevelopmentServer(npmScript: "start");
                    }
                });
            });

            app.Run();
        }
    }
}
