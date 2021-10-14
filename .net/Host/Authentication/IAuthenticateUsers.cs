using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sensemaking.Web.Host
{
    public interface IAuthenticateUsers
    {        
        internal void RegisterServices(IServiceCollection services, IConfiguration configuration);
        internal void Use(IApplicationBuilder app);
    }

    public static class UseAuthentication
    {
        public static IAuthenticateUsers None() => new None();        

        public static IAuthenticateUsers Auth0(Auth0.Settings settings)
        {
            Validation.BasedOn(errors =>
            {
                if (settings.Equals(Host.Auth0.Settings.Empty))
                    errors.Add("JWT settings must be provided.");
            });

            return new Auth0(settings);
        }
    }

    public abstract class AuthenticationBase : IAuthenticateUsers
    {
        void IAuthenticateUsers.RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            RegisterServices(services, configuration);
            services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthorizationPolicies.NoAuthorization.Name, AuthorizationPolicies.NoAuthorization.Policy);
                options.FallbackPolicy = AuthorizationPolicies.RequireAuthenticatedUser.Policy;
            });
        }

        void IAuthenticateUsers.Use(IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }
                
        protected abstract void RegisterServices(IServiceCollection services, IConfiguration configuration);
    }

    internal static class AuthorizationPolicies
    {
        internal static readonly (string Name, AuthorizationPolicy Policy) NoAuthorization = ("NoAuthorization", new AuthorizationPolicyBuilder().RequireAssertion(context => true).Build());
        internal static readonly (string Name, AuthorizationPolicy Policy) RequireAuthenticatedUser = ("RequireAuthenticatedUser", new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());
    }
}