using NodaTime;
using Sensemaking.Host.Monitoring;

namespace Sensemaking.Host.Web
{
    public static class Web
    {
        internal static ServiceStatusNotifier StatusNotifier { get; set; }

        public static void Configure(params ServiceDependency[] dependencies)
        {
            StatusNotifier = new ServiceStatusNotifier(new ServiceMonitor(Period.FromSeconds(20), dependencies));
        }
    }
}