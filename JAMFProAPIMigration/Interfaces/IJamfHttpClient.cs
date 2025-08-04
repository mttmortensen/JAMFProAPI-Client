namespace JAMFProAPIMigration.Interfaces
{
    public interface IJamfHttpClient
    {
        Task<T> GetAsync<T>(string endpoint);
        Task<T> PostAsync<TRequest, TResponse>(string endpoint, TRequest payload);

    }
}
