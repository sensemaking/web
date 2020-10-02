using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NodaTime;
using Sensemaking.Host.Monitoring;

namespace Sensemaking.Web.Host
{
    internal static class ServiceNotification
    {
        internal static IServiceCollection ProvideMonitoring(this IServiceCollection services, IMonitorServices serviceMonitor)
        {
            services.Replace(ServiceDescriptor.Singleton(serviceMonitor));
            return services;
        }

        internal static IApplicationBuilder ScheduleStatusNotification(this IApplicationBuilder app, Period heartbeat)
        {
            Notifier = new ServiceStatusNotifier(app.ApplicationServices.GetRequiredService<IMonitorServices>(), heartbeat);
            return app;
        }

        internal static ServiceStatusNotifier? Notifier { get; private set; }
    } 
}