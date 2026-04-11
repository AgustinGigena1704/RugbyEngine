using RugbyEngine.Shared.Entrenamientos;
using System.Net.Http.Json;

namespace RugbyEngine.Client.Services.Entrenamientos
{
    public class AsistenciaService : IAsistenciaService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AsistenciaService> _logger;

        public AsistenciaService(HttpClient httpClient, ILogger<AsistenciaService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<AsistenciaItemDto>> GetByEntrenamientoAsync(int entrenamientoId, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<AsistenciaItemDto>>(
                    $"api/asistencia/entrenamiento/{entrenamientoId}", cancellationToken) ?? [];
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener asistencia del entrenamiento {Id}", entrenamientoId);
                return [];
            }
        }

        public async Task<bool> GuardarAsistenciaAsync(int entrenamientoId, AsistenciaBulkRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(
                    $"api/asistencia/entrenamiento/{entrenamientoId}", request, cancellationToken);
                return response.IsSuccessStatusCode;
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar asistencia del entrenamiento {Id}", entrenamientoId);
                return false;
            }
        }
    }
}
