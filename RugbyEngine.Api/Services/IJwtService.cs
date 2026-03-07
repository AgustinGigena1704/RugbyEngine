using System.Security.Claims;

namespace RugbyEngine.Api.Services
{
    public interface IJwtService
    {
        string GenerateToken(int userId);
        ClaimsPrincipal? ValidateToken(string token);
        bool ShouldRefreshToken(string token);
        string? RefreshToken(string token);
    }
}
