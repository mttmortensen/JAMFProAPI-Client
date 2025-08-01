using JAMFProAPIMigration.Services.Util;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Xml.Linq;

namespace JAMFProAPIMigration.Services.Core 
{
    public class ApiManager 
    {

        public static async Task<HttpClient> CreateHttpClientAsync()
        {
            var token = await TokenManager.GetTokenAsync();
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        public static HttpRequestMessage CreateRequest(HttpMethod method, string endpoint, string contentType = "application/json")
        {

            return new HttpRequestMessage
            {
                Method = method,
                RequestUri = new Uri($"{ConfigProvider.GetJAMFURL()}{endpoint}"),
                Headers = { { "accept", contentType } }
            };
        }

    }
}


