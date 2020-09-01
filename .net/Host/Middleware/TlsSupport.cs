using System.Net;
using Microsoft.AspNetCore.Builder;

namespace Sensemaking.Web.Host
{
    internal static class TlsSupport
    {
        public static IApplicationBuilder RejectNonTls2OrHigher(this IApplicationBuilder app)
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            ServicePointManager.SecurityProtocol &= ~SecurityProtocolType.Tls11;
            ServicePointManager.SecurityProtocol &= ~SecurityProtocolType.Tls;
            return app;
        }
    }
}