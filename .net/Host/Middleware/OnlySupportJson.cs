using Microsoft.AspNetCore.Builder;

namespace Sensemaking.Host.Web
{
    internal static class OnlySupportJson
    {
        internal static IApplicationBuilder OnlyAcceptJson(this IApplicationBuilder app)
        {
            return app;
        }
    }
}