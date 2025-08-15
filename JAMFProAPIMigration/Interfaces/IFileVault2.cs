namespace JAMFProAPIMigration.Interfaces
{
    public interface IFileVault2
    {
        Task<List<string>> GetFileVault2UsersAsync(string computerId);
    }
}
