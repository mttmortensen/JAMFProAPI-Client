namespace JAMFProAPIMigration.Interfaces
{
    public interface ILAPS
    {
        Task GetLAPSSettings();
        Task GetPendingLAPSRotations();
        Task ProcessLAPSAccounts(string computerName);
        Task<List<string>> GetComputersWithoutLAPSUsers();
    }
}
