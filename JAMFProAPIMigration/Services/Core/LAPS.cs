using JAMFProAPIMigration.Interfaces;
using JAMFProAPIMigration.Services.Util;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;


namespace JAMFProAPIMigration.Services.Core
{
    public class LAPS
    {

        private readonly IComputerService _comService;

        public LAPS(IComputerService comService)
        {
            _comService = comService;
        }

        // Method to retrieve LAPS settings
        public static async Task GetLAPSSettings()
        {
            using (var client = new HttpClient())
            {
                var token = await TokenManager.GetTokenAsync();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"{ConfigProvider.GetJAMFURL()}/api/v2/local-admin-password/settings"),
                    Headers =
                {
                    { "accept", "application/json" },
                },
                };

                using (var response = await client.SendAsync(request))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Failed to retrieve LAPS settings. Status code: {response.StatusCode}");
                        var errorContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Error details: {errorContent}");
                        return;
                    }

                    var content = await response.Content.ReadAsStringAsync();

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
            }
        }

        // Method to retrieve devices with pending LAPS rotations
        public static async Task GetPendingLAPSRotations()
        {
            using (var client = new HttpClient())
            {
                var token = await TokenManager.GetTokenAsync();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"{ConfigProvider.GetJAMFURL()}/api/v2/local-admin-password/pending-rotations"),
                    Headers =
                {
                    { "accept", "application/json" },
                },
                };

                using (var response = await client.SendAsync(request))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Failed to retrieve devices with pending LAPS rotations. Status code: {response.StatusCode}");
                        var errorContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Error details: {errorContent}");
                        return;
                    }

                    var content = await response.Content.ReadAsStringAsync();

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
            using (var client = new HttpClient())
            {
                var token = await TokenManager.GetTokenAsync();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync($"{ConfigProvider.GetJAMFURL()}/api/v2/local-admin-password/{managementId}/accounts");
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to retrieve LAPS accounts. Status code: {response.StatusCode}");
                    return;
                }

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
                using (var client = await ApiManager.CreateHttpClientAsync())
                {
                    var request = ApiManager.CreateRequest(HttpMethod.Get, $"/api/v2/local-admin-password/{managementId}/accounts");

                    using (var response = await client.SendAsync(request))
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            Console.WriteLine($"Failed to retrieve LAPS accounts for {computerName}. Status code: {response.StatusCode}");
                            continue;
                        }

                        var content = await response.Content.ReadAsStringAsync();
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
