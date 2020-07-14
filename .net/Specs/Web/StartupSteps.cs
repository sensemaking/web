using Sensemaking.Bdd;
using System.Net;

namespace Sensemaking.Web.Specs
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

        private void service_monitor_is_used()
        {
            //bootstrapper.GetContainer().Resolve<IMonitor>().should_be(bootstrapper.GetMonitor);
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
