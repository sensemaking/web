﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Sensemaking.Web.Host
{
    public abstract class ApiWithUiStartup : ApiStartup
    {
        protected ApiWithUiStartup(IConfiguration configuration) : base(configuration) { }

        protected override IApplicationBuilder AdditionalMiddleware(IApplicationBuilder app)
        {
            app.ServeStaticContent();
            return app;
        }
    }
}