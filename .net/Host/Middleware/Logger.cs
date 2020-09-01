using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Sensemaking.Web.Host
{
    public static class Logger
    {
        public static IServiceCollection AddLogger(this IServiceCollection services, ILogger logger)
        {
            services.AddSingleton(logger);
            return services;
        }

        public static IApplicationBuilder UseLogger(this IApplicationBuilder app, ILogger logger)
        {
            Logging.Configure(logger);
            return app;
        }
    } 
}