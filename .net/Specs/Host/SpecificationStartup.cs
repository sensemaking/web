using NSubstitute;
using Sensemaking.Bdd.Web;
using Sensemaking.Host.Monitoring;
using Sensemaking.Web.Host;
using Serilog;

namespace Sensemaking.Host.Web.Specs
{
    public class SpecificationStartup : ApiStartup
    {
        protected override IMonitorServices ServiceMonitor { get; } = new FakeServiceMonitor();
        public IMonitorServices FakeMonitor => ServiceMonitor;

        protected override ILogger Logger { get; } = Substitute.For<ILogger>();
        public ILogger SubstituteLogger => Logger;
    }
}