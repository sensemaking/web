using System.Linq;
using System.Net;
using System.Serialization;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.Core;
using Sensemaking.Bdd;
using Sensemaking.Host.Monitoring;
using Serilog;

namespace Sensemaking.Host.Web.Specs
{
    public partial class JsonApiStartupSpecs
    {
        private const int wait_for_notifier = 500;

        private void service_has_started() { }

        private void it_has_dependencies() { }

        private void requesting(string accepts)
        {
            trying(() => get<object>("is-alive", ("Accept", accepts)));
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

        private void it_notifies_every_20_seconds()
        {
            ServiceNotification.Notifier.Heartbeat.Seconds.should_be(20);
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
