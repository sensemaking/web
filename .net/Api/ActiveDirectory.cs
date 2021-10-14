using System;
using System.Linq;
using System.Security.Claims;

namespace Sensemaking.Web.Api
{
    public static class ActiveDirectoryRequests
    {
        public const string IdClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        
        public static Guid Id(this ClaimsPrincipal user)
        {
            return Guid.Parse(user.Identities.First().Claims.Single(c => c.Type == IdClaimType).Value);
        }
    }
}

