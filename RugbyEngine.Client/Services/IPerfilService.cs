using RugbyEngine.Shared.Perfiles;

namespace RugbyEngine.Client.Services
{
    public interface IPerfilService
    {
        Task<List<PerfilDTO>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<List<PerfilDTO>> GetMineAsync(CancellationToken cancellationToken = default);
        Task<bool> AssignAsync(int perfilId, CancellationToken cancellationToken = default);
        Task<bool> UnassignAsync(int perfilId, CancellationToken cancellationToken = default);
    }
}
