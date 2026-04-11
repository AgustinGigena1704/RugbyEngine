using RugbyEngine.Shared.Entrenamientos;
using RugbyEngine.Shared.Tablas;
using System.Net.Http.Json;

namespace RugbyEngine.Client.Services.Entrenamientos
{
    public class JugadorService : IJugadorService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<JugadorService> _logger;

        public JugadorService(HttpClient httpClient, ILogger<JugadorService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<JugadorResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<JugadorResponse>>("api/jugador", cancellationToken) ?? [];
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener jugadores");
                return [];
            }
        }

        public async Task<TableResponse<JugadorResponse>?> GetTableAsync(PaginacionDto paginacion, string? searchText, CancellationToken cancellationToken = default)
        {
            try
            {
                var pagina = paginacion.Pagina < 1 ? 1 : paginacion.Pagina;
                var pageSize = paginacion.RegistrosPorPagina < 1 ? 10 : paginacion.RegistrosPorPagina;
                var encodedSearch = Uri.EscapeDataString(searchText ?? string.Empty);

                cancellationToken.ThrowIfCancellationRequested();
                var registros = await _httpClient.GetFromJsonAsync<List<JugadorResponse>>(
                    $"api/jugador/search?search={encodedSearch}&page={pagina}&pageSize={pageSize}",
                    CancellationToken.None) ?? [];
                cancellationToken.ThrowIfCancellationRequested();

                return new TableResponse<JugadorResponse>
                {
                    Paginacion = new PaginacionDto { Pagina = pagina, RegistrosPorPagina = pageSize, Total = 0 },
                    Registros = registros
                };
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener tabla de jugadores");
                return null;
            }
        }

        public async Task<int> CountAsync(string? searchText, CancellationToken cancellationToken = default)
        {
            try
            {
                var encodedSearch = Uri.EscapeDataString(searchText ?? string.Empty);
                cancellationToken.ThrowIfCancellationRequested();
                var total = await _httpClient.GetFromJsonAsync<int>($"api/jugador/count?search={encodedSearch}", CancellationToken.None);
                cancellationToken.ThrowIfCancellationRequested();
                return total;
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al contar jugadores");
                return 0;
            }
        }

        public async Task<JugadorResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<JugadorResponse>($"api/jugador/{id}", cancellationToken);
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener jugador {Id}", id);
                return null;
            }
        }

        public async Task<JugadorResponse?> CreateAsync(JugadorRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/jugador", request, cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<JugadorResponse>(cancellationToken);
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear jugador");
                return null;
            }
        }

        public async Task<JugadorResponse?> UpdateAsync(int id, JugadorRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/jugador/{id}", request, cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<JugadorResponse>(cancellationToken);
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar jugador {Id}", id);
                return null;
            }
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/jugador/{id}", cancellationToken);
                return response.IsSuccessStatusCode;
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar jugador {Id}", id);
                return false;
            }
        }
    }
}
