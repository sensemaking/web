using Microsoft.AspNetCore.Builder;
using NodaTime;
using Sensemaking.Host.Monitoring;

namespace Sensemaking.Host.Web
{
    internal static class ServiceNotification
    {
        internal static IApplicationBuilder UseStatusNotification(this IApplicationBuilder app, IMonitorServices monitor)
        {
            Notifier = new ServiceStatusNotifier(monitor);
            return app;
        }

        internal static ServiceStatusNotifier? Notifier { get; private set; }
    } 
}