using RugbyEngine.Shared.Auth;
using System.Threading;
using System.Threading.Tasks;

namespace RugbyEngine.Client.Services
{
    public interface IAuthService
    {
        event Action? AuthenticationStateChanged;

        Task<LoginResponse> LoginAsync(LoginDTO request, CancellationToken cancellationToken = default);

        Task LogoutAsync(bool notifyServer = true, CancellationToken cancellationToken = default);

        Task<bool> IsAuthenticatedAsync();

        Task<string?> GetTokenAsync();
    }
}
