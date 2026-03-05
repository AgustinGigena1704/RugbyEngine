using Microsoft.JSInterop;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace RugbyEngine.Client.Services
{
    public class CookieService : ICookieService
    {
        private readonly IJSRuntime _jsRuntime;

        public CookieService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public Task SetCookieAsync(string name, string value, DateTimeOffset? expires = null)
        {
            var encodedValue = Uri.EscapeDataString(value);
            var expiresString = expires?.UtcDateTime.ToString("R", CultureInfo.InvariantCulture);
            return _jsRuntime.InvokeVoidAsync("cookieInterop.set", name, encodedValue, expiresString).AsTask();
        }

        public async Task<string?> GetCookieAsync(string name)
        {
            var encoded = await _jsRuntime.InvokeAsync<string?>("cookieInterop.get", name);
            return string.IsNullOrWhiteSpace(encoded) ? null : Uri.UnescapeDataString(encoded);
        }

        public Task DeleteCookieAsync(string name)
        {
            return _jsRuntime.InvokeVoidAsync("cookieInterop.delete", name).AsTask();
        }
    }
}
