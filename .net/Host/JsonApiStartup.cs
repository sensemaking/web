using System;
using System.Net.Http;
using System.Reflection;
using System.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Sensemaking.Host.Monitoring;
using Sensemaking.Http;
using Sensemaking.Web.Api;
using Serilog;

namespace Sensemaking.Web.Host
{
    public abstract class JsonApiStartup
    {
        protected virtual string ServiceName => Assembly.GetExecutingAssembly().GetName().Name!;
        protected virtual ServiceDependency[] Dependencies => Array.Empty<ServiceDependency>();
        protected abstract ILogger Logger { get; }

        protected JsonApiStartup()
        {
            Serialization.Configure();
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            var monitor = new ServiceMonitor(ServiceName, Dependencies);
            services.AddSingleton<IMonitorServices>(monitor);
            services.AddSingleton(Logger);
        }

        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app
                .UseLogger(Logger)
                .MapExceptionsToProblems()
                .ScheduleStatusNotification(app.ApplicationServices.GetRequiredService<IMonitorServices>(), Period.FromSeconds(20))
            .Request()
                .UseHttpsRedirection()
                .RejectNonTls2OrHigher()
                .RequireJson()
            .Routing()
                .WireUpHandlers()
                .AddIsAlive();
        }
    }

    internal static class Extensions
    {
        internal static IApplicationBuilder Request(this IApplicationBuilder app) { return app; }
        internal static IApplicationBuilder Routing(this IApplicationBuilder app) { return app.UseRouting(); }
    }
}
