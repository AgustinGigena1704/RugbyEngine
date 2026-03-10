using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RugbyEngine.Shared.Usuarios;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace RugbyEngine.Client.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;
        private readonly ILogger<UsuarioService> _logger;
        private readonly Uri? _apiBaseUri;

        public UsuarioService(HttpClient httpClient, IAuthService authService, ILogger<UsuarioService> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _authService = authService;
            _logger = logger;
            var configuredBaseUrl = configuration["API_BASE_URL"] ?? configuration["Api:BaseUrl"];
            _apiBaseUri = NormalizeBaseUri(configuredBaseUrl) ?? _httpClient.BaseAddress;
        }

        public async Task<List<UsuarioResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var request = await CreateAuthorizedRequestAsync(HttpMethod.Get, "api/Usuario");
                var response = await _httpClient.SendAsync(request, cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<UsuarioResponse>>(cancellationToken) ?? [];
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios");
                return [];
            }
        }

        public async Task<UsuarioResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var request = await CreateAuthorizedRequestAsync(HttpMethod.Get, $"api/Usuario/{id}");
                var response = await _httpClient.SendAsync(request, cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<UsuarioResponse>(cancellationToken);
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
                var httpRequest = await CreateAuthorizedRequestAsync(HttpMethod.Post, "api/Usuario");
                httpRequest.Content = JsonContent.Create(dto);
                var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<UsuarioResponse>(cancellationToken);
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
                var httpRequest = await CreateAuthorizedRequestAsync(HttpMethod.Put, $"api/Usuario/{id}");
                httpRequest.Content = JsonContent.Create(dto);
                var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<UsuarioResponse>(cancellationToken);
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
                var request = await CreateAuthorizedRequestAsync(HttpMethod.Delete, $"api/Usuario/{id}");
                var response = await _httpClient.SendAsync(request, cancellationToken);
                return response.IsSuccessStatusCode;
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
                var request = await CreateAuthorizedRequestAsync(HttpMethod.Post, $"api/Usuario/{usuarioId}/perfiles/{perfilId}");
                var response = await _httpClient.SendAsync(request, cancellationToken);
                return response.IsSuccessStatusCode;
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
                var request = await CreateAuthorizedRequestAsync(HttpMethod.Delete, $"api/Usuario/{usuarioId}/perfiles/{perfilId}");
                var response = await _httpClient.SendAsync(request, cancellationToken);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al retirar perfil {PerfilId} de usuario {UsuarioId}", perfilId, usuarioId);
                return false;
            }
        }

        private async Task<HttpRequestMessage> CreateAuthorizedRequestAsync(HttpMethod method, string relativePath)
        {
            var uri = BuildApiUri(relativePath);
            var httpRequest = new HttpRequestMessage(method, uri);
            var token = await _authService.GetTokenAsync();
            if (!string.IsNullOrWhiteSpace(token))
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return httpRequest;
        }

        private Uri BuildApiUri(string relativePath)
        {
            var trimmed = relativePath.TrimStart('/');
            return _apiBaseUri != null
                ? new Uri(_apiBaseUri, trimmed)
                : new Uri(trimmed, UriKind.Relative);
        }

        private static Uri? NormalizeBaseUri(string? baseUrl)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                return null;
            var normalized = baseUrl.EndsWith('/') ? baseUrl : baseUrl + '/';
            return Uri.TryCreate(normalized, UriKind.Absolute, out var uri) ? uri : null;
        }
    }
}
