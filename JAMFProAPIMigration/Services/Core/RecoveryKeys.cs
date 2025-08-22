using JAMFProAPIMigration.Interfaces;
using JAMFProAPIMigration.Models.DTOs;
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

            var content = await _client.GetStringAsync("/api/v1/computers-inventory/{computerId}/view-recovery-lock-password");
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

        // Method to remove the recovery key if it exists
        public async Task RemoveRecoveryKeyIfExists(string computerId)
        {
            //  0. Bail early if there is no key to start with
            var recoveryKey = await GetRecoveryKeyById(computerId);
            if (string.IsNullOrEmpty(recoveryKey))
            {
                Console.WriteLine("No recovery key to remove");
                return;
            }

            // 1. Build out the request dto 
            var payload = new MdmCommandRequest
            {
                command = "ClearRecoveryLock",
                computerIds = new[] { computerId }
            };

            // 2. Send it through the wrapper, throws if Jamf returns non-2xx
            try 
            {
                // Testing for what response comes back. Object as TResponse
                var result = await _client.PostAsync<MdmCommandRequest, MdmCommandResponse>("/api/v2/mdm/commands", payload);

                // 3. Success
                Console.WriteLine("Recovery key has been successfully remove.");
                Console.WriteLine(result);
            }
            // 4. Non-2xx or network errs bubble up as excptions because of EnsureSuccessStatusCode
            catch (HttpRequestException ex) 
            {
                Console.WriteLine($"Failed to remove recovery key: {ex.Message}");
            }
            catch (TaskCanceledException ex) 
            {
                Console.WriteLine($"Request timed out while removing recovery key: {ex.Message}");
            }
        }

        // Process Function that will pull some of the methods above without creating any dependencies
        public async Task ProcessRecoveryKeyRemoval(string computerName)
        {
            // Step 1: Get Computer ID by name
            var computerId = await _comService.GetComputerIdByName(computerName);
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
