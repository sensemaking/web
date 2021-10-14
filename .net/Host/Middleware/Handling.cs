using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Sensemaking.Http;
using Sensemaking.Web.Api;

namespace Sensemaking.Web.Host
{
    public static class Handling
    {
        internal static IServiceCollection AutoRegisterHandlers(this IServiceCollection services)
        {
            services.Scan(scan => scan.FromApplicationDependencies()
                .AddClasses(classes => classes.AssignableTo<IHandleGetRequests>()).As<IHandleGetRequests>()
                .AddClasses(classes => classes.AssignableTo<IHandleDeleteRequests>()).As<IHandleDeleteRequests>()
                .AddClasses(classes => classes.AssignableTo(typeof(IHandlePutRequests<>))).AsImplementedInterfaces()
                .AddClasses(classes => classes.AssignableTo(typeof(IHandlePostRequests<>))).AsImplementedInterfaces());
            return services;
        }

        internal static IApplicationBuilder MapHandlersToRoutes(this IApplicationBuilder app, Action<IEndpointRouteBuilder, IApplicationBuilder, RequestFactory> routeMapper)
        {
            app.UseEndpoints(endpoints => routeMapper(endpoints, app, app.ApplicationServices.GetRequiredService<RequestFactory>()));
            return app;
        }

        internal static void DefaultEndpointMapper(IEndpointRouteBuilder endpoints, IApplicationBuilder app, RequestFactory requestFactory)
        {
            app.ApplicationServices.GetServices<IHandleGetRequests>().ForEach(handler => endpoints.MapGet(handler.Route, context => handler.Get(requestFactory, context)).ApplyAuthorizationPolicy(handler));
            app.ApplicationServices.GetServices<IHandleDeleteRequests>().ForEach(handler => endpoints.MapDelete(handler.Route, context => handler.Delete(requestFactory, context)).ApplyAuthorizationPolicy(handler));
            app.ApplicationServices.GetServices<IPutRequestHandler>().ForEach(handler => endpoints.MapPut(handler.Route, context => handler.Execute(requestFactory, context)).ApplyAuthorizationPolicy(handler));
            app.ApplicationServices.GetServices<IRequestPostHandler>().ForEach(handler => endpoints.MapPost(handler.Route, context => handler.Execute(requestFactory, context)).ApplyAuthorizationPolicy(handler));
        }

        public static async Task Get(this IHandleGetRequests handler, RequestFactory requestFactory, HttpContext context)
        {
            var results = await handler.HandleAsync(requestFactory.Create(context));
            context.Response.ContentType = $"{MediaType.Json}; charset=utf-8";
            await context.Response.WriteAsync(results.Serialize());
        }

        public static async Task Delete(this IHandleDeleteRequests handler, RequestFactory requestFactory, HttpContext context)
        {
            context.Response.StatusCode = (int) await handler.HandleAsync(requestFactory.Create(context));
            await context.Response.CompleteAsync();
        }

        public static async Task Execute(this IHandleRequests handler, RequestFactory requestFactory, HttpContext context)
        {
            context.Response.StatusCode = (int) await handler.Execute(requestFactory.Create(context), await context.PayloadFor(handler));
            await context.Response.CompleteAsync();
        }

        private static async Task<HttpStatusCode> Execute(this IHandleRequests handler, Request request, object payload)
        {
            return await (handler.GetType().GetMethod("HandleAsync")!.Invoke(handler, System.Reflection.BindingFlags.DoNotWrapExceptions, null, new[] { request, payload }, null) as Task<HttpStatusCode>)!;
        }

        private static async Task<object> PayloadFor(this HttpContext context, IHandleRequests handler)
        {
            var payloadType = handler.GetType().GetInterfaces().Single(x => x.Name == typeof(IRequestCommandHandler<>).Name).GenericTypeArguments.Single();
            using var reader = new StreamReader(context.Request.Body);
            return (await reader.ReadToEndAsync()).Deserialize(payloadType);
        }

        private static void ApplyAuthorizationPolicy(this IEndpointConventionBuilder builder, IHandleRequests handler)
        {
            if (handler.AllowUnauthenicatedUsers())
                builder.RequireAuthorization(AuthorizationPolicies.NoAuthorization.Name);
        }
    }
}