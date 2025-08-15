using JAMFProAPIMigration.Interfaces;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace JAMFProAPIMigration.Services.Core
{
    public class FileVault2: IFileVault2
    {

        private readonly IJamfHttpClient _client;

        public FileVault2(IJamfHttpClient client)
        {
            _client = client;
        }

        public async Task<List<string>> GetFileVault2UsersAsync(string computerId)
        {
            var content = await _client.GetStringAsync($"/api/v1/computers-inventory/{computerId}/filevault", accept: "application/json");

            try
            {
                // Future: DTO to replace JObject
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
