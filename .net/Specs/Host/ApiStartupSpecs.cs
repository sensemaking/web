using NUnit.Framework;
using Sensemaking.Bdd.Web;
using Sensemaking.Http;

namespace Sensemaking.Host.Web.Specs
{
    [TestFixture]
    public partial class ApiStartupSpecs : Specification<SpecificationStartup>
    {
        [Test]
        public void refuses_pre_tls12_protocols()
        {
            Given(service_has_started);
            Then(pre_tls12_protocols_are_refused);
        }

        [Test]
        public void makes_logger_available()
        {
            Given(service_has_started);
            Then(logger_is_available);
        }

        [Test]
        public void notifies_of_service_status_every_20_seconds()
        {
            Given(service_has_started);
            And(it_has_dependencies);
            Then(it_monitors_them);
            And(it_notifies_every_20_seconds);
        }

        [Test]
        public void accepts_requests_that_accept_anything()
        {
            scenario(() =>
            {
                Given(service_has_started);
                When(() => requesting("*/*"));
                Then(it_is_ok);
            });

            scenario(() =>
            {
                Given(service_has_started);
                When(() => requesting("text/html,application/xml;q=0.9,image/apng,*/*;"));
                Then(it_is_ok);
            });
        }

        [Test]
        public void accepts_requests_that_do_not_specify_what_they_accept()
        {
            Given(service_has_started);
            When(() => requesting(""));
            Then(it_is_ok);
        }

        [Test]
        public void accepts_requests_that_accept_json()
        {
            Given(service_has_started);
            When(() => requesting(MediaType.Json));
            Then(it_is_ok);
        }

        [Test]
        public void accepts_requests_that_accept_json_sub_types()
        {
            Given(service_has_started);
            When(() => requesting(MediaType.Siren));
            Then(it_is_ok);
        }

        [Test]
        public void refuses_requests_that_are_neither_json_nor_html()
        {
            Given(service_has_started);
            When(() => requesting("application/xml"));
            Then(it_is_not_acceptable);
        }

        //POST DELETE PUT
        //
        // [Test]
        // public void accepts_valid_tokens()
        // {
        //     Given(authentication_is_required);
        //     And(a_valid_token);
        //     When(requesting_a_secure_resource);
        //     Then(it_is_ok);
        // }
        //
        // [Test]
        // public void secure_systems_require_a_token()
        // {
        //     Given(authentication_is_required);
        //     When(requesting_a_secure_resource);
        //     Then(it_is_unauthorised);
        //     And(it_issues_an_authentication_challenge);
        // }
        //
        // [Test]
        // public void rejects_invalid_tokens()
        // {
        //     Given(authentication_is_required);
        //     And(an_invalid_token);
        //     When(requesting_a_secure_resource);
        //     Then(it_is_unauthorised);
        //     And(it_does_not_issue_an_authentication_challenge);
        // }
        //
        // [Test]
        // public void rejects_tokens_without_bearer_with_challenge()
        // {
        //     Given(authentication_is_required);
        //     And(a_token_without_bearer);
        //     When(requesting_a_secure_resource);
        //     Then(it_is_unauthorised);
        //     And(it_issues_an_authentication_challenge);
        // }
        //
        // [Test]
        // public void forbids_requests_that_are_not_allowed()
        // {
        //     Given(authentication_is_required);
        //     And(a_request_that_is_not_allowed);
        //     When(requesting_a_secure_resource);
        //     Then(it_is_forbidden);
        //     And(it_does_not_issue_an_authentication_challenge);
        // }
        //
        // [Test]
        // public void uses_common_json_serialisation()
        // {
        //     Given(api_is_bootstrapped);
        //     Then(common_serialization_is_used);
        // }
        //
        // [Test]
        // public void uses_custom_json_converters()
        // {
        //     Given(api_is_bootstrapped);
        //     Then(custom_json_converters_are_used);
        // }
        //
        // [Test]
        // public void accepts_requests_that_accept_json()
        // {
        //     Given(api_is_bootstrapped);
        //     When(requesting_json);
        //     Then(it_is_ok);
        //     And(it_is_json);
        // }
        //
        // [Test]
        // public void accepts_requests_that_accept_json_sub_types()
        // {
        //     Given(api_is_bootstrapped);
        //     When(requesting_a_json_subtype);
        //     Then(it_is_ok);
        //     And(it_is_json);
        // }
        //
        // [Test]
        // public void accepts_requests_that_accept_html()
        // {
        //     Given(api_is_bootstrapped);
        //     When(requesting_html);
        //     Then(it_is_ok);
        //     And(it_is_html);
        // }
        //
        // [Test]
        // public void accepts_requests_that_include_html_but_not_json_as_being_for_html()
        // {
        //     Given(api_is_bootstrapped);
        //     When(request_including_html_but_not_json);
        //     Then(it_is_ok);
        // }
        //
        // [Test]
        // public void prefers_html()
        // {
        //     isolate(() =>
        //     {
        //         Given(api_is_bootstrapped);
        //         When(requesting_both_html_and_json);
        //         Then(it_is_ok);
        //         And(it_is_html);
        //     });
        //
        //     isolate(() =>
        //     {
        //         Given(api_is_bootstrapped);
        //         When(requesting_a_response_of_unspecified_type);
        //         Then(it_is_ok);
        //         And(it_is_html);
        //     });
        //
        //     isolate(() =>
        //     {
        //         Given(api_is_bootstrapped);
        //         When(requesting_a_response_of_any_type);
        //         Then(it_is_ok);
        //         And(it_is_html);
        //     });
        // }
        //
        // [Test]
        // public void refuses_requests_that_are_neither_json_nor_html()
        // {
        //     Given(api_is_bootstrapped);
        //     When(requesting_a_non_json_non_html_response);
        //     Then(it_is_not_acceptable);
        // }
    }
}
