using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RugbyEngine.Shared.Personas;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace RugbyEngine.Client.Services
{
    public class PersonaService : IPersonaService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;
        private readonly ILogger<PersonaService> _logger;
        private readonly Uri? _apiBaseUri;

        public PersonaService(HttpClient httpClient, IAuthService authService, ILogger<PersonaService> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _authService = authService;
            _logger = logger;
            var configuredBaseUrl = configuration["API_BASE_URL"] ?? configuration["Api:BaseUrl"];
            _apiBaseUri = NormalizeBaseUri(configuredBaseUrl) ?? _httpClient.BaseAddress;
        }

        public async Task<List<PersonaResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var request = await CreateAuthorizedRequestAsync(HttpMethod.Get, "api/Persona");
                var response = await _httpClient.SendAsync(request, cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<PersonaResponse>>(cancellationToken) ?? [];
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener personas");
                return [];
            }
        }

        public async Task<PersonaResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var request = await CreateAuthorizedRequestAsync(HttpMethod.Get, $"api/Persona/{id}");
                var response = await _httpClient.SendAsync(request, cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<PersonaResponse>(cancellationToken);
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
                var httpRequest = await CreateAuthorizedRequestAsync(HttpMethod.Post, "api/Persona");
                httpRequest.Content = JsonContent.Create(request);
                var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<PersonaResponse>(cancellationToken);
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
                var httpRequest = await CreateAuthorizedRequestAsync(HttpMethod.Put, $"api/Persona/{id}");
                httpRequest.Content = JsonContent.Create(request);
                var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<PersonaResponse>(cancellationToken);
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
                var request = await CreateAuthorizedRequestAsync(HttpMethod.Delete, $"api/Persona/{id}");
                var response = await _httpClient.SendAsync(request, cancellationToken);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar persona {Id}", id);
                return false;
            }
        }

        private async Task<HttpRequestMessage> CreateAuthorizedRequestAsync(HttpMethod method, string relativePath)
        {
            var uri = BuildApiUri(relativePath);
            var httpRequest = new HttpRequestMessage(method, uri);
            var token = await _authService.GetTokenAsync();
            if (!string.IsNullOrWhiteSpace(token))
            {
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
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
