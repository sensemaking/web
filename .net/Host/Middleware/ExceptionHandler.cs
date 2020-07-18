﻿using System;
using System.Net;
using System.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Sensemaking.Http;
using Sensemaking.Monitoring;

namespace Sensemaking.Host.Web
{
    internal static class ExceptionHandler
    {
        internal static IApplicationBuilder MapExceptionsToProblems(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(error => error.Run(context =>
            {
                var feature = context.Features.Get<IExceptionHandlerFeature>();
                var (statusCode, problem) = feature.Error switch
                {
                    NotAllowedException _ => (HttpStatusCode.Forbidden, Problem.Empty),
                    NotFoundException _ => (HttpStatusCode.NotFound, Problem.Empty),
                    ServiceAvailabilityException _ => (HttpStatusCode.ServiceUnavailable, Problem.Empty),
                    ValidationException ex => (HttpStatusCode.BadRequest, new Problem("The request could not be correctly validated.", ex.Errors)),
                    ConflictException ex => (HttpStatusCode.Conflict, new Problem("Fulfilling the request would cause a conflict.", ex.Errors)),
                    _ => (HttpStatusCode.InternalServerError, Problem.Empty)
                };
                context.Response.StatusCode = (int)statusCode;

                if (problem == Problem.Empty)
                    return context.Response.CompleteAsync();

                context.Response.ContentType = MediaType.JsonProblem;
                return context.Response.WriteAsync(problem.Serialize());
            }));

            return app;
        }
    }
}