using System;
using System.Net;
using System.Threading.Tasks;
using Castle.Core.Internal;
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

        public static readonly string Url = "/get";
        public static Response TheResponse = new Response("anything will do");

        public string Route => Url;

        public async Task<object> Handle()
        {
            return await Task.FromResult(TheResponse);
        }
    }

    public class FakePutter : IHandlePutRequests<FakePayload>
    {
        public static readonly string Url = "/put";
        public static readonly HttpStatusCode ResponseStatusCode = HttpStatusCode.Accepted;

        public string Route => Url;

        public async Task<HttpStatusCode> Handle(FakePayload payload)
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

        public async Task<HttpStatusCode> Handle(FakePayload payload)
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