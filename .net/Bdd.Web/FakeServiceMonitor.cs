using Sensemaking.Host.Monitoring;
using Sensemaking.Monitoring;

namespace Sensemaking.Bdd.Web
{
    public class FakeServiceMonitor : IMonitorServices
    {
        public Availability Availability()
        {
            return Monitoring.Availability.Up();
        }

        public ServiceMonitor.Status GetStatus()
        {
            return new ServiceMonitor.Status();
        }

        public MonitorInfo Info => new MonitorInfo("Fake Service Monitor", "Faked Json Api");
        public ServiceDependency[] Dependencies { get; } = new[] { new ServiceDependency(new FakeMonitor()) };

        private class FakeMonitor : IMonitor
        {
            public Availability Availability()
            {
                return Sensemaking.Monitoring.Availability.Up();
            }

            public MonitorInfo Info => new MonitorInfo("Fake Monitor", "Bob the monitor");
        }
    }
}