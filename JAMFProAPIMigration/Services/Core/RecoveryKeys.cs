using JAMFProAPIMigration.Interfaces;
using JAMFProAPIMigration.Services.Util;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace JAMFProAPIMigration.Services.Core
{
    public class RecoveryKeys : IRecoveryKeys
    {

        private readonly IComputerService _comService;
        private readonly IJamfHttpClient _client;

        public RecoveryKeys(IComputerService comService, IJamfHttpClient client) 
        {
            _comService = comService;
            _client = client;
        }

        // Method to retrieve the recovery key if available
        public async Task<string> GetRecoveryKeyById(string computerId)
        {
            using (var client = new HttpClient())
            {
                var token = await TokenManager.GetTokenAsync();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync($"{ConfigProvider.GetJAMFURL()}/api/v1/computers-inventory/{computerId}/view-recovery-lock-password");
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to retrieve recovery key. Status code: {response.StatusCode}");
                    return null;
                }

                var json = JObject.Parse(content);
                var recoveryKey = json["recoveryLockPassword"]?.ToString();

                if (string.IsNullOrEmpty(recoveryKey))
                {
                    Console.WriteLine("No recovery key found.");
                    return null;
                }

                Console.WriteLine($"Recovery key retrieved successfully: {recoveryKey} ");
                return recoveryKey;
            }
        }

        // Method to remove the recovery key if it exists
        public async Task RemoveRecoveryKeyIfExists(string computerId)
        {
            var recoveryKey = await GetRecoveryKeyById(computerId);
            if (string.IsNullOrEmpty(recoveryKey))
            {
                Console.WriteLine("No recovery key to remove.");
                return;
            }

            using (var client = new HttpClient())
            {
                var token = await TokenManager.GetTokenAsync();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri($"{ConfigProvider.GetJAMFURL()}/api/v2/mdm/commands"),
                    Headers =
                {
                    { "accept", "application/json" },
                },
                    Content = new StringContent($"{{\"command\": \"ClearRecoveryLock\", \"computerIds\": [\"{computerId}\"]}}")
                    {
                        Headers =
                    {
                        ContentType = new MediaTypeHeaderValue("application/json")
                    }
                    }
                };

                using (var response = await client.SendAsync(request))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Recovery key has been successfully removed.");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to remove recovery key. Status code: {response.StatusCode}");
                        var errorContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Error details: {errorContent}");
                    }
                }
            }
        }

        // Process Function that will pull some of the methods above without creating any dependencies
        public async Task ProcessRecoveryKeyRemoval(string computerName)
        {
            // Step 1: Get Computer ID by name
            var computerId = await GetComputerIdByName(computerName);
            if (string.IsNullOrEmpty(computerId))
            {
                Console.WriteLine($"Could not find computer ID for {computerName}. Aborting process.");
                return;
            }

            // Step 2: Get Recovery Key by ID
            var recoveryKey = await GetRecoveryKeyById(computerId);
            if (string.IsNullOrEmpty(recoveryKey))
            {
                Console.WriteLine($"No recovery key found for computer ID {computerId}. Nothing to remove.");
                return;
            }

            // Step 3: Remove Recovery Key
            await RemoveRecoveryKeyIfExists(computerId);
        }
    }
}
