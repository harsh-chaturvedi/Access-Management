using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Access_Management.Client.Model;
using Microsoft.AspNetCore.Http;

namespace Access_Management.Client.Client
{
    public class AuthorizationPolicyClient : ApiClient
    {
        private readonly Uri _authorizationServerUri;

        public AuthorizationPolicyClient(HttpContext httpContext, string authorizationServerUri, string environmentUri) : base(httpContext, environmentUri)
        {
            _authorizationServerUri = new Uri(authorizationServerUri);
        }

        /// <summary>
        /// Checks wether the user with token is authorized to access the end point
        /// </summary>
        /// <param name="authorizationRequest">Authorization request parameters</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> VerifyAccess(ResourceAuthorizationRequest authorizationRequest)
        {
            var uri = new Uri(_authorizationServerUri, "api/AuthorizationPolicy/Verify");
            return await SendRequestAsync(uri, HttpMethod.Post, authorizationRequest);
        }

        /// <summary>
        /// Gets all the authorization policy for the tenant
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Get()
        {
            var uri = new Uri(_authorizationServerUri, "api/AuthorizationPolicy/tenants");
            return await SendRequestAsync(uri, HttpMethod.Get, new object());
        }

        /// <summary>
        /// Gets the authorization policy specified by the identifier
        /// </summary>
        /// <param name="Id">The policy identifier</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Get(Guid Id)
        {
            var uri = new Uri(_authorizationServerUri, string.Format("api/AuthorizationPolicy/{0}", Id.ToString()));
            return await SendRequestAsync(uri, HttpMethod.Get, new object());
        }

        /// <summary>
        /// Gets the authorization rules specified by the policy identifier
        /// </summary>
        /// <param name="Id">The policy identifier</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetRules(Guid Id)
        {
            var uri = new Uri(_authorizationServerUri, string.Format("api/AuthorizationPolicy/{0}/Rules", Id.ToString()));
            return await SendRequestAsync(uri, HttpMethod.Get, new object());
        }

        /// <summary>
        /// Adds the authorization policy
        /// </summary>
        /// <param name="policy"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Post(ResourceAuthorizationPolicy policy)
        {
            var uri = new Uri(_authorizationServerUri, string.Format("api/AuthorizationPolicy"));
            return await SendRequestAsync(uri, HttpMethod.Post, policy);
        }

        /// <summary>
        /// Deletes the authorization policy specified by the identifier
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Delete(Guid Id)
        {
            var uri = new Uri(_authorizationServerUri, string.Format("api/AuthorizationPolicy/{0}", Id.ToString()));
            return await SendRequestAsync(uri, HttpMethod.Delete, new object());
        }

        /// <summary>
        /// Adds authorization rules to the policy specified by the identifier
        /// </summary>
        /// <param name="Id">The policy identifier</param>
        /// <param name="rules">The authorization rule data</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PostRules(Guid Id, IEnumerable<ResourceAuthorizationRule> rules)
        {
            var uri = new Uri(_authorizationServerUri, string.Format("api/AuthorizationPolicy/{0}/Rules", Id.ToString()));
            return await SendRequestAsync(uri, HttpMethod.Post, rules);
        }

        /// <summary>
        /// Deletes the rules specified by the policy identifier
        /// </summary>
        /// <param name="policyId">The policy identifier</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> DeleteRules(Guid policyId)
        {
            var uri = new Uri(_authorizationServerUri, string.Format("api/AuthorizationPolicy/{0}/Rules", policyId.ToString()));
            return await SendRequestAsync(uri, HttpMethod.Delete, new object());
        }
    }
}