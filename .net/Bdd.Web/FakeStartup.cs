using NSubstitute;
using Sensemaking.Host.Monitoring;
using Sensemaking.Host.Web;
using Sensemaking.Monitoring;
using Serilog;

namespace Sensemaking.Bdd.Web
{
    public abstract class FakeStartup : JsonApiStartup
    {
        protected FakeStartup()
        {
            Logger = Substitute.For<ILogger>();
        }

        protected override string ServiceName => Name;
        protected override ServiceDependency[] Dependencies => new[] { Dependency };
        protected override ILogger Logger { get; }

        private class FakeMonitor : IMonitor
        {
            public Availability Availability()
            {
                return Monitoring.Availability.Up();
            }

            public MonitorInfo Info => new MonitorInfo("Awesome Monitor", "Bob the monitor");
        }

        public ILogger SubstituteLogger => Logger;
        public string Name = "Json Web Api";
        public ServiceDependency Dependency = new ServiceDependency(new FakeMonitor());
    }
}