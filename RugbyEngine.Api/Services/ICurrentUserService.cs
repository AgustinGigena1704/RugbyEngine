namespace RugbyEngine.Api.Services
{
    public interface ICurrentUserService
    {
        int? GetCurrentUserId();
        Task<RugbyEngine.Api.Data.Entities.Usuario?> GetCurrentUserAsync();
        bool IsAuthenticated();
    }
}
