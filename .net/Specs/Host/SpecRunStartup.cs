using NUnit.Framework;

namespace Sensemaking.Host.Web.Specs
{
    [SetUpFixture]
    public class SpecRunStartup
    {
        [OneTimeSetUp]
        public void SetupWeb()
        {
            Web.Configure();
        }
    }
}