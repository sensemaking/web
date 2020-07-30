using System;
using System.Reflection;
using System.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Sensemaking.Host.Monitoring;
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
            services.AddSingleton(Logger);
            services.AddSingleton<IMonitorServices>(new ServiceMonitor(ServiceName, Dependencies));
            services.Scan(scan => scan.FromApplicationDependencies()
                .AddClasses(classes => classes.AssignableTo<IHandleGetRequests>()).As<IHandleGetRequests>()
                .AddClasses(classes => classes.AssignableTo<IHandlePutRequests>()).As<IHandlePutRequests>()
                .AddClasses(classes => classes.AssignableTo<IHandleDeleteRequests>()).As<IHandleDeleteRequests>()
                .AddClasses(classes => classes.AssignableTo<IHandlePostRequests>()).As<IHandlePostRequests>());
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
                .RequireJsonAcceptance()
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
