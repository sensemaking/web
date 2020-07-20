using System.Linq;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Sensemaking.Bdd;
using Sensemaking.Bdd.Web;
using Sensemaking.Host.Monitoring;
using Sensemaking.Http;
using Sensemaking.Monitoring;

namespace Sensemaking.Host.Web.Specs
{
    public partial class StartupSpecs
    {
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
        }

        private void it_monitors_them()
        {
            services.GetRequiredService<IMonitorServices>().Info.Name.should_be(FakeStartup.Name);
            ServiceNotification.Notifier.Monitor.Dependencies.Single().should_be(FakeStartup.Dependency);
            services.GetRequiredService<IMonitorServices>().should_be(ServiceNotification.Notifier.Monitor);
        }

        private void every_20_seconds()
        {
            ServiceNotification.Notifier.Monitor.Heartbeat.Seconds.should_be(20);
        }
    }

    internal static class ProtocolExtensions
    {
        internal static bool supports(this SecurityProtocolType protocol, SecurityProtocolType included)
        {
            return (protocol & included) == included;
        }
    }

    public class FakeStartup : JsonApiStartup
    {
        internal const string Name = "Json Web Api";
        internal static readonly ServiceDependency Dependency = new ServiceDependency(new FakeMonitor());

        protected override string ServiceName => Name;
        protected override ServiceDependency[] Dependencies => new[] { Dependency };

        private class FakeMonitor : IMonitor
        {
            public Availability Availability()
            {
                return Sensemaking.Monitoring.Availability.Up();
            }

            public MonitorInfo Info => new MonitorInfo("Awesome Monitor", "Bob the monitor");
        }
    }
}
