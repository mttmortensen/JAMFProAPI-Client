using JAMFProAPIMigration.Services.Util;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Xml.Linq;

namespace JAMFProAPIMigration.Services.Core 
{
    public abstract class ApiManager 
    {

        protected static async Task<HttpClient> CreateHttpClientAsync()
        {
            var token = await TokenManager.GetTokenAsync();
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        protected static HttpRequestMessage CreateRequest(HttpMethod method, string endpoint, string contentType = "application/json")
        {

            return new HttpRequestMessage
            {
                Method = method,
                RequestUri = new Uri($"{ConfigProvider.GetJAMFURL()}{endpoint}"),
                Headers = { { "accept", contentType } }
            };
        }

        /*=====================================================================*/
        /*JAMF TOOLS*/
        /*=====================================================================*/

        // Method to retrieve computer ID based on computer name
        public static async Task<string> GetComputerIdByName(string computerName)
        {

            using (var client = await CreateHttpClientAsync())
            {
                var request = CreateRequest(HttpMethod.Get, $"/JSSResource/computers/match/{computerName}", "application/xml");

                using (var response = await client.SendAsync(request))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Failed to retrieve computer ID. Status code: {response.StatusCode}");
                        return null;
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var xml = XDocument.Parse(content);
                        var computerElement = xml.Descendants("computer").FirstOrDefault();

                        if (computerElement == null)
                        {
                            Console.WriteLine("Computer not found for the specified name.");
                            return null;
                        }

                        var computerId = computerElement.Element("id")?.Value;

                        if (string.IsNullOrEmpty(computerId))
                        {
                            Console.WriteLine("Computer ID not found for the specified name.");
                            return null;
                        }

                        Console.WriteLine($"Computer ID for {computerName}: {computerId}");
                        return computerId;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error parsing XML: {ex.Message}");
                        return null;
                    }
                }
            }
        }

        // Add a method to retrieve the management ID based on computer ID
        public static async Task<string> GetManagementIdByComputerId(string computerId)
        {
            using (var client = await CreateHttpClientAsync())
            {
                var request = CreateRequest(HttpMethod.Get, $"/api/v1/computers-inventory/{computerId}");

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

        public static async Task<List<(string computerId, string computerName)>> GetAllComputers()
        {
            using (var client = await CreateHttpClientAsync())
            {
                var request = CreateRequest(HttpMethod.Get, "/JSSResource/computers");

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
        public static async Task<List<(string computerId, string managementId)>> GetAllManagementIds()
        {
            using (var client = await CreateHttpClientAsync())
            {
                var request = CreateRequest(HttpMethod.Get, "/api/v1/computers-inventory");

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


