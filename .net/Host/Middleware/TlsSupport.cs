using System.Net;
using Microsoft.AspNetCore.Builder;

namespace Sensemaking.Host.Web
{
    internal static class TlsSupport
    {
        internal static IApplicationBuilder UseTls2AndHigher(this IApplicationBuilder app)
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            ServicePointManager.SecurityProtocol &= ~SecurityProtocolType.Tls11;
            ServicePointManager.SecurityProtocol &= ~SecurityProtocolType.Tls;
            return app;
        }
    }
}