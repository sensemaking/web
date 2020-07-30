using System;
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
        internal static IApplicationBuilder WireUpHandlers(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                app.ApplicationServices.GetServices<IHandleGetRequests>().ForEach(handler => endpoints.MapGet(handler.Route, handler.Get));
                app.ApplicationServices.GetServices<IHandlePutRequests>().ForEach(handler => endpoints.MapPut(handler.Route, handler.Execute));
                app.ApplicationServices.GetServices<IHandleDeleteRequests>().ForEach(handler => endpoints.MapDelete(handler.Route, handler.Execute));
                app.ApplicationServices.GetServices<IHandlePostRequests>().ForEach(handler => endpoints.MapPost(handler.Route, handler.Execute));
            });
            return app;
        }

        private static async Task Get(this IHandleGetRequests handler, HttpContext context)
        {
            var results = await handler.Handle();
            context.Response.ContentType = $"{MediaType.Json}; charset=utf-8";
            await context.Response.WriteAsync(results.Serialize());
        }

        private static async Task Execute(this IHandleCommandRequests handler, HttpContext context)
        {
            context.Response.StatusCode = (int) await handler.Handle();
            await context.Response.CompleteAsync();
        }
    } 
}