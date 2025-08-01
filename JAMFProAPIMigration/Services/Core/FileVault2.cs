using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace JAMFProAPIMigration.Services.Core
{
    public class FileVault2 : ApiManager
    {
        public static async Task<List<string>> GetFileVault2UsersAsync(string computerId)
        {
            using (var client = await CreateHttpClientAsync())
            {
                var endpoint = $"/api/v1/computers-inventory/{computerId}/filevault";
                var request = CreateRequest(HttpMethod.Get, endpoint);

                using (var response = await client.SendAsync(request))
                {
                    var content = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Failed to retrieve FileVault 2 settings. Status: {response.StatusCode}");
                        Console.WriteLine($"Response: {content}");
                        return null;
                    }

                    try
                    {
                        var json = JObject.Parse(content);

                        // Only proceed if "fileVaultUsers" key exists
                        if (json["fileVaultUsers"] == null)
                        {
                            Console.WriteLine("No FileVault 2 users configured for the specified computer.");
                            return new List<string>();
                        }

                        var fileVaultUsers = json["fileVaultUsers"] as JArray;
                        var users = new List<string>();

                        foreach (var user in fileVaultUsers)
                        {
                            var username = user["username"]?.ToString();
                            if (!string.IsNullOrEmpty(username))
                            {
                                users.Add(username);
                            }
                        }

                        return users;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error parsing JSON: {ex.Message}");
                        return null;
                    }
                }
            }
        }


    }
}
