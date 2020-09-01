using Microsoft.AspNetCore.Builder;

namespace Sensemaking.Web.Host
{
    public static class StaticContent
    {
        public static IApplicationBuilder ServeStaticContent(this IApplicationBuilder app)
        {
            return app.UseDefaultFiles().UseStaticFiles();
        }
    } 
}
