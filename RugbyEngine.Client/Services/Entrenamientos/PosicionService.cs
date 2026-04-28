using RugbyEngine.Shared.Entrenamientos;
using RugbyEngine.Shared.Tablas;
using System.Net.Http.Json;

namespace RugbyEngine.Client.Services.Entrenamientos
{
    public class PosicionService : IPosicionService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PosicionService> _logger;

        public PosicionService(HttpClient httpClient, ILogger<PosicionService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<PosicionResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<PosicionResponse>>("api/posicion", cancellationToken) ?? [];
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener posiciones");
                return [];
            }
        }

        public async Task<TableResponse<PosicionResponse>?> GetTableAsync(PaginacionDto paginacion, string? searchText, CancellationToken cancellationToken = default)
        {
            try
            {
                var pagina = paginacion.Pagina < 1 ? 1 : paginacion.Pagina;
                var pageSize = paginacion.RegistrosPorPagina < 1 ? 10 : paginacion.RegistrosPorPagina;
                var encodedSearch = Uri.EscapeDataString(searchText ?? string.Empty);

                cancellationToken.ThrowIfCancellationRequested();
                var registros = await _httpClient.GetFromJsonAsync<List<PosicionResponse>>(
                    $"api/posicion/search?search={encodedSearch}&page={pagina}&pageSize={pageSize}",
                    CancellationToken.None) ?? [];
                cancellationToken.ThrowIfCancellationRequested();

                return new TableResponse<PosicionResponse>
                {
                    Paginacion = new PaginacionDto { Pagina = pagina, RegistrosPorPagina = pageSize, Total = 0 },
                    Registros = registros
                };
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener tabla de posiciones");
                return null;
            }
        }

        public async Task<int> CountAsync(string? searchText, CancellationToken cancellationToken = default)
        {
            try
            {
                var encodedSearch = Uri.EscapeDataString(searchText ?? string.Empty);
                cancellationToken.ThrowIfCancellationRequested();
                var total = await _httpClient.GetFromJsonAsync<int>($"api/posicion/count?search={encodedSearch}", CancellationToken.None);
                cancellationToken.ThrowIfCancellationRequested();
                return total;
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al contar posiciones");
                return 0;
            }
        }

        public async Task<PosicionResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<PosicionResponse>($"api/posicion/{id}", cancellationToken);
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener posición {Id}", id);
                return null;
            }
        }

        public async Task<PosicionResponse?> CreateAsync(PosicionRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/posicion", request, cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<PosicionResponse>(cancellationToken);
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear posición");
                return null;
            }
        }

        public async Task<PosicionResponse?> UpdateAsync(int id, PosicionRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/posicion/{id}", request, cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<PosicionResponse>(cancellationToken);
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar posición {Id}", id);
                return null;
            }
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/posicion/{id}", cancellationToken);
                return response.IsSuccessStatusCode;
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar posición {Id}", id);
                return false;
            }
        }
    }
}
