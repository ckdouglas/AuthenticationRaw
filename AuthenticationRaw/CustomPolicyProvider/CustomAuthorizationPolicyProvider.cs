using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Basics.CustomPolicyProvider
{
    public class CustomAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {

        public static class DynamicPolicies
        {
            public static IEnumerable<string> Get()
            {
                yield return SecurityLevel;
                yield return Rank;
            }
            public const string SecurityLevel = "SecurityLevel";
            public const string Rank = "Rank";
        }

        public CustomAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
        {
        }

        public override Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            return base.GetPolicyAsync(policyName);
        }
    }
}

