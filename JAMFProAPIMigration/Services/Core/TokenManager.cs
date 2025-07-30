using System.Net.Http.Headers;
using JAMFProAPIMigration.Interfacers;
using Newtonsoft.Json.Linq;

namespace JAMFProAPIMigration.Services.Core 
{
    public class TokenManager : IConfigProvider
    {
        private static readonly string JAMF_URL = Environment.GetEnvironmentVariable("JAMF_URL");
        private static readonly string CLIENT_ID = Environment.GetEnvironmentVariable("JAMF_CLIENT_ID");
        private static readonly string CLIENT_SECRET = Environment.GetEnvironmentVariable("JAMF_CLIENT_SECRET");

        public static string GetJAMFURL() => JAMF_URL;
        public static string GetCLIENT_ID() => CLIENT_ID;
        public static string GetCLIENT_SECRET() => CLIENT_SECRET;

        private static long lastCheckedTokenEpoch = 0; // Tracks the last logged token validity message
        private static string accessToken;
        private static long tokenExpirationEpoch;

        public static async Task<string> GetTokenAsync()
        {
            await CheckTokenExpiration();
            return accessToken;
        }
        private static async Task GetAccessToken()
        {
            using (var client = new HttpClient())
            {
                // Adding Accept header for JSON responses
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var requestData = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("client_id", GetCLIENT_ID()),
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_secret", GetCLIENT_SECRET())
            });

                var response = await client.PostAsync($"{GetJAMFURL()}/api/oauth/token", requestData);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var json = JObject.Parse(content);
                        accessToken = json["access_token"].ToString();
                        var expiresIn = Convert.ToInt64(json["expires_in"].ToString());
                        tokenExpirationEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + expiresIn - 1;

                        Console.WriteLine("New token recieved");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error parsing JSON: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode}");
                }
            }
        }
        private static async Task CheckTokenExpiration()
        {
            var currentEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            // Check if the token has expired
            if (tokenExpirationEpoch < currentEpoch)
            {
                Console.WriteLine("No valid token available, getting new token");
                await GetAccessToken();
            }
            else
            {
                // Only log the token validity message once or when it's refreshed
                if (lastCheckedTokenEpoch != tokenExpirationEpoch)
                {
                    Console.WriteLine($"Token valid until the following epoch time: {tokenExpirationEpoch}");
                    lastCheckedTokenEpoch = tokenExpirationEpoch; // Update the last checked time
                }
            }
        }

    }

}

