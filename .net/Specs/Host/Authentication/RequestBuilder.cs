using Sensemaking.Web.Api;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Claims;

namespace Sensemaking.Web.Host.Specs.Authentication
{
    public class RequestBuilder
    {
        protected readonly Dictionary<string, object> requestValues = new Dictionary<string, object>();

        public Request Build()
        {
            return new Request(new ReadOnlyDictionary<string, object>(requestValues));
        }

        public RequestBuilder AuthenticatedAs(Guid user)
        {
            if (requestValues.ContainsKey(Requests.UserKey))
                requestValues.Remove(Requests.UserKey);

            requestValues.Add(Requests.UserKey, new ClaimsPrincipal(new FakeIdentity(user)));
            return this;
        }

        public class FakeIdentity : ClaimsIdentity
        {
            public FakeIdentity(Guid user) : base(new[] { new Claim(ActiveDirectoryRequests.IdClaimType, user.ToString()) })
            {
                if (user == Guid.Empty)
                    throw new Exception("A user must be provided in order to have an authenticated request.");

                this.IsAuthenticated = true;
            }

            public override bool IsAuthenticated { get; }
        }
    }
}
