namespace JAMFProAPIMigration.Interfaces
{
    public interface ITokenManager
    {
        Task<string> GetTokenAsync();

    }
}
