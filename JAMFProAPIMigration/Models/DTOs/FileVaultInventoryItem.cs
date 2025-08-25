using System.Text;

namespace JAMFProAPIMigration.Models.DTOs
{
    public class FileVaultInventoryItem
    {
        public string? ComputerId { get; set; }
        public string? Name { get; set; }
        public string? PersonalRecoveryKey { get; set; }
        public string? IndividualKeyStatus { get; set; }
    }
}
