using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NSubstitute;
using Sensemaking.Bdd;
using Sensemaking.Web.Api;

namespace Sensemaking.Web.Host.Specs.Authentication
{
    public partial class AuthenticatedRequestSpecs
    {
        private static readonly Guid user_id = Guid.NewGuid();
        private ClaimsPrincipal request_user;
        private Request request;
        private ClaimsPrincipal user;

        protected override void before_each()
        {
            base.before_each();
            request_user = default;
            request = default;
            user = default;
        }

        private void an_authenticated_http_context()
        {
            request = new RequestFactory().Create(get_fake_http_context(authenticated: true));
        }

        private void an_unauthenticated_http_context()
        {
            request = new RequestFactory().Create(get_fake_http_context(authenticated: false));
        }

        private void accessing_the_requests_user()
        {
            user = request.User();
        }

        private void the_request_is_authenticated()
        {
            request.IsAuthenticated().should_be_true();
        }

        private void the_request_is_not_authenticated()
        {
            request.IsAuthenticated().should_be_false();
        }

        private void the_user_is_available()
        {
            user.should_be(request_user);
            user.Id().should_be(user_id);
        }

        private HttpContext get_fake_http_context(bool authenticated)
        {
            var httpRequest = Substitute.For<HttpRequest>();
            httpRequest.Query.Returns(new QueryCollection());
            httpRequest.RouteValues.Returns(new RouteValueDictionary());

            var context = Substitute.For<HttpContext>();
            context.Request.Returns(httpRequest);

            request_user = authenticated ? new ClaimsPrincipal(new RequestBuilder.FakeIdentity(user_id)) : new ClaimsPrincipal(new ClaimsIdentity()); 
            context.User.Returns(request_user);
            return context;
        }
    }
}

