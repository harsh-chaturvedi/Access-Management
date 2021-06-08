using System;
using System.Collections.Generic;
using System.Linq;
using Access_Management.Infrastructure.Model;

namespace Access_Management.Infrastructure.Extensions
{
    public static class AuthorizationPolicyValidator
    {
        /// <summary>
        /// Validates the specified application role.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <returns></returns>
        public static (bool, string) Validate(this ResourceAuthorizationPolicy policy)
        {
            if (!(policy.TenantId > 0))
            {
                return (false, "Tenant Id cannot be empty!");
            }
            if (string.IsNullOrEmpty(policy.ApplicationDomain))
            {
                return (false, "Application Domain cannot be empty!");
            }

            return (true, string.Empty);
        }

        public static (bool, string) Validate(this IEnumerable<ResourceAuthorizationRule> rules, Guid policyId)
        {
            if (rules == null || !rules.Any())
            {
                return (false, "Authorization rules cannot be empty!");
            }
            foreach (var item in rules)
            {
                if (Guid.Empty == item.PolicyId || item.PolicyId != policyId)
                {
                    return (false, "Invalid or empty Policy Id!");
                }
                if (string.IsNullOrEmpty(item.Role))
                {
                    return (false, "Authorization role cannot be empty!");
                }
                if (string.IsNullOrEmpty(item.Route))
                {
                    return (false, "Route cannot be empty!");
                }
            }

            return (true, string.Empty);
        }
    }
}