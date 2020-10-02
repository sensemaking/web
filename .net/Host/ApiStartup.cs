using System;
using System.Reflection;
using System.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Sensemaking.Host.Monitoring;
using Sensemaking.Web.Api;
using Serilog;

namespace Sensemaking.Web.Host
{
    public abstract class ApiStartup
    {
        protected abstract IMonitorServices ServiceMonitor { get; }
        protected abstract ILogger Logger { get; }

        protected ApiStartup()
        {
            Serialization.Configure();
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services
                .AddExceptionHandler(new ExceptionHandler())
                .AddLogger(Logger)
                .AddMonitor(ServiceMonitor)
                .AddRequestFactory(new RequestFactory())
                .AutoRegisterHandlers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            this.ConfigureApplication();

            app
                .UseLogger(Logger)
                .MapExceptionsToProblems()
                .ScheduleStatusNotification(app.ApplicationServices.GetRequiredService<IMonitorServices>(), Period.FromSeconds(20))
            .Request()
                .UseHttpsRedirection()
                .RejectNonTls2OrHigher()
                .RequireJsonAcceptance()
            .Routing()
                .AddMiddleware(AdditionalMiddleware)
                .MapHandlersToRoutes(MapHandlersToEndpoints)
                .AddIsAlive();
        }

        public virtual void ConfigureApplication() { }

        protected virtual IApplicationBuilder AdditionalMiddleware(IApplicationBuilder app) { return app; }

        protected virtual void MapHandlersToEndpoints(IEndpointRouteBuilder endpoints, IApplicationBuilder app, RequestFactory requestFactory)
        {
            app.ApplicationServices.GetServices<IHandleGetRequests>().ForEach(handler => endpoints.MapGet(handler.Route, context => handler.Get(requestFactory, context)));
            app.ApplicationServices.GetServices<IHandleDeleteRequests>().ForEach(handler => endpoints.MapDelete(handler.Route, context => handler.Delete(requestFactory, context)));
            app.ApplicationServices.GetServices<IPutRequestHandler>().ForEach(handler => endpoints.MapPut(handler.Route, context => handler.Execute(requestFactory, context)));
            app.ApplicationServices.GetServices<IRequestPostHandler>().ForEach(handler => endpoints.MapPost(handler.Route, context => handler.Execute(requestFactory, context)));
        }
    }

    internal static class Extensions
    {
        public static IApplicationBuilder AddMiddleware(this IApplicationBuilder app, Func<IApplicationBuilder, IApplicationBuilder> addMiddleware)
        {
            return addMiddleware(app);
        }

        public static IApplicationBuilder Request(this IApplicationBuilder app) { return app; }
        public static IApplicationBuilder Routing(this IApplicationBuilder app) { return app.UseRouting(); }
    }
}
