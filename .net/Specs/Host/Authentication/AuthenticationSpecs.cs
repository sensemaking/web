using NUnit.Framework;
using Sensemaking.Bdd.Web;

namespace Sensemaking.Web.Host.Specs.Authentication
{
    public partial class AuthenticationSpecs : Specification<AuthenticationStartup>
    {
        [Test]
        public void unauthenticated_users_are_challenged()
        {
            Given(an_unauthenticated_http_request);
            When(trying(getting<dynamic>(AuthenticatedEndpointHandler.Url)));
            Then(it_is_challenged);
        }

        [Test]
        public void unauthenticated_users_are_authorized_to_use_is_alive()
        {
            Given(an_unauthenticated_http_request);
            When(getting<dynamic>("/is-alive"));
            Then(it_is_ok);
        }

        [Test]
        public void unauthenticated_users_can_still_get_static_content()
        {
            Given(an_unauthenticated_http_request);
            Then(it_serves_static_content);
        }

        [Test]
        public void gets_that_allow_unauthenticated_users_authorize_unauthenticated_users()
        {
            Given(an_unauthenticated_http_request);
            When(getting<dynamic>(AllowUnautheticatedGetHandler.Url));
            Then(it_is_ok);
        }

        [Test]
        public void posts_that_allow_unauthenticated_users_authorize_unauthenticated_users()
        {
            Given(an_unauthenticated_http_request);
            When(posting(AllowUnautheticatedPostHandler.Url, new {}));
            Then(it_is_ok);
        }

        [Test]
        public void puts_that_allow_unauthenticated_users_authorize_unauthenticated_users()
        {
            Given(an_unauthenticated_http_request);
            When(putting(AllowUnautheticatedPutHandler.Url, new {}));
            Then(it_is_ok);
        }

        [Test]
        public void delete_that_allow_unauthenticated_users_authorize_unauthenticated_users()
        {
            Given(an_unauthenticated_http_request);
            When(deleting(AllowUnautheticatedDeleteHandler.Url));
            Then(it_is_ok);
        }
    }
}
