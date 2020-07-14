using System;
using System.Net;
using System.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Sensemaking.Host.Monitoring;

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
            app.UseHttpsRedirection();
            app.RemoveSupportForTls11AndLower();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/is-alive", context =>
                {
                    context.Response.Headers.Add("Content-Type", "application/json");
                    return context.Response.WriteAsync(new { status = "Service is up!" }.Serialize());
                });
            });
        }
    }

    internal static class ApplicationBuilderExtensions
    {
        internal static void RemoveSupportForTls11AndLower(this IApplicationBuilder app)
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            ServicePointManager.SecurityProtocol &= ~SecurityProtocolType.Tls11;
            ServicePointManager.SecurityProtocol &= ~SecurityProtocolType.Tls;
        }
    }
}
