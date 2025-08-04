using JAMFProAPIMigration.Interfaces;
using JAMFProAPIMigration.Services.Util;
using System.Net.Http.Headers;

namespace JAMFProAPIMigration.Services.Core
{
    public class JamfHttpClient : IJamfHttpClient
    {
        private readonly HttpClient _client;
        private readonly TokenManager _tokenManager;

        public JamfHttpClient(HttpClient client, TokenManager tokenManager)
        {
            _client = client;
            _tokenManager = tokenManager;
        }

        public Task<T> GetAsync<T>(string endpoint) 
        {
            throw new NotImplementedException();
        }

        public Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest payload)
        {

            // --- 1. Grab a fresh token each call ----
            var token = _tokenManager.GetTokenAsync();

            // TODO: serialize payload + call _client.PostAsync + deserialize
            throw new NotImplementedException();
        }

        private HttpRequestMessage CreateRequest(HttpMethod method, string endpoint, string contentType = "application/json")
        {
            var request = new HttpRequestMessage(method, endpoint);

            request.Headers.Accept.Add
                (
                    new MediaTypeWithQualityHeaderValue(contentType)
                );

            return request;
        }
    }
}
