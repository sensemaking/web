using Sensemaking.Bdd;
using System.Net;

namespace Sensemaking.Web.Specs
{
    public partial class StartupSpecs 
    {
        private void pre_tls12_protocols_are_not_supported()
        {
            ServicePointManager.SecurityProtocol.supports(SecurityProtocolType.Tls12).should_be_true();
            ServicePointManager.SecurityProtocol.supports(SecurityProtocolType.Tls11).should_be_false();
            ServicePointManager.SecurityProtocol.supports(SecurityProtocolType.Tls).should_be_false();
            ServicePointManager.SecurityProtocol.supports(SecurityProtocolType.Ssl3).should_be_false();
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
