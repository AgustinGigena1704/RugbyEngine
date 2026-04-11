using RugbyEngine.Shared.Entrenamientos;
using RugbyEngine.Shared.Tablas;

namespace RugbyEngine.Client.Services.Entrenamientos
{
    public interface IPosicionService
    {
        Task<List<PosicionResponse>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<TableResponse<PosicionResponse>?> GetTableAsync(PaginacionDto paginacion, string? searchText, CancellationToken cancellationToken = default);
        Task<int> CountAsync(string? searchText, CancellationToken cancellationToken = default);
        Task<PosicionResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<PosicionResponse?> CreateAsync(PosicionRequest request, CancellationToken cancellationToken = default);
        Task<PosicionResponse?> UpdateAsync(int id, PosicionRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
