using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
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

        public static IApplicationBuilder MapHandlersToRoutes(this IApplicationBuilder app)
        {
            var requestFactory = app.ApplicationServices.GetRequiredService<RequestFactory>();
            app.UseEndpoints(endpoints =>
            {
                app.ApplicationServices.GetServices<IHandleGetRequests>().ForEach(handler => endpoints.MapGet(handler.Route, context => handler.Get(requestFactory, context)));
                app.ApplicationServices.GetServices<IHandleDeleteRequests>().ForEach(handler => endpoints.MapDelete(handler.Route, context => handler.Delete(requestFactory, context)));
                app.ApplicationServices.GetServices<IPutRequestHandler>().ForEach(handler => endpoints.MapPut(handler.Route, context => handler.Execute(requestFactory, context)));
                app.ApplicationServices.GetServices<IRequestPostHandler>().ForEach(handler => endpoints.MapPost(handler.Route, context => handler.Execute(requestFactory, context)));
            });
            return app;
        }

        public static async Task Get(this IHandleGetRequests handler, RequestFactory requestFactory, HttpContext context)
        {
            var results = await handler.Handle(requestFactory.Create(context));
            context.Response.ContentType = $"{MediaType.Json}; charset=utf-8";
            await context.Response.WriteAsync(results.Serialize());
        }

        public static async Task Delete(this IHandleDeleteRequests handler, RequestFactory requestFactory, HttpContext context)
        {
            context.Response.StatusCode = (int) await handler.Handle(requestFactory.Create(context));
            await context.Response.CompleteAsync();
        }

        public static async Task Execute(this IRequestCommandHandler handler, RequestFactory requestFactory, HttpContext context)
        {
            context.Response.StatusCode = (int) await handler.Execute(requestFactory.Create(context), await context.PayloadFor(handler));
            await context.Response.CompleteAsync();
        }

        public static async Task<HttpStatusCode> Execute(this IRequestCommandHandler handler, Request request, object payload)
        {
            return await (handler.GetType().GetMethod("Handle")!.Invoke(handler, System.Reflection.BindingFlags.DoNotWrapExceptions, null, new[] { request, payload }, null) as Task<HttpStatusCode>)!;
        }

        public static async Task<object> PayloadFor(this HttpContext context, IRequestCommandHandler handler)
        {
            var payloadType = handler.GetType().GetInterfaces().Single(x => x.Name == typeof(IRequestCommandHandler<>).Name).GenericTypeArguments.Single();
            using var reader = new StreamReader(context.Request.Body);
            return (await reader.ReadToEndAsync()).Deserialize(payloadType);
        }
    }
}