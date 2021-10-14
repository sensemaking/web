using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Sensemaking.Web.Host
{
    public class Auth0 : AuthenticationBase
    {
        private readonly Settings settings;

        internal Auth0(Settings settings)
        {
            this.settings = settings;
        }

        protected override IEnumerable<KeyValuePair<string, string>> GetConfiguration()
        {
            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Auth0:Domain", settings.Domain),
                new KeyValuePair<string, string>("Auth0:Audience", settings.Audience)
            };
        }

        protected override void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.Authority = settings.Domain;
                options.Audience = settings.Audience;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = ClaimTypes.NameIdentifier
                };
            });
        }

        public readonly struct Settings
        {   
            public static Settings Empty = new Settings();

            public string Domain { get; }
            public string Audience { get; }

            public Settings(string domain, string audience)
            {
                Validation.BasedOn(errors =>
                {
                    if (domain.IsNullOrEmpty()) errors.Add("Domain must be provided.");
                    if (audience.IsNullOrEmpty()) errors.Add("Audience must be provided.");

                });

                Domain = domain;
                Audience = audience;
            }            
        }

        public bool Equals(Auth0 that)
        {
            return this.settings.Equals(that.settings);
        }

        public override bool Equals(object? that)
        {
            return that is Auth0 jwt && this.Equals(jwt);
        }

        public override int GetHashCode()
        {
            return this.settings.GetHashCode();
        }
    }
}