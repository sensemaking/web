using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sensemaking.Web.Host
{
    public class None : IAuthenticateUsers
    {
        internal None() { }
        
        void IAuthenticateUsers.RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthorization(options => { options.AddPolicy(AuthorizationPolicies.NoAuthorization.Name, AuthorizationPolicies.NoAuthorization.Policy); });
        }

        void IAuthenticateUsers.Use(IApplicationBuilder app)
        {
            app.UseAuthorization();
        }

        public bool Equals(None that)
        {
            return true;
        }

        public override bool Equals(object? that)
        {
            return that is None none && this.Equals(none);
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
}