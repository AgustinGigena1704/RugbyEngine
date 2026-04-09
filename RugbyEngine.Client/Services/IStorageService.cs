namespace RugbyEngine.Client.Services
{
    public interface IStorageService
    {
        Task<T?> GetSessionItemAsync<T>(string key);
        Task SetSessionItemAsync<T>(string key, T value);
        Task RemoveSessionItemAsync(string key);

        Task<T?> GetLocalItemAsync<T>(string key);
        Task SetLocalItemAsync<T>(string key, T value);
        Task RemoveLocalItemAsync(string key);
    }
}
