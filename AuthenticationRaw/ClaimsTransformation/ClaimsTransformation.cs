using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace Basics.ClaimsTransformation
{
    public class ClaimsTransformation : IClaimsTransformation
    {
        //gets called everytime a user is authenticated
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var hasFriendClaim = principal.Claims.Any(x => x.Type == "Friend");
            if (!hasFriendClaim)
            {
                ((ClaimsIdentity)principal.Identity).AddClaim(new Claim("Friend", "Good"));
            }
            return Task.FromResult(principal);
        }
    }
}

