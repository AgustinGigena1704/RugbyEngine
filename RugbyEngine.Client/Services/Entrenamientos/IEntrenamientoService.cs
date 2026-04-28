using RugbyEngine.Shared.Entrenamientos;
using RugbyEngine.Shared.Tablas;

namespace RugbyEngine.Client.Services.Entrenamientos
{
    public interface IEntrenamientoService
    {
        Task<List<EntrenamientoResponse>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<TableResponse<EntrenamientoResponse>?> GetTableAsync(PaginacionDto paginacion, string? searchText, CancellationToken cancellationToken = default);
        Task<int> CountAsync(string? searchText, CancellationToken cancellationToken = default);
        Task<EntrenamientoResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<EntrenamientoResponse?> CreateAsync(EntrenamientoRequest request, CancellationToken cancellationToken = default);
        Task<EntrenamientoResponse?> UpdateAsync(int id, EntrenamientoRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
