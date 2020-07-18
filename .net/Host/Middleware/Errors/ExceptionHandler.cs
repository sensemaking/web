using System.Net;
using Microsoft.AspNetCore.Builder;

namespace Sensemaking.Host.Web
{
    internal static class ExceptionHandler
    {
        internal static IApplicationBuilder UseProblemHandling(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(error => error.Run(async context =>
            {
                context.Response.StatusCode = (int) HttpStatusCode.ServiceUnavailable;
                await context.Response.CompleteAsync();
            }));

            return app;
        }
    }
}