using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RugbyEngine.Shared.Personas;
using RugbyEngine.Shared.Tablas;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace RugbyEngine.Client.Services
{
    public class PersonaService : IPersonaService
    {
        private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);
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
                var table = await GetTableAsync(new PaginacionDto { Pagina = 1, RegistrosPorPagina = 1000000 }, cancellationToken);
                return table?.Registros ?? [];
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
                var request = await CreateAuthorizedRequestAsync(HttpMethod.Get, $"api/persona/{id}");
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
                var httpRequest = await CreateAuthorizedRequestAsync(HttpMethod.Post, "api/persona");
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
                var httpRequest = await CreateAuthorizedRequestAsync(HttpMethod.Put, $"api/persona/{id}");
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
                var request = await CreateAuthorizedRequestAsync(HttpMethod.Delete, $"api/persona/{id}");
                var response = await _httpClient.SendAsync(request, cancellationToken);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar persona {Id}", id);
                return false;
            }
        }

        public async Task<TableResponse<PersonaResponse>?> GetTableAsync(PaginacionDto paginacion, CancellationToken cancellationToken = default)
        {
            try
            {
                var request = await CreateAuthorizedRequestAsync(HttpMethod.Post, "api/persona/table");
                request.Content = JsonContent.Create(paginacion);
                var response = await _httpClient.SendAsync(request, cancellationToken);

                if (response.StatusCode == System.Net.HttpStatusCode.MethodNotAllowed)
                {
                    var fallbackRequest = await CreateAuthorizedRequestAsync(
                        HttpMethod.Get,
                        $"api/persona?Pagina={paginacion.Pagina}&RegistrosPorPagina={paginacion.RegistrosPorPagina}");
                    response = await _httpClient.SendAsync(fallbackRequest, cancellationToken);
                }

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                using var doc = JsonDocument.Parse(json);

                if (doc.RootElement.ValueKind == JsonValueKind.Array)
                {
                    var legacyList = JsonSerializer.Deserialize<List<PersonaResponse>>(json, _jsonOptions) ?? [];
                    var pagina = paginacion.Pagina < 1 ? 1 : paginacion.Pagina;
                    var registrosPorPagina = paginacion.RegistrosPorPagina < 1 ? 10 : paginacion.RegistrosPorPagina;
                    var total = legacyList.Count;
                    var registros = legacyList
                        .Skip((pagina - 1) * registrosPorPagina)
                        .Take(registrosPorPagina)
                        .ToList();

                    return new TableResponse<PersonaResponse>
                    {
                        Paginacion = new PaginacionDto
                        {
                            Pagina = pagina,
                            RegistrosPorPagina = registrosPorPagina,
                            Total = total
                        },
                        Registros = registros
                    };
                }

                if (doc.RootElement.ValueKind == JsonValueKind.Object)
                {
                    var table = JsonSerializer.Deserialize<TableResponse<PersonaResponse>>(json, _jsonOptions);
                    if (table is not null)
                        return table;
                }

                _logger.LogError("Formato de respuesta no soportado al obtener tabla paginada de personas");
                return null;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Respuesta inválida al obtener tabla paginada de personas");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener tabla paginada de personas");
                return null;
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
