# API Communication Configuration

<purpose>
Configuración del HttpClient para comunicarse con la API backend, permitiendo especificar la URL base mediante configuración (appsettings.json o variables de entorno) y soportando desarrollo local con diferentes puertos para cliente y API.
</purpose>

<requirements>
- La URL base de la API se lee de `IConfiguration` con keys `API_BASE_URL` o `Api:BaseUrl`
- El `HttpClient` registrado en DI usa la URL configurada como `BaseAddress`
- Fallback al host del cliente Blazor (`builder.HostEnvironment.BaseAddress`) si no hay configuración
- `AuthService` construye URIs absolutas para llamadas de autenticación usando `BuildApiUri()`
- Soporte para desarrollo local donde API y cliente corren en diferentes puertos
</requirements>

<implementation>
Configuración del HttpClient en Program.cs:
```csharp
builder.Services.AddScoped(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var apiBaseUrl = configuration["API_BASE_URL"] ?? configuration["Api:BaseUrl"];
    var baseAddress = !string.IsNullOrWhiteSpace(apiBaseUrl)
        ? new Uri(apiBaseUrl, UriKind.Absolute)
        : new Uri(builder.HostEnvironment.BaseAddress);

    return new HttpClient { BaseAddress = baseAddress };
});
```

Construcción de URIs en AuthService:
- `_apiBaseUri` se inicializa desde configuración o HttpClient.BaseAddress
- `NormalizeBaseUri()` asegura que la URL termine en `/` y sea válida
- `BuildApiUri()` combina base URI con path relativo (ej: `api/Auth/Login`)

Archivos de configuración:
- `wwwroot/appsettings.json`:
  ```json
  {
    "API_BASE_URL": "https://localhost:7083/"
  }
  ```
- `Properties/launchSettings.json`:
  ```json
  "environmentVariables": {
    "ASPNETCORE_ENVIRONMENT": "Development",
    "API_BASE_URL": "https://localhost:7083"
  }
  ```

Endpoints de la API:
- Login: `POST {baseUrl}api/Auth/Login`
- Logout: `POST {baseUrl}api/Auth/LogOut`

Key files:
- `Program.cs` - Factory de HttpClient con IConfiguration
- `Services/AuthService.cs` - `BuildApiUri()`, `NormalizeBaseUri()`
- `wwwroot/appsettings.json` - Configuración de desarrollo
- `Properties/launchSettings.json` - Variables de entorno de desarrollo
</implementation>

<testing>
Verification plan:
- Con `API_BASE_URL` configurado: Requests van a `https://localhost:7083/api/...`
- Sin configuración: Requests van al mismo host del cliente Blazor
- `appsettings.json` en wwwroot es leído correctamente por Blazor WASM
- Variable de entorno en launchSettings sobreescribe appsettings
- Login request usa URI absoluta construida por `BuildApiUri()`
- Logout request usa URI absoluta con Authorization header
- CORS: API permite requests desde el origen del cliente
</testing>
