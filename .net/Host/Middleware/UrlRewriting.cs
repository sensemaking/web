using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Rewrite;

namespace Sensemaking.Web.Host
{
    public static class UrlRewriting
    {
        internal static IApplicationBuilder ForceWww(this IApplicationBuilder app)
        {
            var options = new RewriteOptions();

            options.AddRedirectToWwwPermanent();

            return app.UseRewriter(options);
        }
    }
}
