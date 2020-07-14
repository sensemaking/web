using System.Linq;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Sensemaking.Bdd;
using Sensemaking.Host.Monitoring;
using Sensemaking.Monitoring;

namespace Sensemaking.Host.Web.Specs
{
    public partial class StartupSpecs
    {
        private static readonly ServiceDependency Dependency = new ServiceDependency(new FakeMonitor());

        private void pre_tls12_protocols_are_refused()
        {
            #pragma warning disable CS0618
            ServicePointManager.SecurityProtocol.supports(SecurityProtocolType.Tls12).should_be_true();
            ServicePointManager.SecurityProtocol.supports(SecurityProtocolType.Tls11).should_be_false();
            ServicePointManager.SecurityProtocol.supports(SecurityProtocolType.Tls).should_be_false();
            ServicePointManager.SecurityProtocol.supports(SecurityProtocolType.Ssl3).should_be_false();
        }

        private void service_dependencies()
        {
            Web.Configure(Dependency);
        }

        private void it_monitors_them()
        {
            Web.StatusNotifier.Monitor.Dependencies.Single().should_be(Dependency);
        }

        private void every_20_seconds()
        {
            Web.StatusNotifier.Monitor.Heartbeat.Seconds.should_be(20);
        }

        internal class FakeMonitor : IMonitor
        {
            public Availability Availability()
            {
                return Sensemaking.Monitoring.Availability.Up();
            }

            public MonitorInfo Info => new MonitorInfo("Awesome Monitor", "Bob the monitor");
        }
    }

    internal static class ProtocolExtensions
    {
        internal static bool supports(this SecurityProtocolType protocol, SecurityProtocolType included)
        {
            return (protocol & included) == included;
        }
    }
}
