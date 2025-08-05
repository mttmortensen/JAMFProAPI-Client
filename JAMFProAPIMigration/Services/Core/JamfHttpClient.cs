using JAMFProAPIMigration.Interfaces;
using JAMFProAPIMigration.Services.Util;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

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

        public async Task<T> GetAsync<T>(string endpoint) 
        {
            // 1. --- Grab the token ---
            var token = await _tokenManager.GetTokenAsync();

            // 2. --- Build out req skeleton --- 
            var request = CreateRequest(HttpMethod.Get, endpoint);

            // 3. --- Attach Bearer token to headers for Request ---
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // 4. --- Sending the GET response ---
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // 5. --- Deserialize JSON into C# Obj i.e. <T> ---
            var stream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<T>(stream);
        }

        public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest payload)
        {

            // --- 1. Grab a fresh token each call ----
            var token = await _tokenManager.GetTokenAsync();

            // --- 2. Build the request skeleton ---
            var request = CreateRequest(HttpMethod.Get, endpoint);

            // --- 3. Attach the JSON body ---
            //  A. Turn the payload object into a JSON string: 
            var jsonString = JsonSerializer.Serialize(payload);

            //  B. Wrap that string in an HttpContent object, and proper headers: 
            // StringContent is HttpContent object.
            request.Content = new StringContent
                (
                    jsonString,
                    Encoding.UTF8,
                    "application/json"
                );

            // --- 4. Adding the Bearer token header ---
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // --- 5. Send and Parse ----
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // Streams are easier to load into memory since it's not 
            // one giant block being thrown in. It comes in steadily as bytes.
            // The next line will send make it into object we can use in the service layer
            var stream = await response.Content.ReadAsStreamAsync();

            // This is going to return whatever JAMF has setup for this return. 
            // Helps it make more generic of a response vs something specific. 
            return await JsonSerializer.DeserializeAsync<TResponse>(stream);
            
        }


        // Making this private due to what the IJamfHttpClient is standing for 
        // Its only providing a contract for CRUD operations. 
        // Creating the Http Request doesn't fall under that interface although it is still needed to be made. 
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
