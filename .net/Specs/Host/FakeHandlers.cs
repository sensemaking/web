using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Routing;
using Sensemaking.Web.Api;

namespace Sensemaking.Host.Web.Specs
{
    public class FakeGetter : IHandleGetRequests
    {
        public readonly struct Response
        {
            public Response(string content)
            {
                Content = content;
            }

            public string Content { get; }
        }

        public static readonly string RouteKey = "routeKey";
        public static readonly string QueryKey = "queryKey";
        public static readonly string Url = "/get";
        public static Response TheResponse = new Response("Anything will do");

        public string Route => $"/get/{{{RouteKey}}}";

        public async Task<object> Handle(Request parameters)
        {
            if(parameters.Values[RouteKey] == null || parameters.Values[QueryKey] == null)
                throw new Exception("Route values or query string were not provided.");

            return await Task.FromResult(TheResponse);
        }
    }

    public class FakePutter : IHandlePutRequests<FakePayload>
    {
        public static readonly string Url = "/put";
        public static readonly HttpStatusCode ResponseStatusCode = HttpStatusCode.Accepted;

        public string Route => Url;

        public async Task<HttpStatusCode> Handle(Request request, FakePayload payload)
        {
            if(payload.Content.IsNullOrEmpty())
                throw new Exception("Payload was not provided.");

            return await Task.FromResult(ResponseStatusCode);
        }
    }

    public class FakeDeleter : IHandleDeleteRequests
    {
        public static readonly string Url = "/delete";
        public static readonly HttpStatusCode ResponseStatusCode = HttpStatusCode.NoContent;

        public string Route => Url;

        public async Task<HttpStatusCode> Handle()
        {
            return await Task.FromResult(ResponseStatusCode);
        }
    }

    public class FakePoster : IHandlePostRequests<FakePayload>
    {
        public static readonly string Url = "/post";
        public static readonly HttpStatusCode ResponseStatusCode = HttpStatusCode.Created;

        public string Route => Url;

        public async Task<HttpStatusCode> Handle(Request request, FakePayload payload)
        {
            if(payload.Content.IsNullOrEmpty())
                throw new Exception("Payload was not provided.");

            return await Task.FromResult(ResponseStatusCode);
        }
    }

    public class FakePayload
    {
        public FakePayload(string content)
        {
            Content = content;
        }

        public string Content { get; private set; }
    }
}