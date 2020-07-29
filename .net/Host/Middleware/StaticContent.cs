using Microsoft.AspNetCore.Builder;

namespace Sensemaking.Host.Web
{
    internal static class StaticContent
    {
        internal static IApplicationBuilder ServeStaticContent(this IApplicationBuilder app)
        {
            return app.UseDefaultFiles().UseStaticFiles();
        }
    } 
}