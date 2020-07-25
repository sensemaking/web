using System;
using System.Reflection;
using System.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sensemaking.Host.Monitoring;
using Serilog;

namespace Sensemaking.Host.Web
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
            services.AddSingleton<ILogger>(Logger);
        }

        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app
                .UseLogger(Logger)
                .MapExceptionsToProblems()
                .UseStatusNotification(app.ApplicationServices.GetRequiredService<IMonitorServices>())
            .Request()
                .UseHttpsRedirection()
                .RejectNonTls2OrHigher()
                .RequireJsonAcceptance()
            .Routing()
                .AddIsAlive();
        }
    }

    internal static class Extensions
    {
        internal static IApplicationBuilder Request(this IApplicationBuilder app) { return app; }
        internal static IApplicationBuilder Routing(this IApplicationBuilder app) { return app.UseRouting(); }
    }
}
