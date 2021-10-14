using System;
using System.Collections.Generic;
using System.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Sensemaking.Host.Monitoring;
using Serilog;

namespace Sensemaking.Web.Host
{
    public abstract class ApiStartup
    {
        protected IConfiguration Configuration { get; private set; }
        protected virtual IAuthenticateUsers Authentication { get; } = UseAuthentication.None();
        protected abstract IMonitorServices ServiceMonitor { get; }
        protected abstract ILogger Logger { get; }

        protected ApiStartup(IConfiguration configuration)
        {
            Configuration = configuration;
            Configuration = configuration.Apply(Authentication.ApplyConfiguration());
            Serialization.Configure();
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services
                .ProvideLogging(Logger)
                .ProvideExceptionHandling(new ExceptionHandler())
                .ProvideMonitoring(ServiceMonitor)
                .ProvideRequestCreation(new RequestFactory())
                .ProvideAuthentication(Authentication, Configuration)
                .AutoRegisterHandlers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            ConfigureApplication(app);
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
                .ResolveAuthentication(Authentication)
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
        public static IConfiguration Apply(this IConfiguration configuration, IEnumerable<KeyValuePair<string, string>> settings)
        {
            return new ConfigurationBuilder().AddConfiguration(configuration).AddInMemoryCollection(settings).Build();
        }

        internal static IApplicationBuilder AddMiddleware(this IApplicationBuilder app, Func<IApplicationBuilder, IApplicationBuilder> addMiddleware)
        {
            return addMiddleware(app);
        }

        internal static IApplicationBuilder Request(this IApplicationBuilder app) { return app; }
        internal static IApplicationBuilder Routing(this IApplicationBuilder app) { return app.UseRouting(); }
    }
}
