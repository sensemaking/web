using NodaTime;
using Sensemaking.Host.Monitoring;

namespace Sensemaking.Host.Web
{
    internal static class ServiceStatus
    {
        internal static ServiceStatusNotifier? Notifier { get; set; }
    }
}