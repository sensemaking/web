using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Sensemaking.Web.Host
{
    public abstract class ApiWithUiStartup : ApiStartup
    {
        protected override IApplicationBuilder AdditionalMiddleware(IApplicationBuilder app)
        {
            app.ServeStaticContent();
            return app;
        }
    }
}