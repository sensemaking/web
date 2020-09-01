using System.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Sensemaking.Host.Monitoring;
using Sensemaking.Http;
using Sensemaking.Monitoring;

namespace Sensemaking.Web.Host
{
    public static class IsAlive
    {
        public static IApplicationBuilder AddIsAlive(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/is-alive", async context =>
                {
                    var monitor = app.ApplicationServices.GetRequiredService<IMonitorServices>();
                    if (!monitor.Availability())
                        throw new ServiceAvailabilityException();

                    context.Response.ContentType = $"{MediaType.Json}; charset=utf-8";
                    await context.Response.WriteAsync(new { status = "Service is up!" }.Serialize());
                });
            });
            return app;
        }
    } 
}