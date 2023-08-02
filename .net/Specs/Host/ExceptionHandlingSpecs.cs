using System;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Sensemaking.Bdd.Web;
using Sensemaking.Host.Monitoring;
using Sensemaking.Monitoring;

namespace Sensemaking.Host.Web.Specs
{
    public partial class ExceptionHandlingSpecs : Specification<ExceptionStartup>
    {
        [Test]
        public void custom_exception_handlers_replace_the_default_exception_handler()
        {
            Given(a_custom_exception_handler);
            When(handling_a_request);
            Then(the_custom_handler_is_used);
        }

        [Test]
        public void who_are_you_exception_causes_not_authorised_problem()
        {
            Given(a_(who_are_you_exception));
            When(handling_a_request);
            Then(it_is_unauthorised);
        }

        [Test]
        public void not_allowed_exception_causes_forbidden_error_problem()
        {
            Given(a_(not_allowed_exception));
            When(handling_a_request);
            Then(it_is_forbidden);
        }

        [Test]
        public void not_found_exception_causes_not_found_problem()
        {
            Given(a_(not_found_exception));
            When(handling_a_request);
            Then(it_is_not_found);
        }

        [Test]
        public void too_late_exception_causes_gone_problem()
        {
            Given(a_(too_late_exception));
            When(handling_a_request);
            Then(it_is_gone);
        }

        [Test]
        public void service_unavailable_exception_causes_service_unavailable_problem()
        {
            Given(a_(service_availability_exception));
            When(handling_a_request);
            Then(it_is_service_unavailable);
        }

        [Test]
        public void unexpected_exception_causes_internal_server_error_problem()
        {
            Given(an_(unexpected_exception));
            When(handling_a_request);
            Then(it_is_an_internal_error);
            And(it_logs(new Alert<Exception>("UnexpectedException", unexpected_exception)));
        }

        [Test]
        public void validation_exception_causes_bad_request_detailing_the_problem()
        {
            Given(a_(validation_exception));
            When(handling_a_request);
            Then(it_is_a_bad_request("The request could not be correctly validated.", validation_exception.Errors));
        }

        [Test]
        public void serilaization_exception_causes_bad_request_detailing_the_problem()
        {
            Given(a_(serialization_exception));
            When(handling_a_request);
            Then(it_is_a_bad_request("The request could not be correctly serialized.", serialization_exception.Errors));
        }

        [Test]
        public void conflict_exception_causes_conflict_detailing_the_problem()
        {
            Given(a_(conflict_exception));
            When(handling_a_request);
            Then(it_is_a_conflict(conflict_exception.Errors));
        }

        [Test]
        public void legal_exception_causes_unavailable_for_legal_reasons_detailing_the_problem()
        {
            Given(a_(legal_exception));
            When(handling_a_request);
            Then(it_is_unavailable_for_legal_reasons(legal_exception.Errors));
        }

        [Test]
        public void problems_without_details_have_no_content_type()
        {
            Given(a_(not_allowed_exception));
            When(handling_a_request);
            Then(it_has_no_content_type);
        }
    }
}