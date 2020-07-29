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

        private void getting()
        {
            get<GetHandler.Response>(GetHandler.Url);
        }

        private void the_handler_processes_the_request()
        {
            (the_response as JsonResponse<GetHandler.Response>).Body.should_be(GetHandler.TheResponse);
        }
    }

    public class RoutingStartup : FakeStartup
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.AddSingleton<IHandleGetRequests, GetHandler>();
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

        public static readonly string Url = "/get_from_here";
        public static Response TheResponse = new Response("anything will do");

        public HttpMethod Method => HttpMethod.Get;
        public string Route => Url;

        public async Task<object> Handle()
        {
            return TheResponse;
        }
    }
}
