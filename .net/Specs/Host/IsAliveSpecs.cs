using NUnit.Framework;
using Sensemaking.Bdd.Web;

namespace Sensemaking.Host.Web.Specs
{
    public partial class IsAliveSpecs : Specification<IsAliveStartup>
    {
        [Test]
        public void if_all_is_well_service_is_up()
        {
            Given(monitor_has_full_service);
            When(checking_service_availability);
            Then(it_is_ok);
            And(status_is_up);
        }
    }
}