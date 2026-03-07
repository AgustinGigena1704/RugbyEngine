using System;
using System.Threading.Tasks;

namespace RugbyEngine.Client.Services
{
    public interface ICookieService
    {
        Task SetCookieAsync(string name, string value, DateTimeOffset? expires = null);

        Task<string?> GetCookieAsync(string name);

        Task DeleteCookieAsync(string name);
    }
}
