using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RugbyEngine.Shared.Tablas;
using RugbyEngine.Shared.Usuarios;
using System.Net.Http.Json;

namespace RugbyEngine.Client.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UsuarioService> _logger;

        public UsuarioService(HttpClient httpClient, IAuthService authService, ILogger<UsuarioService> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<UsuarioResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<UsuarioResponse>>("api/usuario", cancellationToken) ?? [];
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios");
                return [];
            }
        }

        public async Task<TableResponse<UsuarioResponse>?> GetTableAsync(PaginacionDto paginacion, string? searchText, CancellationToken cancellationToken = default)
        {
            try
            {
                var pagina = paginacion.Pagina < 1 ? 1 : paginacion.Pagina;
                var pageSize = paginacion.RegistrosPorPagina < 1 ? 10 : paginacion.RegistrosPorPagina;
                var encodedSearch = Uri.EscapeDataString(searchText ?? string.Empty);

                cancellationToken.ThrowIfCancellationRequested();
                var registros = await _httpClient.GetFromJsonAsync<List<UsuarioResponse>>(
                    $"api/usuario/search?search={encodedSearch}&page={pagina}&pageSize={pageSize}",
                    CancellationToken.None) ?? [];
                cancellationToken.ThrowIfCancellationRequested();

                return new TableResponse<UsuarioResponse>
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
                _logger.LogError(ex, "Error al obtener tabla paginada de usuarios");
                return null;
            }
        }

        public async Task<int> CountAsync(string? searchText, CancellationToken cancellationToken = default)
        {
            try
            {
                var encodedSearch = Uri.EscapeDataString(searchText ?? string.Empty);
                cancellationToken.ThrowIfCancellationRequested();
                var total = await _httpClient.GetFromJsonAsync<int>($"api/usuario/count?search={encodedSearch}", CancellationToken.None);
                cancellationToken.ThrowIfCancellationRequested();
                return total;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al contar usuarios");
                return 0;
            }
        }

        public async Task<UsuarioResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<UsuarioResponse>($"api/usuario/{id}", cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario {Id}", id);
                return null;
            }
        }

        public async Task<UsuarioResponse?> CreateAsync(UsuarioCreateDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/usuario", dto, cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<UsuarioResponse>(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear usuario");
                return null;
            }
        }

        public async Task<UsuarioResponse?> UpdateAsync(int id, UsuarioUpdateDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/usuario/{id}", dto, cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<UsuarioResponse>(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar usuario {Id}", id);
                return null;
            }
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/usuario/{id}", cancellationToken);
                return response.IsSuccessStatusCode;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar usuario {Id}", id);
                return false;
            }
        }

        public async Task<bool> AssignPerfilAsync(int usuarioId, int perfilId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/usuario/{usuarioId}/perfiles/{perfilId}", null, cancellationToken);
                return response.IsSuccessStatusCode;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asignar perfil {PerfilId} a usuario {UsuarioId}", perfilId, usuarioId);
                return false;
            }
        }

        public async Task<bool> UnassignPerfilAsync(int usuarioId, int perfilId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/usuario/{usuarioId}/perfiles/{perfilId}", cancellationToken);
                return response.IsSuccessStatusCode;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al retirar perfil {PerfilId} de usuario {UsuarioId}", perfilId, usuarioId);
                return false;
            }
        }
    }
}
