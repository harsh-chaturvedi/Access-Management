using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Access_Management.Ancilliary.Contracts;
using Access_Management.Extensions;
using Access_Management.Infrastructure.Contracts;
using Access_Management.Infrastructure.Extensions;
using Access_Management.Infrastructure.Model;
using Microsoft.AspNetCore.Mvc;

namespace Access_Management.Controllers
{
    public class AuthorizationPolicyController : ApiController
    {
        private readonly IAuthorizationPolicyService _policyService;
        public AuthorizationPolicyController(ITenantOrganizationService tenantOrganizationService, IAuthorizationPolicyService authorizationPolicyService) : base(tenantOrganizationService)
        {
            _policyService = authorizationPolicyService;
        }

        /// <summary>
        /// Checks wether the user with token is authorized to access the end point
        /// </summary>
        /// <param name="authorizationRequest">Authorization request parameters</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Verify")]
        public IActionResult VerifyAccess([FromBody] ResourceAuthorizationRequest authorizationRequest)
        {
            return Ok(_policyService.VerifyAccess(User, authorizationRequest));
        }

        /// <summary>
        /// Gets all the authorization policy for the tenant
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("tenants/")]
        public IActionResult Get()
        {
            if (User.IsSuperAdmin())
            {
                return Ok(_policyService.GetAuthorizationPoliciesByTenant(null));
            }
            if (IsUserExternalApplicationTenantAdmin)
            {
                return Ok(_policyService.GetAuthorizationPoliciesByTenant(LoggedInTenantId));
            }
            return BadRequest(Infrastructure.Model.Constants.ErrorMessage.AccessDenied);
        }

        /// <summary>
        /// Gets the authorization policy specified by the identifier
        /// </summary>
        /// <param name="Id">The policy identifier</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id}")]
        public IActionResult Get([FromRoute] Guid Id)
        {
            var policy = _policyService.GetAuthorizationPolicyById(Id);
            if (policy == null || !CheckEntityAccessToLoggedInTenant(policy.TenantId))
            {
                return BadRequest(Infrastructure.Model.Constants.ErrorMessage.AccessDenied);
            }
            return Ok(policy);
        }

        /// <summary>
        /// Gets the authorization rules specified by the policy identifier
        /// </summary>
        /// <param name="Id">The policy identifier</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id}/Rules")]
        public IActionResult GetRules([FromRoute] Guid Id)
        {
            var policy = _policyService.GetAuthorizationPolicyById(Id);
            if (policy == null || !CheckEntityAccessToLoggedInTenant(policy.TenantId))
            {
                return BadRequest(Infrastructure.Model.Constants.ErrorMessage.AccessDenied);
            }
            return Ok(_policyService.GetAuthorizationRulesByPolicyId(Id));
        }

        /// <summary>
        /// Adds the authorization policy
        /// </summary>
        /// <param name="policy"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post([FromBody] ResourceAuthorizationPolicy policy)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessage());
            }

            var (isSuccess, errorMessage) = policy.Validate();
            if (!isSuccess)
            {
                return BadRequest(errorMessage);
            }

            if (!CheckEntityAccessToLoggedInTenant(policy.TenantId))
            {
                return BadRequest(Infrastructure.Model.Constants.ErrorMessage.AuthroizationPolicyNotFound);
            }

            var (isInserted, message) = _policyService.AddAuthorizationPolicy(policy);
            if (isInserted)
            {
                return Ok(message);
            }
            return BadRequest(message);
        }

        /// <summary>
        /// Deletes the authorization policy specified by the identifier
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{Id}")]
        public IActionResult Delete([FromRoute] Guid Id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (Id == Guid.Empty)
            {
                return BadRequest(Infrastructure.Model.Constants.ErrorMessage.AuthroizationPolicyNotFound);
            }

            var authPolicy = _policyService.GetAuthorizationPolicyById(Id);
            if (authPolicy == null || !CheckEntityAccessToLoggedInTenant(authPolicy.TenantId))
            {
                return BadRequest(Infrastructure.Model.Constants.ErrorMessage.AuthroizationPolicyNotFound);
            }

            var (isDeleted, message) = _policyService.DeleteAuthorizationPolicy(authPolicy);
            if (isDeleted)
            {
                return Ok(message);
            }
            return BadRequest(message);
        }

        /// <summary>
        /// Adds authorization rules to the policy specified by the identifier
        /// </summary>
        /// <param name="Id">The policy identifier</param>
        /// <param name="rules">The authorization rule data</param>
        /// <returns></returns>
        [HttpPost]
        [Route("{Id}/Rules")]
        public IActionResult PostRules([FromRoute] Guid Id, [FromBody][Required] IEnumerable<ResourceAuthorizationRule> rules)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessage());
            }

            var authPolicy = _policyService.GetAuthorizationPolicyById(Id);
            if (authPolicy != null && !CheckEntityAccessToLoggedInTenant(authPolicy.TenantId))
            {
                return BadRequest(Infrastructure.Model.Constants.ErrorMessage.AuthroizationPolicyNotFound);
            }

            var (isSuccess, errorMessage) = rules.Validate(authPolicy.Id);
            if (!isSuccess)
            {
                return BadRequest(errorMessage);
            }

            var (isInserted, message) = _policyService.AddAuthorizationRulesToPolicy(Id, rules);
            if (isInserted)
            {
                return Ok(message);
            }
            return BadRequest(message);
        }

        /// <summary>
        /// Deletes the rules specified by the policy identifier
        /// </summary>
        /// <param name="policyId">The policy identifier</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{policyId}/Rules")]
        public IActionResult DeleteRules([FromRoute] Guid policyId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (policyId == Guid.Empty)
            {
                return BadRequest(Infrastructure.Model.Constants.ErrorMessage.AuthroizationPolicyNotFound);
            }

            var authPolicy = _policyService.GetAuthorizationPolicyById(policyId);
            if (authPolicy == null || !CheckEntityAccessToLoggedInTenant(authPolicy.TenantId))
            {
                return BadRequest(Infrastructure.Model.Constants.ErrorMessage.AuthroizationPolicyNotFound);
            }

            var authorizationRules = _policyService.GetAuthorizationRulesByPolicyId(policyId);
            var (isDeleted, message) = authorizationRules != null ?
                                        _policyService.DeleteAuthorizationRules(authorizationRules)
                                        : (true, Infrastructure.Model.Constants.ErrorMessage.RecordDeleted);
            if (isDeleted)
            {
                return Ok(message);
            }
            return BadRequest(message);
        }
    }
}