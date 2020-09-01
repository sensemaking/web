using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Sensemaking.Host.Monitoring;

namespace Sensemaking.Web.Host
{
    public static class ServiceNotification
    {
        public static IServiceCollection AddMonitor(this IServiceCollection services, string serviceName, params ServiceDependency[] dependencies)
        {
            services.AddSingleton<IMonitorServices>(new ServiceMonitor(serviceName, dependencies));
            return services;
        }

        public static IApplicationBuilder ScheduleStatusNotification(this IApplicationBuilder app, IMonitorServices monitor, Period heartbeat)
        {
            Notifier = new ServiceStatusNotifier(monitor, heartbeat);
            return app;
        }

        internal static ServiceStatusNotifier? Notifier { get; private set; }
    } 
}