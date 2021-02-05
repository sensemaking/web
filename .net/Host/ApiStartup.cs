using System;
using System.Reflection;
using System.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Sensemaking.Host.Monitoring;
using Sensemaking.Web.Api;
using Serilog;

namespace Sensemaking.Web.Host
{
    public abstract class ApiStartup
    {
        protected IConfiguration Configuration { get; private set; }
        protected abstract IMonitorServices ServiceMonitor { get; }
        protected abstract ILogger Logger { get; }

        protected ApiStartup()
        {
            Serialization.Configure();
            Configuration = null!;
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services
                .ProvideLogging(Logger)
                .ProvideExceptionHandling(new ExceptionHandler())
                .ProvideMonitoring(ServiceMonitor)
                .ProvideRequestCreation(new RequestFactory())
                .AutoRegisterHandlers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            ConfigureApplication(app);
            Configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();

            app
                .UseLogger()
                .MapExceptionsToProblems()
                .ScheduleStatusNotification(Period.FromSeconds(20))
            .Request()
                .UseHttpsRedirection()
                .RejectNonTls2OrHigher()
                .RequireJsonAcceptance()
            .Routing()
                .AddMiddleware(AdditionalMiddleware)
                .MapHandlersToRoutes(MapHandlersToEndpoints)
                .AddIsAlive();
        }

        public virtual void ConfigureApplication(IApplicationBuilder app) { }

        protected virtual IApplicationBuilder AdditionalMiddleware(IApplicationBuilder app) { return app; }

        protected virtual void MapHandlersToEndpoints(IEndpointRouteBuilder endpoints, IApplicationBuilder app, RequestFactory requestFactory)
        {
            Handling.DefaultEndpointMapper(endpoints, app, requestFactory);
        }
    }

    internal static class Extensions
    {
        internal static IApplicationBuilder AddMiddleware(this IApplicationBuilder app, Func<IApplicationBuilder, IApplicationBuilder> addMiddleware)
        {
            return addMiddleware(app);
        }

        internal static IApplicationBuilder Request(this IApplicationBuilder app) { return app; }
        internal static IApplicationBuilder Routing(this IApplicationBuilder app) { return app.UseRouting(); }
    }
}
