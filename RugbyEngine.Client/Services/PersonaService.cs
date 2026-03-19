using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RugbyEngine.Shared.Personas;
using RugbyEngine.Shared.Tablas;
using System.Net.Http.Json;

namespace RugbyEngine.Client.Services
{
    public class PersonaService : IPersonaService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PersonaService> _logger;

        public PersonaService(HttpClient httpClient, IAuthService authService, ILogger<PersonaService> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<PersonaResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<PersonaResponse>>("api/persona", cancellationToken) ?? [];
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener personas");
                return [];
            }
        }

        public async Task<TableResponse<PersonaResponse>?> GetTableAsync(PaginacionDto paginacion, string? searchText, CancellationToken cancellationToken = default)
        {
            try
            {
                var pagina = paginacion.Pagina < 1 ? 1 : paginacion.Pagina;
                var pageSize = paginacion.RegistrosPorPagina < 1 ? 10 : paginacion.RegistrosPorPagina;
                var encodedSearch = Uri.EscapeDataString(searchText ?? string.Empty);

                cancellationToken.ThrowIfCancellationRequested();
                var registros = await _httpClient.GetFromJsonAsync<List<PersonaResponse>>(
                    $"api/persona/search?search={encodedSearch}&page={pagina}&pageSize={pageSize}",
                    CancellationToken.None) ?? [];
                cancellationToken.ThrowIfCancellationRequested();

                return new TableResponse<PersonaResponse>
                {
                    Paginacion = new PaginacionDto
                    {
                        Pagina = pagina,
                        RegistrosPorPagina = pageSize,
                        Total = 0
                    },
                    Registros = registros
                };
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener tabla paginada de personas");
                return null;
            }
        }

        public async Task<int> CountAsync(string? searchText, CancellationToken cancellationToken = default)
        {
            try
            {
                var encodedSearch = Uri.EscapeDataString(searchText ?? string.Empty);
                cancellationToken.ThrowIfCancellationRequested();
                var total = await _httpClient.GetFromJsonAsync<int>($"api/persona/count?search={encodedSearch}", CancellationToken.None);
                cancellationToken.ThrowIfCancellationRequested();
                return total;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al contar personas");
                return 0;
            }
        }

        public async Task<PersonaResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<PersonaResponse>($"api/persona/{id}", cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener persona {Id}", id);
                return null;
            }
        }

        public async Task<PersonaResponse?> CreateAsync(PersonaRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/persona", request, cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<PersonaResponse>(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear persona");
                return null;
            }
        }

        public async Task<PersonaResponse?> UpdateAsync(int id, PersonaRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/persona/{id}", request, cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<PersonaResponse>(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar persona {Id}", id);
                return null;
            }
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/persona/{id}", cancellationToken);
                return response.IsSuccessStatusCode;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar persona {Id}", id);
                return false;
            }
        }
    }
}
