using System;
using System.Net.Http;
using System.Reflection;
using System.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
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
                .UseStatusNotification(app.ApplicationServices.GetRequiredService<IMonitorServices>())
            .Request()
                .UseHttpsRedirection()
                .RejectNonTls2OrHigher()
                .RequireJsonAcceptance()
            .Routing()
                .AddIsAlive();

            app.UseEndpoints(endpoints =>
            {
                app.ApplicationServices.GetServices<IHandleGetRequests>().ForEach(handler => endpoints.MapGet(handler.Route, ctx => Get(ctx, handler)));
                app.ApplicationServices.GetServices<IHandlePutRequests>().ForEach(handler => endpoints.MapPut(handler.Route, ctx => Execute(ctx, handler)));
                app.ApplicationServices.GetServices<IHandlePostRequests>().ForEach(handler => endpoints.MapPost(handler.Route, ctx => Execute(ctx, handler)));
            });
        }

        private static async Task Get(HttpContext context, IHandleGetRequests handler)
        {
            var results = await handler.Handle();
            context.Response.ContentType = $"{MediaType.Json}; charset=utf-8";
            await context.Response.WriteAsync(results.Serialize());
        }

        private static async Task Execute(HttpContext context, IHandleCommandRequests handler)
        {
            context.Response.StatusCode = (int) await handler.Handle();
            await context.Response.CompleteAsync();
        }
    }

    internal static class Extensions
    {
        internal static IApplicationBuilder Request(this IApplicationBuilder app) { return app; }
        internal static IApplicationBuilder Routing(this IApplicationBuilder app) { return app.UseRouting(); }
    }
}
