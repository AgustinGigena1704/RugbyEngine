using RugbyEngine.Shared.Personas;
using RugbyEngine.Shared.Tablas;

namespace RugbyEngine.Client.Services
{
    public interface IPersonaService
    {
        Task<List<PersonaResponse>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<List<PersonaResponse>> SearchDisponiblesAsync(string? search, int pageSize = 10, int? incluirPersonaId = null, CancellationToken cancellationToken = default);
        Task<TableResponse<PersonaResponse>?> GetTableAsync(PaginacionDto paginacion, string? searchText, CancellationToken cancellationToken = default);
        Task<int> CountAsync(string? searchText, CancellationToken cancellationToken = default);
        Task<PersonaResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<PersonaResponse?> CreateAsync(PersonaRequest request, CancellationToken cancellationToken = default);
        Task<PersonaResponse?> UpdateAsync(int id, PersonaRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
