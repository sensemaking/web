using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
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
            var request = new Requests(Append(context.Request.RouteValues, context.Request.Query));
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
            using (var reader = new StreamReader(context.Request.Body))
                context.Response.StatusCode = (int) await handler.HandleJson((await reader.ReadToEndAsync()));

            await context.Response.CompleteAsync();
        }

        private static IReadOnlyDictionary<string, object> Append(this IDictionary<string, object> routeValues, IQueryCollection queryValues)
        {
            var r = queryValues.ToDictionary(x => x.Key, x => x.Value.First() as object);
            return routeValues.Concat(r).ToDictionary(x => x.Key, x => x.Value);
        }
    }
}