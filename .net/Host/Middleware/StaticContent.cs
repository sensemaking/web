using Microsoft.AspNetCore.Builder;

namespace Sensemaking.Web.Host
{
    internal static class StaticContent
    {
        internal static IApplicationBuilder ServeStaticContent(this IApplicationBuilder app)
        {
            return app.UseDefaultFiles().UseStaticFiles();
        }
    } 
}