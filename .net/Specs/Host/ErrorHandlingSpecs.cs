using NUnit.Framework;
using Sensemaking.Bdd.Web;
using Sensemaking.Monitoring;

namespace Sensemaking.Host.Web.Specs
{
    public partial class ErrorHandlingSpecs : Specification<ErrorStartup>
    {
        [Test]
        public void validation_exception_causes_bad_request_detailing_the_problem()
        {
            // Given(a_(service_availability_exception));
            // When(handling_a_request);
            // Then(() => it_is_a_bad_request(ErrorHandlingSpecs.validation_exception.Errors));
        }
    }
}