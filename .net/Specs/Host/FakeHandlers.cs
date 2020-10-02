using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Sensemaking.Web.Api;
using Sensemaking.Web.Host;

namespace Sensemaking.Host.Web.Specs
{
    public static class FakeKeys
    {
        public static readonly string RouteKey = "routeKey";
        public static readonly string QueryKey = "queryKey";
        public static readonly string PipelineKey = "pipelineKey";

        public static void Verify(Request request)
        {
            if (!request.Values.ContainsKey(RouteKey) || !request.Values.ContainsKey(QueryKey) || !request.Values.ContainsKey(PipelineKey))
                throw new ValidationException("Route values, query string value or pipeline values were not provided.");
        }
    }

    public class FakeFeature
    {
        public FakeFeature(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }

    public class FakeRequestFactory : RequestFactory
    {
        protected override IDictionary<string, object> GetAdditionalValuesFrom(IFeatureCollection features)
        {
            return new Dictionary<string, object> { { FakeKeys.PipelineKey, features.Get<FakeFeature>().Value } };
        }
    }

    public class FakeGetter : IHandleGetRequests
    {
        public readonly struct Response
        {
            public Response(string queryValue, string routeValue, string pipelineValue)
            {
                QueryValue = queryValue;
                RouteValue = routeValue;
                PipelineValue = pipelineValue;
            }

            public string QueryValue { get; }
            public string RouteValue { get; }
            public string PipelineValue { get; }
        }

        public static readonly string Url = "/get";

        public string Route => $"{Url}/{{{FakeKeys.RouteKey}}}";

        public async Task<object> HandleAsync(Request request)
        {
            return await Task.FromResult(new Response(request[FakeKeys.QueryKey].ToString(), request[FakeKeys.RouteKey].ToString(), request[FakeKeys.PipelineKey].ToString()));
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

            if (payload.Content.IsNullOrEmpty())
                throw new ValidationException("Payload was not provided.");

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

            if (payload.Content.IsNullOrEmpty())
                throw new ValidationException("Payload was not provided.");

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