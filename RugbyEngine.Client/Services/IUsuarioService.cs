using RugbyEngine.Shared.Tablas;
using RugbyEngine.Shared.Usuarios;

namespace RugbyEngine.Client.Services
{
    public interface IUsuarioService
    {
        Task<List<UsuarioResponse>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<TableResponse<UsuarioResponse>?> GetTableAsync(PaginacionDto paginacion, string? searchText, CancellationToken cancellationToken = default);
        Task<int> CountAsync(string? searchText, CancellationToken cancellationToken = default);
        Task<UsuarioResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<UsuarioResponse?> CreateAsync(UsuarioCreateDto dto, CancellationToken cancellationToken = default);
        Task<UsuarioResponse?> UpdateAsync(int id, UsuarioUpdateDto dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> AssignPerfilAsync(int usuarioId, int perfilId, CancellationToken cancellationToken = default);
        Task<bool> UnassignPerfilAsync(int usuarioId, int perfilId, CancellationToken cancellationToken = default);
    }
}
