using RugbyEngine.Shared.Entrenamientos;

namespace RugbyEngine.Client.Services.Entrenamientos
{
    public interface IAsistenciaService
    {
        Task<List<AsistenciaItemDto>> GetByEntrenamientoAsync(int entrenamientoId, CancellationToken cancellationToken = default);
        Task<bool> GuardarAsistenciaAsync(int entrenamientoId, AsistenciaBulkRequest request, CancellationToken cancellationToken = default);
    }
}
