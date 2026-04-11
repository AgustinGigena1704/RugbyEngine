using RugbyEngine.Shared.Entrenamientos;
using RugbyEngine.Shared.Tablas;

namespace RugbyEngine.Client.Services.Entrenamientos
{
    public interface IJugadorService
    {
        Task<List<JugadorResponse>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<TableResponse<JugadorResponse>?> GetTableAsync(PaginacionDto paginacion, string? searchText, CancellationToken cancellationToken = default);
        Task<int> CountAsync(string? searchText, CancellationToken cancellationToken = default);
        Task<JugadorResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<JugadorResponse?> CreateAsync(JugadorRequest request, CancellationToken cancellationToken = default);
        Task<JugadorResponse?> UpdateAsync(int id, JugadorRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
