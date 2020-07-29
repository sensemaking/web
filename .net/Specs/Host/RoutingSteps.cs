using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sensemaking.Bdd;
using Sensemaking.Bdd.Web;
using Sensemaking.Http.Json.Client;
using Sensemaking.Web.Api;

namespace Sensemaking.Host.Web.Specs
{
    public partial class RoutingSpecs
    {
        private void a_url() {}

        private void a_get_handler_for_the_url() {}

        private void a_put_handler_for_the_url() { }

        private void a_post_handler_for_the_url() { }

        private void getting()
        {
            get<GetHandler.Response>(GetHandler.Url);
        }

        private void putting()
        {
            put(PutHandler.Url, null);
        }

        private void posting()
        {
            post(PostHandler.Url, null);
        }

        private void the_get_handler_processes_the_request()
        {
            the_response.Status.should_be(HttpStatusCode.OK);
            the_response_body<GetHandler.Response>().should_be(GetHandler.TheResponse);
        }

        private void the_put_handler_processes_the_request()
        {
            the_response.Status.should_be(PutHandler.ResponseStatusCode);
        }

        private void the_post_handler_processes_the_request()
        {
            the_response.Status.should_be(PostHandler.ResponseStatusCode);
        }
    }

    public class RoutingStartup : FakeStartup
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.AddSingleton<IHandleGetRequests, GetHandler>();
            services.AddSingleton<IHandlePutRequests, PutHandler>();
            services.AddSingleton<IHandlePostRequests, PostHandler>();
        }
    }

    public class GetHandler : IHandleGetRequests
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

    public class PutHandler : IHandlePutRequests
    {
        public static readonly string Url = "/put";
        public static readonly HttpStatusCode ResponseStatusCode = HttpStatusCode.Accepted;

        public string Route => Url;

        public async Task<HttpStatusCode> Handle()
        {
            return await Task.FromResult(ResponseStatusCode);
        }
    }

    public class PostHandler : IHandlePostRequests
    {
        public static readonly string Url = "/post";
        public static readonly HttpStatusCode ResponseStatusCode = HttpStatusCode.Created;

        public string Route => Url;

        public async Task<HttpStatusCode> Handle()
        {
            return await Task.FromResult(ResponseStatusCode);
        }
    }
}
