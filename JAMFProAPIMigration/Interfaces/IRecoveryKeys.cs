namespace JAMFProAPIMigration.Interfaces
{
    public interface IRecoveryKeys
    {
        Task<string> GetRecoveryKeyById(string computerId);
        Task RemoveRecoveryKeyIfExists(string computerId);
        Task ProcessRecoveryKeyRemoval(string computerName);
    }
}
