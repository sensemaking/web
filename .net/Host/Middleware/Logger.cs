using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Sensemaking.Web.Host
{
    internal static class Logger
    {
        internal static IServiceCollection ProvideLogging(this IServiceCollection services, ILogger logger)
        {
            services.AddSingleton(logger);
            return services;
        }

        internal static IApplicationBuilder UseLogger(this IApplicationBuilder app)
        {
            Logging.Configure(app.ApplicationServices.GetRequiredService<ILogger>());
            return app;
        }
    } 
}