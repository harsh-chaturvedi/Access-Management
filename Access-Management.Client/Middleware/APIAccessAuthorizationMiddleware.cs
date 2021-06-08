using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Access_Management.Client.Client;
using Access_Management.Client.Extensions;
using Access_Management.Client.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Access_Management.Client.Middleware
{
    public class APIAccessAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private AuthorizationPolicyClient _policyClient;
        private readonly APIAccessAuthorizationOptions _options;

        public APIAccessAuthorizationMiddleware(RequestDelegate next, APIAccessAuthorizationOptions options)
        {
            _next = next;
            _options = options;
        }

        public async Task InvokeAsync(HttpContext httpContext, ILogger<APIAccessAuthorizationMiddleware> logger)
        {
            //bypass middleware pipeline if the route does not match
            if (!httpContext.Request.Path.StartsWithSegments(_options.RouteSegmentToMatch))
            {
                await _next(httpContext);
                return;
            }
            if (_options.AllowedNonAuthorizedRouteTemplates != null && _options.AllowedNonAuthorizedRouteTemplates.Any(x => RouteMatcher.Match(x, $"/{httpContext.Request.Method}{httpContext.Request.Path}")))
            {
                await _next(httpContext);
                return;
            }
            else
            {
                ResourceAuthorizationResponse authorizationResponse = new ResourceAuthorizationResponse();
                try
                {
                    logger.LogInformation(string.Format("Sending authorization request for path {0} , with ApplicationUrl {1}", $"/{httpContext.Request.Method}{httpContext.Request.Path}", _options.ApplicationUrl));
                    _policyClient = new AuthorizationPolicyClient(httpContext, _options.AuthorizationServerUrl, _options.ApplicationUrl);

                    if (_options.UseUntrustedHttpClientHandler)
                    {
                        var httpClientHandler = new HttpClientHandler();
                        httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>
                        {
                            return true;   //Is valid
                        };
                        _policyClient.ClientHandler = httpClientHandler;
                    }

                    var httpResponse = _policyClient.VerifyAccess(new ResourceAuthorizationRequest
                    {
                        AccessSource = AccessSource.API,
                        ApplicationDomain = GetDomainName(_options.ApplicationUrl),
                        Route = $"/{httpContext.Request.Method}{httpContext.Request.Path}"
                    }).Result;

                    var hasContentType = httpResponse.Content.Headers.Contains("Content-Type");
                    var contentType = hasContentType ? httpResponse.Content.Headers.GetValues("Content-Type") : null;

                    //ensure that the authorization call was successful - else block access
                    if (httpResponse.IsSuccessStatusCode && hasContentType && contentType != null && contentType.Any(x => x.Contains("application/json")))
                    {
                        logger.LogInformation(string.Format("Authorization response received for path {0} , with ApplicationUrl {1}", $"/{httpContext.Request.Method}{httpContext.Request.Path}", _options.ApplicationUrl));
                        authorizationResponse = JsonConvert.DeserializeObject<ResourceAuthorizationResponse>(await httpResponse.Content.ReadAsStringAsync());

                        if (!authorizationResponse.Succeeded)
                        {
                            logger.LogError("Authorization response failed for path {0}, error {1}", $"/{httpContext.Request.Method}{httpContext.Request.Path}", authorizationResponse.ToString());

                            httpContext.Response.ContentType = "text/plain";
                            httpContext.Response.StatusCode = 401; //UnAuthorized
                            await httpContext.Response.WriteAsync(authorizationResponse.ToString());
                        }
                    }
                    // there was un-identified exception or there was html response
                    else
                    {
                        logger.LogError("Authorization response failed for path {0}, error {1}", $"/{httpContext.Request.Method}{httpContext.Request.Path}", await httpResponse.Content.ReadAsStringAsync());

                        httpContext.Response.ContentType = "text/plain";
                        httpContext.Response.StatusCode = 401; //UnAuthorized
                        await httpContext.Response.WriteAsync(Constants.ApplicationAuthorizationFailedMessage);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError("Exception in Authorization request for path {0}, error {1}", $"/{httpContext.Request.Method}{httpContext.Request.Path}", ex.Message);

                    httpContext.Response.ContentType = "text/plain";
                    httpContext.Response.StatusCode = 401; //UnAuthorized
                    await httpContext.Response.WriteAsync(Constants.ApplicationAuthorizationFailedMessage);
                }

                if (authorizationResponse.Succeeded)
                {
                    logger.LogInformation("Authorization request successful for path {0}", $"/{httpContext.Request.Method}{httpContext.Request.Path}");
                    await _next(httpContext);
                }
            }
        }

        private static string GetDomainName(string uri)
        {
            Uri URI = new Uri(uri);
            return URI.Authority;
        }
    }
}