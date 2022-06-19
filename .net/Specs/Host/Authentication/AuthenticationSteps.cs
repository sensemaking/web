using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Flurl.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Sensemaking.Bdd;
using Sensemaking.Host.Web.Specs;
using Sensemaking.Web.Api;

namespace Sensemaking.Web.Host.Specs.Authentication
{
    public partial class AuthenticationSpecs
    {
        private void an_unauthenticated_http_request() { }

        private void it_serves_static_content()
        {
            client.Request("/").GetAsync().Result.ResponseMessage.StatusCode.should_be(HttpStatusCode.OK);
        }

        private void it_is_challenged()
        {
            it_is_unauthorised();
            the_problem_exception.Headers.Single().should_be(("WWW-Authenticate", "Bearer"));
        }
    }

    public class AuthenticationStartup : SpecificationStartup
    {
        private readonly Auth0.Settings settings = new Auth0.Settings("https://auth0.com","a_key_of_some_sort");
        protected override IAuthenticateUsers Authentication => UseAuthentication.Auth0(settings);
        public AuthenticationStartup(IConfiguration configuration) : base(configuration) { }

        protected override IApplicationBuilder AdditionalMiddleware(IApplicationBuilder app)
        {
            app.ServeStaticContent();
            return app;
        }
    }

    public class AuthenticatedEndpointHandler : IHandleGetRequests
    {
        public const string Url = "/authenticated";
        public string Route => Url;
        public async Task<object> HandleAsync(Request request) { return await Task.FromResult("Wibble"); }
    }

    public class AllowUnautheticatedGetHandler : IHandleGetRequests
    {
        public bool AllowUnauthenicatedUsers => true;
        
        public const string Url = "/get_allowing_unauthenticated_users";
        public string Route => Url;
        public async Task<object> HandleAsync(Request request) { return await Task.FromResult("Wibble"); }
    }

    public class AllowUnautheticatedPostHandler : IHandlePostRequests<object>
    {
        public bool AllowUnauthenicatedUsers => true;

        public const string Url = "/post_allowing_unauthenticated_users";
        public string Route => Url;
        public async Task<HttpStatusCode> HandleAsync(Request request, object payload) { return await Task.FromResult(HttpStatusCode.OK); }
    }

    public class AllowUnautheticatedPutHandler : IHandlePutRequests<object>
    {
        public bool AllowUnauthenicatedUsers => true;

        public const string Url = "/put_allowing_unauthenticated_users";
        public string Route => Url;
        public async Task<HttpStatusCode> HandleAsync(Request request, object payload) { return await Task.FromResult(HttpStatusCode.OK); }
    }

    public class AllowUnautheticatedDeleteHandler : IHandleDeleteRequests
    {
        public bool AllowUnauthenicatedUsers => true;

        public const string Url = "/delete_allowing_unauthenticated_users";
        public string Route => Url;
        public async Task<HttpStatusCode> HandleAsync(Request request) { return await Task.FromResult(HttpStatusCode.OK); }
    }
}

