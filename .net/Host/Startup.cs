using System;
using System.Net;
using System.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Sensemaking.Host.Monitoring;
using Sensemaking.Host.Web.Errors;
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
            ServiceStatus.Notifier = new ServiceStatusNotifier(monitor);
        }

        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHttpsRedirection()
                .UseTls2AndHigherOnly()
                .UseProblemHandling()
                .UseRouting();

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

    internal static class ServiceStatus
    {
        internal static ServiceStatusNotifier? Notifier { get; set; }
    }

    internal static class ApplicationBuilderExtensions
    {
        internal static IApplicationBuilder UseTls2AndHigherOnly(this IApplicationBuilder app)
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            ServicePointManager.SecurityProtocol &= ~SecurityProtocolType.Tls11;
            ServicePointManager.SecurityProtocol &= ~SecurityProtocolType.Tls;
            return app;
        }
    }
}
