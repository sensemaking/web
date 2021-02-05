using NUnit.Framework;
using Sensemaking.Bdd;
using Sensemaking.Bdd.Web;

namespace Sensemaking.Host.Web.Specs
{
    public partial class ConfigurationSpecs : Specification
    {
        [Test]
        public void provides_json_app_settings()
        {
            Then(app_settings_are_provided);
        }

        [Test]
        public void allows_for_further_configuration()
        {
            Then(any_further_configuration_is_provided);
        }
    }
}
