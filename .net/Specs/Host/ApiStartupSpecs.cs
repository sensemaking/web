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
        public void notifies_of_service_status_every_1_minute()
        {
            Given(service_has_started);
            And(it_has_dependencies);
            Then(it_monitors_them);
            And(it_notifies_every_1_minute);
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
                When(() => requesting("text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8"));
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
    }
}
