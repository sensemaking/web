using NSubstitute;
using Sensemaking.Host.Monitoring;
using Sensemaking.Monitoring;
using Sensemaking.Web.Host;
using Serilog;

namespace Sensemaking.Bdd.Web
{
    public abstract class FakeStartup : ApiStartup
    {
        public const string Name = "Json Web Api";
        public static readonly ServiceDependency Dependency = new ServiceDependency(new FakeMonitor());

        protected override IMonitorServices ServiceMonitor { get; } = new ServiceMonitor(Name, Dependency);

        protected override ILogger Logger { get; } = Substitute.For<ILogger>();

        private class FakeMonitor : IMonitor
        {
            public Availability Availability()
            {
                return Monitoring.Availability.Up();
            }

            public MonitorInfo Info => new MonitorInfo("Awesome Monitor", "Bob the monitor");
        }

        public ILogger SubstituteLogger => Logger;
    }
}