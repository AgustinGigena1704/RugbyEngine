using RugbyEngine.Shared.Entrenamientos;
using RugbyEngine.Shared.Tablas;
using System.Net.Http.Json;

namespace RugbyEngine.Client.Services.Entrenamientos
{
    public class EntrenamientoService : IEntrenamientoService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<EntrenamientoService> _logger;

        public EntrenamientoService(HttpClient httpClient, ILogger<EntrenamientoService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<EntrenamientoResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<EntrenamientoResponse>>("api/entrenamiento", cancellationToken) ?? [];
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener entrenamientos");
                return [];
            }
        }

        public async Task<TableResponse<EntrenamientoResponse>?> GetTableAsync(PaginacionDto paginacion, string? searchText, CancellationToken cancellationToken = default)
        {
            try
            {
                var pagina = paginacion.Pagina < 1 ? 1 : paginacion.Pagina;
                var pageSize = paginacion.RegistrosPorPagina < 1 ? 10 : paginacion.RegistrosPorPagina;
                var encodedSearch = Uri.EscapeDataString(searchText ?? string.Empty);

                cancellationToken.ThrowIfCancellationRequested();
                var registros = await _httpClient.GetFromJsonAsync<List<EntrenamientoResponse>>(
                    $"api/entrenamiento/search?search={encodedSearch}&page={pagina}&pageSize={pageSize}",
                    CancellationToken.None) ?? [];
                cancellationToken.ThrowIfCancellationRequested();

                return new TableResponse<EntrenamientoResponse>
                {
                    Paginacion = new PaginacionDto { Pagina = pagina, RegistrosPorPagina = pageSize, Total = 0 },
                    Registros = registros
                };
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener tabla de entrenamientos");
                return null;
            }
        }

        public async Task<int> CountAsync(string? searchText, CancellationToken cancellationToken = default)
        {
            try
            {
                var encodedSearch = Uri.EscapeDataString(searchText ?? string.Empty);
                cancellationToken.ThrowIfCancellationRequested();
                var total = await _httpClient.GetFromJsonAsync<int>($"api/entrenamiento/count?search={encodedSearch}", CancellationToken.None);
                cancellationToken.ThrowIfCancellationRequested();
                return total;
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al contar entrenamientos");
                return 0;
            }
        }

        public async Task<EntrenamientoResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<EntrenamientoResponse>($"api/entrenamiento/{id}", cancellationToken);
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener entrenamiento {Id}", id);
                return null;
            }
        }

        public async Task<EntrenamientoResponse?> CreateAsync(EntrenamientoRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/entrenamiento", request, cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<EntrenamientoResponse>(cancellationToken);
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear entrenamiento");
                return null;
            }
        }

        public async Task<EntrenamientoResponse?> UpdateAsync(int id, EntrenamientoRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/entrenamiento/{id}", request, cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<EntrenamientoResponse>(cancellationToken);
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar entrenamiento {Id}", id);
                return null;
            }
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/entrenamiento/{id}", cancellationToken);
                return response.IsSuccessStatusCode;
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar entrenamiento {Id}", id);
                return false;
            }
        }
    }
}
