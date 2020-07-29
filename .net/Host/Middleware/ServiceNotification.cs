using Microsoft.AspNetCore.Builder;
using NodaTime;
using Sensemaking.Host.Monitoring;

namespace Sensemaking.Web.Host
{
    internal static class ServiceNotification
    {
        internal static IApplicationBuilder ScheduleStatusNotification(this IApplicationBuilder app, IMonitorServices monitor, Period heartbeat)
        {
            Notifier = new ServiceStatusNotifier(monitor, heartbeat);
            return app;
        }

        internal static ServiceStatusNotifier? Notifier { get; private set; }
    } 
}