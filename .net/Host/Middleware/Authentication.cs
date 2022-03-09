using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sensemaking.Web.Host
{
    internal static class Authentication
    {
        internal static IServiceCollection ProvideAuthentication(this IServiceCollection services, IAuthenticateUsers authentication, IConfiguration configuration)
        {
            authentication.RegisterServices(services, configuration);
            return services;
        }

        internal static IApplicationBuilder ResolveAuthentication(this IApplicationBuilder app, IAuthenticateUsers authentication)
        {
            authentication.Use(app);
            return app;
        }
    }
}