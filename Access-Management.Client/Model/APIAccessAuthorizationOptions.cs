using System.Collections.Generic;

namespace Access_Management.Client.Model
{
    public class APIAccessAuthorizationOptions
    {
        public string AuthorizationServerUrl { get; set; }

        public string ApplicationUrl { get; set; }

        public bool UseUntrustedHttpClientHandler { get; set; }

        public string RouteSegmentToMatch { get; set; }

        public List<string> AllowedNonAuthorizedRouteTemplates { get; set; }
    }
}