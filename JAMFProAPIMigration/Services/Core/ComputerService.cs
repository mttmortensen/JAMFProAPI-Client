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
            var content = await _client.GetStringAsync($"/api/v1/computers-inventory/{computerId}");

            try 
            {
                var json = JObject.Parse(content);
                var managementId = json["general"]?["managementId"]?.ToString();
                return string.IsNullOrEmpty(managementId) ? null : managementId;
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Error parsing JSON: {ex.Message}");
                return null;
            }
        }

        // Future update: Turning the turple into a Dto Model 
        // Get
        public async Task<List<(string computerId, string computerName)>> GetAllComputers()
        {
            var content = await _client.GetStringAsync("/JSSResource/computers", accept: "application/json");

            try 
            {
                var json = JObject.Parse(content);
                var computers = json["computers"] as JArray;

                var result = new List<(string computerId, string computerName)>();
                if(computers != null ) 
                {
                    foreach (var computer in computers) 
                    {
                        var id = computer["id"]?.ToString();
                        var name = computer["name"]?.ToString();
                        if(!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(name)) 
                        {
                            result.Add((id, name));
                        }
                    }
                }

                return result;
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Error parsing JSON: {ex.Message}");
                return new List<(string, string)>();
            }
        }

        // Method to get all computers from Jamf inventory (BETA)
        // Future: Another DTO will be applied for Com and Mang Ids
        public async Task<List<(string computerId, string managementId)>> GetAllManagementIds()
        {
            var content = await _client.GetStringAsync("/api/v1/computers-inventory");

            try 
            {
                var json = JObject.Parse(content);
                var computers = json["computers"] as JArray;

                var result = new List<(string computerId, string managementId)>();
                if (computers != null)
                {
                    foreach (var computer in computers) 
                    {
                        var id = computer["id"]?.ToString();
                        var mgmId = computer["general"]?["managementId"]?.ToString();
                        if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(mgmId))
                            result.Add((id, mgmId));
                    }
                }

                return result;
            }
            catch(Exception ex) 
            {
                Console.WriteLine($"Error parsing JSON: {ex.Message}");
                return new List<(string, string)>();
            }
        }
    }
}
