namespace JAMFProAPIMigration.Interfaces
{
    public interface IComputerService
    {
        Task<string> GetComputerIdByName(string computer);
        Task<string> GetManagementIdByComputerId(string computerId);

        // These truples will need to be models sometime soon
        Task<List<(string computerId, string computerName)>> GetAllComputers();
        Task<List<(string computerId, string managementId)>> GetAllManagementIds();
        
    }
}
