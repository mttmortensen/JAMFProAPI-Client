using JAMFProAPIMigration.Interfaces;
using JAMFProAPIMigration.Services.Util;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;


namespace JAMFProAPIMigration.Services.Core
{
    public class LAPS : ILAPS
    {

        private readonly IComputerService _comService;
        private readonly IJamfHttpClient _client;

        public LAPS(IComputerService comService, IJamfHttpClient client)
        {
            _comService = comService;
            _client = client;
        }

        // Method to retrieve LAPS settings
        public async Task GetLAPSSettings()
        {
            var content = await _client.GetStringAsync("/api/v2/local-admin-password/settings");

            try
            {
                var json = JObject.Parse(content);
                Console.WriteLine("LAPS Settings:");
                Console.WriteLine(json.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing JSON: {ex.Message}");
            }
        }

        // Method to retrieve devices with pending LAPS rotations
        public async Task GetPendingLAPSRotations()
        {
            var content = await _client.GetStringAsync("/api/v2/local-admin-password/pending-rotations");

            try
            {
                var json = JObject.Parse(content);
                Console.WriteLine("Devices with Pending LAPS Rotations:");
                Console.WriteLine(json.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing JSON: {ex.Message}");
            }
        }

        // Process Function to retrieve LAPS accounts using Computer Name
        public async Task ProcessLAPSAccounts(string computerName)
        {
            // Step 1: Get Computer ID by name
            var computerId = await _comService.GetComputerIdByName(computerName);
            if (string.IsNullOrEmpty(computerId))
            {
                Console.WriteLine($"Could not find computer ID for {computerName}. Aborting process.");
                return;
            }

            // Step 2: Get Management ID by Computer ID
            var managementId = await _comService.GetManagementIdByComputerId(computerId);
            if (string.IsNullOrEmpty(managementId))
            {
                Console.WriteLine($"Could not find management ID for computer ID {computerId}. Aborting process.");
                return;
            }

            // Step 3: Retrieve LAPS accounts using Management ID

            var content = await _client.GetStringAsync("/api/v2/local-admin-password/{managementId}/accounts");

            try
            {
                var lapsJson = JObject.Parse(content);
                Console.WriteLine("LAPS Accounts:");
                Console.WriteLine(lapsJson.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing JSON: {ex.Message}");
            }
        }

        // Method to get all computers without LAPS enabled
        public async Task<List<string>> GetComputersWithoutLAPSUsers()
        {
            var allComputers = await _comService.GetAllComputers();

            if (allComputers.Count == 0)
            {
                Console.WriteLine("No computers retrieved from JAMF.");
                return new List<string>();
            }

            var computersWithoutLAPS = new List<string>();

            foreach (var (computerId, computerName) in allComputers)
            {
                // Step 1: Get Management ID for the computer
                var managementId = await _comService.GetManagementIdByComputerId(computerId);
                if (string.IsNullOrEmpty(managementId))
                {
                    Console.WriteLine($"Management ID not found for computer {computerName} (ID: {computerId}). Skipping.");
                    continue;
                }

                // Step 2: Check LAPS accounts for the computer
                
                var content = await _client.GetStringAsync("/api/v2/local-admin-password/{managementId}/accounts");

                try
                {
                    var json = JObject.Parse(content);
                    var accounts = json["results"] as JArray;

                    // Check if only "jamf_manage" account exists
                    if (accounts != null && accounts.Count == 1 && accounts[0]["username"]?.ToString() == "jamf_manage")
                    {
                        computersWithoutLAPS.Add(computerName);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing JSON for {computerName}: {ex.Message}");
                }
            }

            Console.WriteLine($"Number of Computers without LAPS users: {computersWithoutLAPS.Count}");
            Console.WriteLine("Computers Without LAPS Users:");
            foreach (var computerName in computersWithoutLAPS)
            {
                Console.WriteLine($"Computer Name: {computerName}");
            }
            return computersWithoutLAPS;
        }
    }
}
