using RugbyEngine.Shared.Entrenamientos;
using RugbyEngine.Shared.Tablas;

namespace RugbyEngine.Client.Services.Entrenamientos
{
    public interface ICategoriaService
    {
        Task<List<CategoriaResponse>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<TableResponse<CategoriaResponse>?> GetTableAsync(PaginacionDto paginacion, string? searchText, CancellationToken cancellationToken = default);
        Task<int> CountAsync(string? searchText, CancellationToken cancellationToken = default);
        Task<CategoriaResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<CategoriaResponse?> CreateAsync(CategoriaRequest request, CancellationToken cancellationToken = default);
        Task<CategoriaResponse?> UpdateAsync(int id, CategoriaRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
