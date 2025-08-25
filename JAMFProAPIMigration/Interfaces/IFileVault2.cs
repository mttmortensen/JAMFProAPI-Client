using JAMFProAPIMigration.Models.DTOs;

namespace JAMFProAPIMigration.Interfaces
{
    public interface IFileVault2
    {
        Task<List<FileVaultInventoryItem>> GetFileVaultInventoryAsync();
    }
}
