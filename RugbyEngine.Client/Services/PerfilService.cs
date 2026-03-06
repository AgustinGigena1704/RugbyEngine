using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RugbyEngine.Shared.Perfiles;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace RugbyEngine.Client.Services
{
    public class PerfilService : IPerfilService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;
        private readonly ILogger<PerfilService> _logger;
        private readonly Uri? _apiBaseUri;

        public PerfilService(HttpClient httpClient, IAuthService authService, ILogger<PerfilService> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _authService = authService;
            _logger = logger;
            var configuredBaseUrl = configuration["API_BASE_URL"] ?? configuration["Api:BaseUrl"];
            _apiBaseUri = NormalizeBaseUri(configuredBaseUrl) ?? _httpClient.BaseAddress;
        }

        public async Task<List<PerfilDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var request = await CreateAuthorizedRequestAsync(HttpMethod.Get, "api/Perfil");
                var response = await _httpClient.SendAsync(request, cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<PerfilDTO>>(cancellationToken) ?? [];
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener perfiles");
                return [];
            }
        }

        public async Task<List<PerfilDTO>> GetMineAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var request = await CreateAuthorizedRequestAsync(HttpMethod.Get, "api/Perfil/mine");
                var response = await _httpClient.SendAsync(request, cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<PerfilDTO>>(cancellationToken) ?? [];
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener mis perfiles");
                return [];
            }
        }

        public async Task<bool> AssignAsync(int perfilId, CancellationToken cancellationToken = default)
        {
            try
            {
                var request = await CreateAuthorizedRequestAsync(HttpMethod.Post, $"api/Perfil/{perfilId}/assign");
                var response = await _httpClient.SendAsync(request, cancellationToken);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asignar perfil {PerfilId}", perfilId);
                return false;
            }
        }

        public async Task<bool> UnassignAsync(int perfilId, CancellationToken cancellationToken = default)
        {
            try
            {
                var request = await CreateAuthorizedRequestAsync(HttpMethod.Delete, $"api/Perfil/{perfilId}/unassign");
                var response = await _httpClient.SendAsync(request, cancellationToken);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al retirar perfil {PerfilId}", perfilId);
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
            if (string.IsNullOrWhiteSpace(baseUrl)) return null;
            var normalized = baseUrl.EndsWith('/') ? baseUrl : baseUrl + '/';
            return Uri.TryCreate(normalized, UriKind.Absolute, out var uri) ? uri : null;
        }
    }
}
