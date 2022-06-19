using System.IO;
using System.Reflection;
using Flurl.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Sensemaking.Bdd;
using Sensemaking.Bdd.Web;
using Sensemaking.Web.Host;

namespace Sensemaking.Host.Web.Specs
{
    public partial class StaticContentSpecs
    {
        private string html;

        protected override void before_each()
        {
            base.before_each();
            html = null;
        }

        private void requesting_the_root()
        {
            html = client.Request("/").GetAsync().Result.ResponseMessage.Content.ReadAsStringAsync().Result;
        }

        private void it_provides_the_index()
        {
            html.should_be(File.ReadAllText($"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/../../public/index.html"));
        }
    }

    public class StaticContentStartup : SpecificationStartup
    {
        public StaticContentStartup(IConfiguration configuration) : base(configuration) { }

        protected override IApplicationBuilder AdditionalMiddleware(IApplicationBuilder app)
        {
            app.ServeStaticContent();
            return app;
        }
    }
}
