namespace JAMFProAPIMigration.Interfaces
{
    public interface IJamfHttpClient
    {
        Task<T> GetAsync<T>(string endpoint);

        // We are adding a type to work with
        // And telling what type we're waning back 
        // This is done with PostAsync<T,T> 
        // TRquest is the request type we're sending in
        // And TResponse is the type we expect to come out. 
        Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest payload);


        // A raw GET that allows me to pick the Accept header
        Task<string> GetStringAsync(string endpoint, string accept = "application/json");
    }
}
