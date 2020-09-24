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
    public static class Handlers
    {
        public static IServiceCollection AutoRegisterHandlers(this IServiceCollection services)
        {
            services.Scan(scan => scan.FromApplicationDependencies()
                .AddClasses(classes => classes.AssignableTo<IHandleGetRequests>()).As<IHandleGetRequests>()
                .AddClasses(classes => classes.AssignableTo<IHandleDeleteRequests>()).As<IHandleDeleteRequests>()
                .AddClasses(classes => classes.AssignableTo(typeof(IHandlePutRequests<>))).AsImplementedInterfaces()
                .AddClasses(classes => classes.AssignableTo(typeof(IHandlePostRequests<>))).AsImplementedInterfaces());
            return services;
        }

        public static IApplicationBuilder MapHandlersToRoutes(this IApplicationBuilder app, Action<IEndpointRouteBuilder, IApplicationBuilder, RequestFactory> routeMapper)
        {
            app.UseEndpoints(endpoints => routeMapper(endpoints, app, app.ApplicationServices.GetRequiredService<RequestFactory>()));
            return app;
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

        public static async Task Execute(this IRequestCommandHandler handler, RequestFactory requestFactory, HttpContext context)
        {
            context.Response.StatusCode = (int) await handler.Execute(requestFactory.Create(context), await context.PayloadFor(handler));
            await context.Response.CompleteAsync();
        }

        public static async Task<HttpStatusCode> Execute(this IRequestCommandHandler handler, Request request, object payload)
        {
            return await (handler.GetType().GetMethod("HandleAsync")!.Invoke(handler, System.Reflection.BindingFlags.DoNotWrapExceptions, null, new[] { request, payload }, null) as Task<HttpStatusCode>)!;
        }

        public static async Task<object> PayloadFor(this HttpContext context, IRequestCommandHandler handler)
        {
            var payloadType = handler.GetType().GetInterfaces().Single(x => x.Name == typeof(IRequestCommandHandler<>).Name).GenericTypeArguments.Single();
            using var reader = new StreamReader(context.Request.Body);
            return (await reader.ReadToEndAsync()).Deserialize(payloadType);
        }
    }
}