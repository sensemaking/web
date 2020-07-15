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
        
        [Test]
        public void if_service_levels_are_reduced_service_is_up()
        {
            Given(monitor_has_reduced_service);
            When(checking_service_availability);
            Then(it_is_ok);
            And(status_is_up);
        }

        [Test]
        public void if_all_is_not_well_service_is_down()
        {
            Given(monitor_has_no_service);
            When(checking_service_availability);
            Then(() => it_is_service_unavailable("Service is currently unavailable."));
        }
    }
}