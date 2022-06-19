using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sensemaking.Host.Monitoring;
using Serilog;

namespace Sensemaking.Web.Host
{
    internal static class Logger
    {
        internal static IServiceCollection ProvideLogging(this IServiceCollection services, ILogger logger)
        {
            services.Replace(ServiceDescriptor.Singleton(logger));
            return services;
        }

        internal static IApplicationBuilder UseLogger(this IApplicationBuilder app, IMonitorServices serviceMonitor)
        {
            Logging.Configure(serviceMonitor.Info, app.ApplicationServices.GetRequiredService<ILogger>());
            return app;
        }
    } 
}