using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sensemaking.Http;
using Sensemaking.Web.Api;

namespace Sensemaking.Web.Host
{
    internal static class Handlers
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

        internal static IApplicationBuilder MapHandlersToRoutes(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                app.ApplicationServices.GetServices<IHandleGetRequests>().ForEach(handler => endpoints.MapGet(handler.Route, handler.Get));
                app.ApplicationServices.GetServices<IHandleDeleteRequests>().ForEach(handler => endpoints.MapDelete(handler.Route, handler.Execute));
                app.ApplicationServices.GetServices<IPutRequestHandler>().ForEach(handler => endpoints.MapPut(handler.Route, handler.Execute));
                app.ApplicationServices.GetServices<IRequestPostHandler>().ForEach(handler => endpoints.MapPost(handler.Route, handler.Execute));
            });
            return app;
        }

        private static async Task Get(this IHandleGetRequests handler, HttpContext context)
        {
            var request = new Request(Append(context.Request.RouteValues, context.Request.Query));
            var results = await handler.Handle(request);
            context.Response.ContentType = $"{MediaType.Json}; charset=utf-8";
            await context.Response.WriteAsync(results.Serialize());
        }

        private static async Task Execute(this IHandleDeleteRequests handler, HttpContext context)
        {
            context.Response.StatusCode = (int) await handler.Handle();
            await context.Response.CompleteAsync();
        }

        private static async Task Execute(this IRequestCommandHandler handler, HttpContext context)
        {
            var request = new RequestFactory().Create(context);
            var payload = await context.PayloadFor(handler);
            context.Response.StatusCode = (int) await handler.Execute(request, payload);
            await context.Response.CompleteAsync();
        }

        private static IReadOnlyDictionary<string, object> Append(this IDictionary<string, object> routeValues, IQueryCollection queryValues)
        {
            var r = queryValues.ToDictionary(x => x.Key, x => x.Value.First() as object);
            return routeValues.Concat(r).ToDictionary(x => x.Key, x => x.Value);
        }

        private static async Task<object> PayloadFor(this HttpContext context, IRequestCommandHandler handler)
        {
            var payloadType = handler.GetType().GetInterfaces().Single(x=>x.Name == typeof(IRequestCommandHandler<>).Name).GenericTypeArguments.Single();
            using var reader = new StreamReader(context.Request.Body);
            return (await reader.ReadToEndAsync()).Deserialize(payloadType);
        }

        private static async Task<HttpStatusCode> Execute(this IRequestCommandHandler handler, Request request, object payload)
        {
            return await (handler.GetType().GetMethod("Handle")!.Invoke(handler, new [] {request, payload}) as Task<HttpStatusCode>)!;
        }
    }

    public class RequestFactory
    {
        public Request Create(HttpContext context)
        {
            var r = context.Request.Query.ToDictionary(x => x.Key, x => x.Value.First() as object);
            return new Request(context.Request.RouteValues.Concat(r).ToDictionary(x => x.Key, x => x.Value));
        }
    }
}