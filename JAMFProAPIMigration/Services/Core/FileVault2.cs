using JAMFProAPIMigration.Interfaces;
using JAMFProAPIMigration.Models.DTOs;
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

        public async Task<List<FileVaultInventoryItem>> GetFileVaultInventoryAsync()
        {
            var content = await _client.GetStringAsync($"/api/v1/computers-inventory/filevault", accept: "application/json");

            try
            {
                // Parse the JSON into an Object
                var root = JObject.Parse(content);

                // If results are missing/empty, return empty list. 
                var results = root["results"] as JArray;
                if ( results == null || results.Count == 0 ) 
                {
                    return new List<FileVaultInventoryItem>();
                }


            }
            catch (Exception ex)
            {

            }

        }

    }
}
