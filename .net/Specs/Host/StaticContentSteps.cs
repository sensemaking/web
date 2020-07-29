using System.IO;
using System.Reflection;
using Flurl.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Sensemaking.Bdd;
using Sensemaking.Bdd.Web;

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

        private void requsting_the_root()
        {
            html = client.Request("/").GetAsync().Result.Content.ReadAsStringAsync().Result;
        }

        private void it_provides_the_index()
        {
            html.should_be(File.ReadAllText($"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/../../wwwroot/index.html"));
        }
    }

    public class StaticContentStartup : FakeStartup
    {
        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            base.Configure(app, env);
            app.ServeStaticContent();
        }
    }
}
