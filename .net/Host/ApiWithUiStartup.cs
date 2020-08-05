using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Sensemaking.Web.Host
{
    public abstract class ApiWithUiStartup : ApiStartup
    {
        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            base.Configure(app, env);
            app.ServeStaticContent();
        }
    }
}