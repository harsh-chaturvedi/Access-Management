using System;
using System.Collections.Generic;
using System.Security.Claims;
using Access_Management.Infrastructure.Model;

namespace Access_Management.Infrastructure.Contracts
{
    public interface IAuthorizationPolicyService
    {
        ResourceAuthorizationPolicy GetAuthorizationPolicyById(Guid policyId);

        IEnumerable<ResourceAuthorizationPolicy> GetAuthorizationPoliciesByTenant(int? tenantId);

        IEnumerable<ResourceAuthorizationPolicy> GetAuthorizationPoliciesByTenantAndOrganizationId(int tenantId, Guid organizationId);

        IEnumerable<ResourceAuthorizationPolicy> GetAuthorizationPoliciesByTenantAndOrganizationAndTargetDomain(int tenantId, Guid organizationId, string targetDomain);

        IEnumerable<ResourceAuthorizationRule> GetAuthorizationRulesByPolicyId(Guid policyId);

        (bool, string) AddAuthorizationPolicy(ResourceAuthorizationPolicy policy);

        (bool, string) DeleteAuthorizationPolicy(ResourceAuthorizationPolicy policy);

        (bool, string) AddAuthorizationRulesToPolicy(Guid policyId, IEnumerable<ResourceAuthorizationRule> rules);

        (bool, string) DeleteAuthorizationRules(IEnumerable<ResourceAuthorizationRule> rules);

        ResourceAuthorizationResponse VerifyAccess(ClaimsPrincipal claimsPrincipal, ResourceAuthorizationRequest request);
    }
}