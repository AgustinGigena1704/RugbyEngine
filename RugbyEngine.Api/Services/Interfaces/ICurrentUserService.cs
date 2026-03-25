namespace RugbyEngine.Api.Services.Interfaces
{
    public interface ICurrentUserService
    {
        int? GetCurrentUserId();
        Task<Data.Entities.Usuario?> GetCurrentUserAsync();
        bool IsAuthenticated();
    }
}
