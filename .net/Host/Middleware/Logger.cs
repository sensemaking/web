using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Sensemaking.Web.Host
{
    internal static class Logger
    {
        internal static IServiceCollection AddLogger(this IServiceCollection services, ILogger logger)
        {
            services.AddSingleton(logger);
            return services;
        }

        internal static IApplicationBuilder UseLogger(this IApplicationBuilder app, ILogger logger)
        {
            Logging.Configure(logger);
            return app;
        }
    } 
}