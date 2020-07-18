using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
        }

        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStatusNotification(app.ApplicationServices.GetRequiredService<IMonitorServices>())
            .Request()
                .UseHttpsRedirection()
                .UseTls2AndHigher()
            .Response() 
                .UseProblemHandling()
            .Routing()
                .AddIsAlive();
        }
    }

    internal static class Extensions
    {
        internal static IApplicationBuilder Request(this IApplicationBuilder app) { return app; }
        internal static IApplicationBuilder Response(this IApplicationBuilder app) { return app; }
        internal static IApplicationBuilder Routing(this IApplicationBuilder app) { return app.UseRouting(); }
    }
}
