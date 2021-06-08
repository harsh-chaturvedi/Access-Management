using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Access_Management.Client.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Access_Management.Client.Client
{
    public class ApiClient
    {
        private HttpClient _client;
        private HttpRequestMessage _request;
        public HttpClientHandler ClientHandler { get; set; }

        private readonly bool _useBearerToken;
        private readonly HttpContext _context;
        private readonly string _tenantUri;

        public ApiClient(HttpContext context, string environmentUri)
        {
            _useBearerToken = true;
            _context = context;
            _tenantUri = environmentUri;
        }
        
        /// <summary>
        /// Adds the Authorization header
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="httpMethod"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> SendRequestAsync(string uri, HttpMethod httpMethod, object content)
        {
            return await ProcessRequestAsync(new Uri(uri), httpMethod, content);
        }

        public async Task<HttpResponseMessage> SendRequestAsync(Uri uri, HttpMethod httpMethod, object content)
        {
            return await ProcessRequestAsync(uri, httpMethod, content);
        }

        private async Task<HttpResponseMessage> ProcessRequestAsync(Uri uri, HttpMethod httpMethod, object content)
        {
            if (string.IsNullOrEmpty(_tenantUri))
                throw new Exception("Environment Uri is missing!");

            _request = new HttpRequestMessage
            {
                Method = httpMethod,
                RequestUri = uri,
                Content = new StringContent(JsonConvert.SerializeObject(content, Formatting.Indented), Encoding.UTF8, "application/json"),
            };

            if (_useBearerToken)
            {
                var bearerToken = _context.GetTokenAsync(JwtBearerDefaults.AuthenticationScheme, "access_token").Result;

                if (string.IsNullOrEmpty(bearerToken))
                    throw new InvalidTokenException("No bearer access_token found, User not authorized!!");

                _request.Headers.Add("Authorization", $"Bearer {bearerToken}");
            }
            

            _request.Headers.Add(Constants.TenantUri, _tenantUri);

            if (ClientHandler != null)
            {
                using (_client = new HttpClient(ClientHandler))
                {
                    return await _client.SendAsync(_request);
                }
            }
            _client = new HttpClient();
            return await _client.SendAsync(_request);
        }
    }
}