using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Sensemaking.Web.Host
{
    public static class Acceptability
    {
        private const string JsonMatchPattern = @"application\/([\S]+\+)*json";

        internal static IApplicationBuilder RequireJsonAcceptance(this IApplicationBuilder app)
        {
            app.Use((context, next) =>
            {
                if(context.Request.AcceptsAnything() || context.Request.IsAcceptable())
                    return next();

                context.Response.StatusCode = (int) HttpStatusCode.NotAcceptable;
                return  context.Response.CompleteAsync();
            });
            return app;
        }

        private static bool AcceptsAnything(this HttpRequest request)
        {
            return request.Headers.Accept().None() || request.Headers.Accept().Any(header => header.Contains("*/*"));
        }

        private static bool IsAcceptable(this HttpRequest request)
        {
            return request.Headers.Accept().Any(header => Regex.IsMatch(header, JsonMatchPattern));
        }
        
        private static StringValues Accept(this IHeaderDictionary headers)
        {
            return headers["Accept"];
        }
    }
}