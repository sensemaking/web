using NUnit.Framework;
using Sensemaking.Bdd.Web;

namespace Sensemaking.Host.Web.Specs
{
    public partial class RequestHandlingSpecs : Specification<RequestHandlingStartup>
    {
        [Test]
        public void get_requests_are_handled()
        {
            Given(a_url);
            And(a_route_value);
            And(a_query_value);
            And(a_header_value);
            And(a_pipeline_injected_value);
            And(a_get_handler_for_the_url);
            When(getting);
            Then(the_get_handler_processes_the_request);
        }

        [Test]
        public void put_requests_are_handled()
        {
            Given(a_url);
            And(a_route_value);
            And(a_query_value);
            And(a_header_value);
            And(a_pipeline_injected_value);
            And(a_payload);
            And(a_put_handler_for_the_url);
            When(putting);
            Then(the_put_handler_processes_the_request);
        }

        [Test]
        public void put_payloads_are_validated()
        {
            Given(a_url);
            And(an_invalid_payload);
            When(() => trying(putting));
            Then(() => it_is_a_bad_request("The request could not be correctly validated.", FakePayload.ValidationError));
        }

        [Test]
        public void delete_requests_are_handled()
        {
            Given(a_url);
            And(a_route_value);
            And(a_query_value);
            And(a_header_value);
            And(a_pipeline_injected_value);
            And(a_delete_handler_for_the_url);
            When(deleting);
            Then(the_delete_handler_processes_the_request);
        }

        [Test]
        public void post_requests_are_handled()
        {
            Given(a_url);
            And(a_route_value);
            And(a_query_value);
            And(a_header_value);
            And(a_pipeline_injected_value);
            And(a_payload);
            And(a_post_handler_for_the_url);
            When(posting);
            Then(the_post_handler_processes_the_request);
        }

        [Test]
        public void post_payloads_are_validated()
        {
            Given(a_url);
            And(an_invalid_payload);
            When(() => trying(posting));
            Then(() => it_is_a_bad_request("The request could not be correctly validated.", FakePayload.ValidationError));
        }

        [Test]
        public void custom_request_factories_replace_the_default_request_factory()
        {
            Given(a_custom_request_factory);
            When(making_a_request);
            Then(the_custom_factory_is_used);
        }
    }
}
