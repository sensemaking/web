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
        private const string the_route_value = "route_value";
        private const string the_query_value = "query_value";
        private FakePayload the_payload;

        protected override void before_each()
        {
            base.before_each();
            the_payload = null;
        }

        private void a_url() { }

        private void a_route_value() { }

        private void a_query_value() { }

        private void a_payload()
        {
            the_payload = new FakePayload("The payload contents");
        }

        private void a_get_handler_for_the_url() { }

        private void a_put_handler_for_the_url() { }

        private void a_delete_handler_for_the_url() { }

        private void a_post_handler_for_the_url() { }

        private void getting()
        {
            get<FakeGetter.Response>($"{FakeGetter.Url}/{the_route_value}?{FakeGetter.QueryKey}={the_query_value}");
        }

        private void putting()
        {
            put(FakePutter.Url, the_payload);
        }

        private void deleting()
        {
            delete(FakeDeleter.Url);
        }

        private void posting()
        {
            post(FakePoster.Url, the_payload);
        }

        private void the_get_handler_processes_the_request()
        {
            the_response.Status.should_be(HttpStatusCode.OK);
            the_response_body<FakeGetter.Response>().should_be(FakeGetter.TheResponse);
        }

        private void the_put_handler_processes_the_request()
        {
            the_response.Status.should_be(FakePutter.ResponseStatusCode);
        }

        private void the_delete_handler_processes_the_request()
        {
            the_response.Status.should_be(FakeDeleter.ResponseStatusCode);
        }

        private void the_post_handler_processes_the_request()
        {
            the_response.Status.should_be(FakePoster.ResponseStatusCode);
        }
    }
}
