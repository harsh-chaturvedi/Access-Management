using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Access_Management.Ancilliary.Contracts;
using Access_Management.Infrastructure.Contracts;
using Access_Management.Infrastructure.Database;
using Access_Management.Infrastructure.Extensions;
using Access_Management.Infrastructure.Model;

namespace Access_Management.Infrastructure.Services
{
    public class AuthorizationPolicyService : IAuthorizationPolicyService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ITenantOrganizationService _tenantOrganizationService;

        public AuthorizationPolicyService(ApplicationDbContext dbContext, ITenantOrganizationService tenantOrganizationService)
        {
            _dbContext = dbContext;
            _tenantOrganizationService = tenantOrganizationService;
        }

        public (bool, string) AddAuthorizationRulesToPolicy(Guid policyId, IEnumerable<ResourceAuthorizationRule> rules)
        {
            if (rules == null || !rules.Any())
            {
                return (false, "Invalid parameter!");
            }

            var existingRules = GetAuthorizationRulesByPolicyId(policyId);
            var isDuplicate = existingRules != null ? existingRules.Any(x => rules.Select(t => t.Route).Contains(x.Route)) : false;

            if (isDuplicate)
            {
                return (false, Constants.ErrorMessage.DuplicateAuthorizationPolicy);
            }

            _dbContext.AuthorizationRules.AddRange(rules);
            _dbContext.SaveChanges();

            return (true, Constants.ErrorMessage.NewRecordCreated);
        }

        public (bool, string) AddAuthorizationPolicy(ResourceAuthorizationPolicy policy)
        {
            if (policy == null)
            {
                return (false, "Invalid parameter!");
            }

            var isDuplicate = _dbContext.AuthorizationPolicies.Any(t => t.TenantId == policy.TenantId
                            && (policy.OrganizationId.HasValue ? t.OrganizationId == policy.OrganizationId : true)
                            && t.ApplicationDomain == policy.ApplicationDomain
                            && t.AccessSource == policy.AccessSource);

            if (isDuplicate)
            {
                return (false, Constants.ErrorMessage.DuplicateAuthorizationPolicy);
            }

            _dbContext.AuthorizationPolicies.Add(policy);
            _dbContext.SaveChanges();

            return (true, policy.Id.ToString());
        }

        public (bool, string) DeleteAuthorizationPolicy(ResourceAuthorizationPolicy policy)
        {
            _dbContext.AuthorizationPolicies.Remove(policy);
            _dbContext.SaveChanges();

            return (true, Constants.ErrorMessage.RecordDeleted);
        }

        public (bool, string) DeleteAuthorizationRules(IEnumerable<ResourceAuthorizationRule> rules)
        {
            _dbContext.AuthorizationRules.RemoveRange(rules);
            _dbContext.SaveChanges();

            return (true, Constants.ErrorMessage.RecordDeleted);
        }

        public IEnumerable<ResourceAuthorizationRule> GetAuthorizationRulesByPolicyId(Guid policyId)
        {
            return _dbContext.AuthorizationRules.Where(x => x.PolicyId == policyId);
        }

        public IEnumerable<ResourceAuthorizationPolicy> GetAuthorizationPoliciesByTenant(int? tenantId)
        {
            var globalRules = tenantId.HasValue ?
                _dbContext.AuthorizationPolicies.Where(x => x.TenantId == tenantId.Value)
                : _dbContext.AuthorizationPolicies;
            return globalRules;
        }

        public IEnumerable<ResourceAuthorizationPolicy> GetAuthorizationPoliciesByTenantAndOrganizationAndTargetDomain(int tenantId, Guid organizationId, string targetDomain)
        {
            IQueryable<ResourceAuthorizationPolicy> rules;
            if (organizationId != Guid.Empty && _dbContext.AuthorizationPolicies.Any(x => x.OrganizationId == organizationId && x.TenantId == tenantId))
            {
                rules = _dbContext.AuthorizationPolicies.Where(x => x.TenantId == tenantId && targetDomain.Normalize() == x.ApplicationDomain && x.OrganizationId == organizationId);
            }
            else
            {
                rules = _dbContext.AuthorizationPolicies.Where(x => x.TenantId == tenantId && targetDomain.Normalize() == x.ApplicationDomain);
            }
            return rules;
        }

