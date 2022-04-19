using System;
using System.Net;
using System.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sensemaking.Host.Monitoring;
using Sensemaking.Http;
using Sensemaking.Monitoring;
using Sensemaking.Web.Api;
using Serilog;

namespace Sensemaking.Web.Host
{
    public static class ExceptionHandling
    {
        public static IServiceCollection ProvideExceptionHandling(this IServiceCollection services, ExceptionHandler handler)
        {
            services.Replace(ServiceDescriptor.Singleton(handler));
            return services;
        }

        internal static IApplicationBuilder MapExceptionsToProblems(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(error => error.Run(context =>
            {
                var feature = context.Features.Get<IExceptionHandlerFeature>();
                var exceptionHandler = app.ApplicationServices.GetRequiredService<ExceptionHandler>();
                var (statusCode, problem) = exceptionHandler.HandleException(feature.Error);
                context.Response.StatusCode = (int) statusCode;

                if (statusCode == HttpStatusCode.InternalServerError)
                    Log.Logger.Error(AlertFactory.UnknownErrorOccured(app.ApplicationServices.GetRequiredService<IMonitorServices>().Info, feature.Error).Serialize());

                if (problem == Problem.Empty)
                    return context.Response.CompleteAsync();

                context.Response.ContentType = MediaType.JsonProblem;
                return context.Response.WriteAsync(problem.Serialize());
            }));

            return app;
        }
    }

    public class ExceptionHandler
    {
        public (HttpStatusCode, Problem) HandleException(Exception exception)
        {
            return exception switch
            {
                WhoAreYouException ex => Handle(ex),
                AccessException ex => Handle(ex),
                NotFoundException ex => Handle(ex),
                ServiceAvailabilityException ex => Handle(ex),
                ValidationException  ex => Handle(ex),
                SerializationException ex => Handle(ex),
                ConflictException ex => Handle(ex),
                _ => Handle(exception),
            };
        }

        protected virtual (HttpStatusCode, Problem) Handle(WhoAreYouException ex)
        {
            return (HttpStatusCode.Unauthorized, Problem.Empty);
        }

        protected virtual (HttpStatusCode, Problem) Handle(AccessException ex)
        {
            return (HttpStatusCode.Forbidden, Problem.Empty);
        }

        protected virtual (HttpStatusCode, Problem) Handle(NotFoundException ex)
        {
            return (HttpStatusCode.NotFound, Problem.Empty);
        }

        protected virtual (HttpStatusCode, Problem) Handle(ServiceAvailabilityException ex)
        {
            return (HttpStatusCode.ServiceUnavailable, Problem.Empty);
        }

        protected virtual (HttpStatusCode, Problem) Handle(ValidationException ex)
        {
            return (HttpStatusCode.BadRequest, new Problem("The request could not be correctly validated.", ex.Errors));
        }

        protected virtual (HttpStatusCode, Problem) Handle(SerializationException ex)
        {
            return (HttpStatusCode.BadRequest, new Problem("The request could not be correctly serialized.", ex.Errors));
        }

        protected virtual (HttpStatusCode, Problem) Handle(ConflictException ex)
        {
            return (HttpStatusCode.Conflict, new Problem("Fulfilling the request would cause a conflict.", ex.Errors));
        }

        protected virtual (HttpStatusCode, Problem) Handle(Exception ex)
        {
            return (HttpStatusCode.InternalServerError, Problem.Empty);
        }
    }
}