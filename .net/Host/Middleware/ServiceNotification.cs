using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Sensemaking.Host.Monitoring;

namespace Sensemaking.Web.Host
{
    internal static class ServiceNotification
    {
        internal static IServiceCollection AddMonitor(this IServiceCollection services, string serviceName, params ServiceDependency[] dependencies)
        {
            services.AddSingleton<IMonitorServices>(new ServiceMonitor(serviceName, dependencies));
            return services;
        }

        internal static IApplicationBuilder ScheduleStatusNotification(this IApplicationBuilder app, IMonitorServices monitor, Period heartbeat)
        {
            Notifier = new ServiceStatusNotifier(monitor, heartbeat);
            return app;
        }

        internal static ServiceStatusNotifier? Notifier { get; private set; }
    } 
}