using System;
using System.Reflection;
using System.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Sensemaking.Host.Monitoring;
using Serilog;

namespace Sensemaking.Web.Host
{
    public abstract class ApiStartup
    {
        protected abstract string ServiceName { get; }
        protected abstract ServiceDependency[] Dependencies { get; }
        protected abstract ILogger Logger { get; }

        protected ApiStartup()
        {
            Serialization.Configure();
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services
                .AddLogger(Logger)
                .AddMonitor(ServiceName, Dependencies)
                .AddRequestFactory(new RequestFactory())
                .AutoRegisterHandlers();
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
                .MapHandlersToRoutes()
                .AddIsAlive();
        }
    }

    public static class Extensions
    {
        public static IApplicationBuilder Request(this IApplicationBuilder app) { return app; }
        public static IApplicationBuilder Routing(this IApplicationBuilder app) { return app.UseRouting(); }
    }
}
