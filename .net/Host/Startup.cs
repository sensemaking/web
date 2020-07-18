using System;
using System.Net;
using System.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Sensemaking.Host.Monitoring;
using Sensemaking.Monitoring;

namespace Sensemaking.Host.Web
{
    public class Startup
    {
        protected virtual ServiceDependency[] Dependencies => Array.Empty<ServiceDependency>();

        public virtual void ConfigureServices(IServiceCollection services)
        {
            var monitor = new ServiceMonitor(Period.FromSeconds(20), Dependencies);
            services.AddSingleton<IMonitorServices>(monitor);
        }

        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStatusNotification(app.ApplicationServices.GetRequiredService<IMonitorServices>())
                .UseHttpsRedirection()
                .UseTls2AndHigher()
                .UseRouting()
                .UseProblemHandling();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/is-alive", async context =>
                {
                    var monitor = app.ApplicationServices.GetRequiredService<IMonitorServices>();
                    if(!monitor.Availability())
                        throw new ServiceAvailabilityException();

                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(new { status = "Service is up!" }.Serialize());
                });
            });
        }
    }
}
