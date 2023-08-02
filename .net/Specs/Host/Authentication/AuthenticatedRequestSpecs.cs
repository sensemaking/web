using System;
using NUnit.Framework;
using Sensemaking.Bdd.Web;
using Sensemaking.Host.Web.Specs;

namespace Sensemaking.Web.Host.Specs.Authentication
{
    public partial class AuthenticatedRequestSpecs : Specification<SpecificationStartup>
    {
        [Test]
        public void requests_with_an_authenticated_user_are_authenticated()
        {
            scenario(() =>
            {
                Given(an_authenticated_http_context);
                Then(the_request_is_authenticated);
            });

            scenario(() =>
            {
                Given(an_unauthenticated_http_context);
                Then(the_request_is_not_authenticated);
            });
        }

        [Test]
        public void requests_with_an_authenticated_user_make_the_user_available()
        {
            scenario(() =>
            {
                Given(an_authenticated_http_context);
                When(accessing_the_requests_user);
                Then(the_user_is_available);
            });

            scenario(() =>
            {
                Given(an_unauthenticated_http_context);
                When(() => trying(accessing_the_requests_user));
                Then(informs<Exception>("You cannot retrieve the user from an unauthenticated request."));
            });
        }
    }
}
