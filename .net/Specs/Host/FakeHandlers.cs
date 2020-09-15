using System;
using System.Net;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Sensemaking.Web.Api;

namespace Sensemaking.Host.Web.Specs
{
    public static class FakeKeys
    {
        public static readonly string RouteKey = "routeKey";
        public static readonly string QueryKey = "queryKey";

        public static void Verify(Request request)
        {
            if(!request.Values.ContainsKey(RouteKey) || !request.Values.ContainsKey(QueryKey))
                throw new ValidationException("Route values or query string were not provided.");
        }
    }
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

        public static readonly string Url = "/get";
        public static Response TheResponse = new Response("Anything will do");

        public string Route => $"{Url}/{{{FakeKeys.RouteKey}}}";

        public async Task<object> HandleAsync(Request request)
        {
            FakeKeys.Verify(request);
            return await Task.FromResult(TheResponse);
        }
    }

    public class FakePutter : IHandlePutRequests<FakePayload>
    {
        public static readonly string Url = "/put";
        public static readonly HttpStatusCode ResponseStatusCode = HttpStatusCode.Accepted;

        public string Route => $"{Url}/{{{FakeKeys.RouteKey}}}";

        public async Task<HttpStatusCode> HandleAsync(Request request, FakePayload payload)
        {
            FakeKeys.Verify(request);

            if(payload.Content.IsNullOrEmpty())
                throw new Exception("Payload was not provided.");

            return await Task.FromResult(ResponseStatusCode);
        }
    }

    public class FakeDeleter : IHandleDeleteRequests
    {
        public static readonly string Url = "/delete";
        public static readonly HttpStatusCode ResponseStatusCode = HttpStatusCode.NoContent;

        public string Route => $"{Url}/{{{FakeKeys.RouteKey}}}";

        public async Task<HttpStatusCode> HandleAsync(Request request)
        {
            FakeKeys.Verify(request);

            return await Task.FromResult(ResponseStatusCode);
        }
    }

    public class FakePoster : IHandlePostRequests<FakePayload>
    {
        public static readonly string Url = "/post";
        public static readonly HttpStatusCode ResponseStatusCode = HttpStatusCode.Created;

        public string Route => $"{Url}/{{{FakeKeys.RouteKey}}}";

        public async Task<HttpStatusCode> HandleAsync(Request request, FakePayload payload)
        {
            FakeKeys.Verify(request);

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