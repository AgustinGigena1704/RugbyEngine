using Microsoft.JSInterop;
using System.Text.Json;

namespace RugbyEngine.Client.Services
{
    public class StorageService : IStorageService
    {
        private readonly IJSRuntime _js;

        public StorageService(IJSRuntime js)
        {
            _js = js;
        }

        // ── SessionStorage ──────────────────────────────────────────

        public async Task<T?> GetSessionItemAsync<T>(string key)
        {
            var json = await _js.InvokeAsync<string?>("storageInterop.getSession", key);
            return Deserialize<T>(json);
        }

        public async Task SetSessionItemAsync<T>(string key, T value)
        {
            var json = JsonSerializer.Serialize(value);
            await _js.InvokeVoidAsync("storageInterop.setSession", key, json);
        }

        public async Task RemoveSessionItemAsync(string key)
        {
            await _js.InvokeVoidAsync("storageInterop.removeSession", key);
        }

        // ── LocalStorage ────────────────────────────────────────────

        public async Task<T?> GetLocalItemAsync<T>(string key)
        {
            var json = await _js.InvokeAsync<string?>("storageInterop.getLocal", key);
            return Deserialize<T>(json);
        }

        public async Task SetLocalItemAsync<T>(string key, T value)
        {
            var json = JsonSerializer.Serialize(value);
            await _js.InvokeVoidAsync("storageInterop.setLocal", key, json);
        }

        public async Task RemoveLocalItemAsync(string key)
        {
            await _js.InvokeVoidAsync("storageInterop.removeLocal", key);
        }

        // ── Helpers ─────────────────────────────────────────────────

        private static T? Deserialize<T>(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return default;

            // When T is string, return the raw value without JSON deserialization
            // so that plain values set via browser console (e.g. "true") work directly.
            if (typeof(T) == typeof(string))
                return (T)(object)json;

            try
            {
                return JsonSerializer.Deserialize<T>(json);
            }
            catch
            {
                return default;
            }
        }
    }
}
