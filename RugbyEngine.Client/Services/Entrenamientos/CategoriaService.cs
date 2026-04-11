using RugbyEngine.Shared.Entrenamientos;
using RugbyEngine.Shared.Tablas;
using System.Net.Http.Json;

namespace RugbyEngine.Client.Services.Entrenamientos
{
    public class CategoriaService : ICategoriaService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CategoriaService> _logger;

        public CategoriaService(HttpClient httpClient, ILogger<CategoriaService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<CategoriaResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<CategoriaResponse>>("api/categoria", cancellationToken) ?? [];
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener categorías");
                return [];
            }
        }

        public async Task<TableResponse<CategoriaResponse>?> GetTableAsync(PaginacionDto paginacion, string? searchText, CancellationToken cancellationToken = default)
        {
            try
            {
                var pagina = paginacion.Pagina < 1 ? 1 : paginacion.Pagina;
                var pageSize = paginacion.RegistrosPorPagina < 1 ? 10 : paginacion.RegistrosPorPagina;
                var encodedSearch = Uri.EscapeDataString(searchText ?? string.Empty);

                cancellationToken.ThrowIfCancellationRequested();
                var registros = await _httpClient.GetFromJsonAsync<List<CategoriaResponse>>(
                    $"api/categoria/search?search={encodedSearch}&page={pagina}&pageSize={pageSize}",
                    CancellationToken.None) ?? [];
                cancellationToken.ThrowIfCancellationRequested();

                return new TableResponse<CategoriaResponse>
                {
                    Paginacion = new PaginacionDto { Pagina = pagina, RegistrosPorPagina = pageSize, Total = 0 },
                    Registros = registros
                };
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener tabla de categorías");
                return null;
            }
        }

        public async Task<int> CountAsync(string? searchText, CancellationToken cancellationToken = default)
        {
            try
            {
                var encodedSearch = Uri.EscapeDataString(searchText ?? string.Empty);
                cancellationToken.ThrowIfCancellationRequested();
                var total = await _httpClient.GetFromJsonAsync<int>($"api/categoria/count?search={encodedSearch}", CancellationToken.None);
                cancellationToken.ThrowIfCancellationRequested();
                return total;
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al contar categorías");
                return 0;
            }
        }

        public async Task<CategoriaResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<CategoriaResponse>($"api/categoria/{id}", cancellationToken);
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener categoría {Id}", id);
                return null;
            }
        }

        public async Task<CategoriaResponse?> CreateAsync(CategoriaRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/categoria", request, cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<CategoriaResponse>(cancellationToken);
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear categoría");
                return null;
            }
        }

        public async Task<CategoriaResponse?> UpdateAsync(int id, CategoriaRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/categoria/{id}", request, cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<CategoriaResponse>(cancellationToken);
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar categoría {Id}", id);
                return null;
            }
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/categoria/{id}", cancellationToken);
                return response.IsSuccessStatusCode;
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar categoría {Id}", id);
                return false;
            }
        }
    }
}
