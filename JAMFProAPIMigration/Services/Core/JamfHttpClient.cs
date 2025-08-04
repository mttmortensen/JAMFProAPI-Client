using JAMFProAPIMigration.Interfaces;

namespace JAMFProAPIMigration.Services.Core
{
    public class JamfHttpClient : IJamfHttpClient
    {
        private readonly HttpClient _client;

        public JamfHttpClient(HttpClient client)
        {
            _client = client;
        }

        public Task<T> GetAsync<T>(string endpoint) 
        {
            throw new NotImplementedException();
        }

        public Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest payload)
        {
            // TODO: serialize payload + call _client.PostAsync + deserialize
            throw new NotImplementedException();
        }
    }
}