        public ResourceAuthorizationPolicy GetAuthorizationPoliciesByTenantAndOrganizationAndTargetDomainAndAccessSource(int tenantId, Guid organizationId, string targetDomain, AccessSource accessSource)
        {
            var globalRules = _dbContext.AuthorizationPolicies.Where(x => x.TenantId == tenantId && x.AccessSource == accessSource && x.ApplicationDomain == targetDomain.Normalize());

            // two ways if organization id is empty (either in db or request) -
            if (organizationId != Guid.Empty && _dbContext.AuthorizationPolicies.Any(x => x.OrganizationId == organizationId && x.TenantId == tenantId))
            {
                globalRules = globalRules.Where(x => x.OrganizationId == organizationId);
            }

            return globalRules.FirstOrDefault();
        }

        public IEnumerable<ResourceAuthorizationPolicy> GetAuthorizationPoliciesByTenantAndOrganizationId(int tenantId, Guid organizationId)
        {
            var rules = _dbContext.AuthorizationPolicies.Where(x => x.TenantId == tenantId);

            if (organizationId != Guid.Empty && _dbContext.AuthorizationPolicies.Any(x => x.OrganizationId == organizationId && x.TenantId == tenantId))
            {
                rules = rules.Where(x => x.OrganizationId == organizationId);
            }

            return rules;
        }

        public ResourceAuthorizationPolicy GetAuthorizationPolicyById(Guid policyId)
        {
            return _dbContext.AuthorizationPolicies.FirstOrDefault(x => x.Id == policyId);
        }

        public ResourceAuthorizationResponse VerifyAccess(ClaimsPrincipal principal, ResourceAuthorizationRequest request)
        {
            var tenantDomain = principal.FindFirst(Constants.UniqueHomeEnvironmentIdentifier);
            var organizationId = principal.FindFirst(Constants.TenantId) != null ? Guid.Parse(principal.FindFirst(Constants.TenantId).Value) : Guid.Empty;
            var roleClaim = principal.FindFirst(ClaimTypes.Role) != null ? principal.FindFirst(ClaimTypes.Role).Value : null;

            if (tenantDomain == null || tenantDomain.Value == null)
            {
                return ResourceAuthorizationResponse.Failed(new IdentityError { Code = "Unable to identify application. Invalid access token!!" });
            }

            var tenantId = _tenantOrganizationService.IdentifyTenant(tenantDomain.Value);

            var authPolicy = GetAuthorizationPoliciesByTenantAndOrganizationAndTargetDomainAndAccessSource(tenantId, organizationId, request.ApplicationDomain, request.AccessSource);
            var authorizationRules = authPolicy != null ? GetAuthorizationRulesByPolicyId(authPolicy.Id) : null;

            //no auth policy configured - allow access
            if (authPolicy == null || authorizationRules == null || !authorizationRules.Any())
            {
                return ResourceAuthorizationResponse.Success;
            }

            foreach (var authRule in authorizationRules)
            {
                //find a route match - execute matched rule
                if (RouteMatcher.Match(authRule.Route, request.Route))
                {
                    var userRoles = !string.IsNullOrEmpty(roleClaim) ? roleClaim.Split(',', StringSplitOptions.RemoveEmptyEntries) : null;
                    var acceptableRoles = authRule.Role.Split(',', StringSplitOptions.RemoveEmptyEntries);

                    //no roles available for validation - allowing access
                    if (acceptableRoles == null || !acceptableRoles.Any())
                    {
                        return ResourceAuthorizationResponse.Success;
                    }

                    // there are roles configured for authorization - user does not have any role - deny access
                    if (userRoles == null || !userRoles.Any())
                    {
                        return ResourceAuthorizationResponse.Failed(new IdentityError { Code = "User has no roles. Unable to authorize!!" });
                    }

                    foreach (var userRole in userRoles)
                    {
                        if (acceptableRoles.Any(x => x.Normalize() == userRole.Normalize()))
                        {
                            return ResourceAuthorizationResponse.Success;
                        }
                    }

                    // routes matched - and no roles matched - thus denying authorization
                    return ResourceAuthorizationResponse.Failed(new IdentityError { Code = "User does not have correct role for authorization. Access denied!!" });
                }
            }

            // all rules were iterated - no match found - i.e. no rule configured for the route - allowing success
            return ResourceAuthorizationResponse.Success;
        }

        #region Private


        #endregion
    }
}