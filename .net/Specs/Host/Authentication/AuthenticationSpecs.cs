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
            When(() => trying(() => get<dynamic>(AuthenticatedEndpointHandler.Url)));
            Then(it_is_challenged);
        }

        [Test]
        public void unauthenticated_users_are_authorized_to_use_is_alive()
        {
            Given(an_unauthenticated_http_request);
            When(() => get<dynamic>("/is-alive"));
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
            When(() => get<dynamic>(AllowUnautheticatedGetHandler.Url));
            Then(it_is_ok);
        }

        [Test]
        public void posts_that_allow_unauthenticated_users_authorize_unauthenticated_users()
        {
            Given(an_unauthenticated_http_request);
            When(() => post(AllowUnautheticatedPostHandler.Url, new {}));
            Then(it_is_ok);
        }

        [Test]
        public void puts_that_allow_unauthenticated_users_authorize_unauthenticated_users()
        {
            Given(an_unauthenticated_http_request);
            When(() => put(AllowUnautheticatedPutHandler.Url, new {}));
            Then(it_is_ok);
        }

        [Test]
        public void delete_that_allow_unauthenticated_users_authorize_unauthenticated_users()
        {
            Given(an_unauthenticated_http_request);
            When(() => delete(AllowUnautheticatedDeleteHandler.Url));
            Then(it_is_ok);
        }
    }
}
