using NUnit.Framework;
using Sensemaking.Bdd.Web;

namespace Sensemaking.Host.Web.Specs
{
    public partial class StaticContentSpecs : Specification<StaticContentStartup>
    {
        [Test]
        public void serves_index_on_root()
        {
            When(requsting_the_root);
            Then(it_provides_the_index);
        }
    }
}
