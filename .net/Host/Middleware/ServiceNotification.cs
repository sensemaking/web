using Microsoft.AspNetCore.Builder;
using NodaTime;
using Sensemaking.Host.Monitoring;

namespace Sensemaking.Web.Host
{
    internal static class ServiceNotification
    {
        internal static IApplicationBuilder UseStatusNotification(this IApplicationBuilder app, IMonitorServices monitor)
        {
            Notifier = new ServiceStatusNotifier(monitor, Period.FromSeconds(20));
            return app;
        }

        internal static ServiceStatusNotifier? Notifier { get; private set; }
    } 
}