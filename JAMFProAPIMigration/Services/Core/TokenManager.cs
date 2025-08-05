using JAMFProAPIMigration.Services.Util;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace JAMFProAPIMigration.Services.Core 
{
    public class TokenManager
    {
        private static long lastCheckedTokenEpoch = 0; // Tracks the last logged token validity message
        private static string accessToken;
        private static long tokenExpirationEpoch;

        public async Task<string> GetTokenAsync()
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
                new KeyValuePair<string, string>("client_id", ConfigProvider.GetCLIENT_ID()),
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_secret", ConfigProvider.GetCLIENT_SECRET())
            });

                var response = await client.PostAsync($"{ConfigProvider.GetJAMFURL()}/api/oauth/token", requestData);
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

