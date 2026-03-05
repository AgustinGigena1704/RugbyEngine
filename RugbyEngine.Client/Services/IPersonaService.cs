using RugbyEngine.Shared.Personas;

namespace RugbyEngine.Client.Services
{
    public interface IPersonaService
    {
        Task<List<PersonaResponse>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<PersonaResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<PersonaResponse?> CreateAsync(PersonaRequest request, CancellationToken cancellationToken = default);
        Task<PersonaResponse?> UpdateAsync(int id, PersonaRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
