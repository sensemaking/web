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
        private void a_url() { }

        private void a_payload()
        {
            throw new System.NotImplementedException();
        }

        private void a_get_handler_for_the_url() { }

        private void a_put_handler_for_the_url() { }

        private void a_delete_handler_for_the_url() { }

        private void a_post_handler_for_the_url() { }

        private void getting()
        {
            get<FakeGetter.Response>(FakeGetter.Url);
        }

        private void putting()
        {
            put(FakePutter.Url, null);
        }

        private void deleting()
        {
            delete(FakeDeleter.Url);
        }

        private void posting()
        {
            post(FakePoster.Url, null);
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
