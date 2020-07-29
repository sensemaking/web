using NUnit.Framework;
using Sensemaking.Bdd.Web;

namespace Sensemaking.Host.Web.Specs
{
    public partial class RoutingSpecs : Specification<RoutingStartup>
    {
        [Test]
        public void get_requests_are_handled()
        {
            Given(a_url);
            And(a_get_handler_for_the_url);
            When(getting);
            Then(the_handler_processes_the_request);
        }
    }
}
