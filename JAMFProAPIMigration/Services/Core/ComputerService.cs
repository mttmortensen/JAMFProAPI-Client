using JAMFProAPIMigration.Interfaces;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;

namespace JAMFProAPIMigration.Services.Core
{
    public class ComputerService: IComputerService
    {

        private readonly IJamfHttpClient _client;

        public ComputerService(IJamfHttpClient client)
        {
            _client = client;
        }

        // Method to retrieve computer ID based on computer name
        public async Task<string> GetComputerIdByName(string computerName)
        {
            // 1. Ask our client to GET the Classic API with Accept: XML
            var content = await _client.GetStringAsync
                (
                    $"/JSSResource/computers/match/{computerName}",
                    accept: "application/xml"
                );

            // 2. Parse the XML 
            try 
            {
                var xml = XDocument.Parse(content);
                var computerElement = xml.Descendants("computer").FirstOrDefault();
                if (computerElement == null) return null;

                var computerId = computerElement.Element("id")?.Value;
                return string.IsNullOrEmpty(computerId) ? null : computerId;
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Error pasrsing XML: {ex.Message}");
                return null;
            }
            
        }

        // Add a method to retrieve the management ID based on computer ID
        public async Task<string> GetManagementIdByComputerId(string computerId)
        {
            using (var client = await ApiManager.CreateHttpClientAsync())
            {
                var request = ApiManager.CreateRequest(HttpMethod.Get, $"/api/v1/computers-inventory/{computerId}");

                using (var response = await client.SendAsync(request))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Failed to retrieve computer inventory. Status code: {response.StatusCode}");
                        return null;
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var json = JObject.Parse(content);
                        var managementId = json["general"]?["managementId"]?.ToString();

                        if (string.IsNullOrEmpty(managementId))
                        {
                            Console.WriteLine("Management ID not found for the specified computer ID.");
                            return null;
                        }

                        Console.WriteLine($"Management ID for Computer ID {computerId}: {managementId}");
                        return managementId;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error parsing JSON: {ex.Message}");
                        return null;
                    }
                }
            }
        }

        public async Task<List<(string computerId, string computerName)>> GetAllComputers()
        {
            using (var client = await ApiManager.CreateHttpClientAsync())
            {
                var request = ApiManager.CreateRequest(HttpMethod.Get, "/JSSResource/computers");

                using (var response = await client.SendAsync(request))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Failed to retrieve computers. Status code: {response.StatusCode}");
                        var errorContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Error details: {errorContent}");
                        return new List<(string computerId, string computerName)>();
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var json = JObject.Parse(content);
                        var computers = json["computers"] as JArray;
                        var result = new List<(string computerId, string computerName)>();

                        if (computers != null)
                        {
                            foreach (var computer in computers)
                            {
                                var computerId = computer["id"]?.ToString();
                                var computerName = computer["name"]?.ToString();

                                if (!string.IsNullOrEmpty(computerId) && !string.IsNullOrEmpty(computerName))
                                {
                                    result.Add((computerId, computerName));
                                }
                            }
                        }

                        return result;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error parsing JSON: {ex.Message}");
                        return new List<(string computerId, string computerName)>();
                    }
                }
            }
        }

        // Method to get all computers from Jamf inventory (BETA)
        public async Task<List<(string computerId, string managementId)>> GetAllManagementIds()
        {
            using (var client = await ApiManager.CreateHttpClientAsync())
            {
                var request = ApiManager.CreateRequest(HttpMethod.Get, "/api/v1/computers-inventory");

                using (var response = await client.SendAsync(request))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Failed to retrieve computers inventory. Status code: {response.StatusCode}");
                        return null;
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var json = JObject.Parse(content);
                        var computers = json["computers"] as JArray;
                        var result = new List<(string computerId, string managementId)>();

                        if (computers != null)
                        {
                            foreach (var computer in computers)
                            {
                                var computerId = computer["id"]?.ToString();
                                var managementId = computer["general"]?["managementId"]?.ToString();
                                if (!string.IsNullOrEmpty(computerId) && !string.IsNullOrEmpty(managementId))
                                {
                                    result.Add((computerId, managementId));
                                }
                            }
                        }

                        Console.WriteLine("Computers Retrieved:");
                        foreach (var computer in result)
                        {
                            Console.WriteLine($"Computer ID: {computer.computerId}, Management ID: {computer.managementId}");
                        }
                        return result;
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
