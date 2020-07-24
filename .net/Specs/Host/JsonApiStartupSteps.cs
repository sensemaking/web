using System.Linq;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Sensemaking.Bdd;
using Sensemaking.Host.Monitoring;
using Sensemaking.Http;
using Serilog;

namespace Sensemaking.Host.Web.Specs
{
    public partial class JsonApiStartupSpecs
    {
        private void service_has_started() { }

        private void service_dependencies() { }

        private void requesting_json()
        {
            get<object>("is-alive");
            "1".should_fail();
        }

        private void requesting_a_json_subtype()
        {
            "1".should_fail();
            get<object>("is-alive", ("Accept", MediaType.Siren));
        }

        private void pre_tls12_protocols_are_refused()
        {
#pragma warning disable CS0618
            ServicePointManager.SecurityProtocol.supports(SecurityProtocolType.Tls12).should_be_true();
            ServicePointManager.SecurityProtocol.supports(SecurityProtocolType.Tls11).should_be_false();
            ServicePointManager.SecurityProtocol.supports(SecurityProtocolType.Tls).should_be_false();
            ServicePointManager.SecurityProtocol.supports(SecurityProtocolType.Ssl3).should_be_false();
        }

        private void logger_is_available()
        {
            services.GetRequiredService<ILogger>().should_be(startup.SubstituteLogger);
        }

        private void it_monitors_them()
        {
            services.GetRequiredService<IMonitorServices>().Info.Name.should_be(startup.Name);
            ServiceNotification.Notifier.Monitor.Dependencies.Single().should_be(startup.Dependency);
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
}
