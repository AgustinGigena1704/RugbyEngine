# RugbyEngine Client - Project Overview

## Cambios recientes
- Componente reusable `<Tabla>` con sintaxis declarativa (`Header/Columna`, `Rows/Row`) y paginación integrada en `tfoot`.
- Migración de `Personas` y `Usuarios` al componente `<Tabla>`.
- Paginación visual homogénea en tablas con contador compacto `x / x2`, selector de filas y navegación `<` `>`.
- `Usuarios`: filtros y paginación movidos a servidor (API + repositorio) con endpoints dedicados de búsqueda y conteo.
- Compatibilidad en cliente para escenarios de despliegue parcial (fallback controlado cuando endpoints nuevos no están disponibles).

## 1. Overview
- **Tecnología**: Blazor WebAssembly standalone
- **Target Framework**: .NET 10
- **UI**: Bootstrap 5.3 (CDN) + CSS propio
- **Autenticación**: JWT con cookies

## 2. Project Structure
```
RugbyEngine.Client/
├── Components/
│   └── Tabla/
│       ├── Tabla.razor          # Grilla reusable con paginación
│       ├── Tabla.razor.css      # Estilos scoped de tabla/paginación
│       ├── ITablaRegistration.cs
│       ├── Header.razor
│       ├── Columna.razor
│       ├── Rows.razor
│       └── Row.razor
├── Layout/
│   ├── MainLayout.razor
│   └── NavMenu.razor
├── Pages/
│   ├── Administracion/
│   │   └── Gestion/
│   │       ├── Personas.razor   # Usa <Tabla>
│   │       └── Usuarios.razor   # Usa <Tabla>
│   ├── Auth/
│   │   └── Login.razor
│   ├── Home/
│   │   └── Home.razor
│   └── NotFound.razor
├── Services/
│   ├── PersonaService.cs
│   ├── UsuarioService.cs
│   └── ...
├── specs/
│   ├── feat/
│   └── docs/
├── wwwroot/
│   ├── js/
│   ├── appsettings.json
│   └── index.html
├── App.razor
├── Program.cs
└── _Imports.razor
```

## 3. Dependencies (NuGet)
| Package | Version | Purpose |
|---------|---------|---------|
| Microsoft.AspNetCore.Components.WebAssembly | 10.0.x | Runtime Blazor WASM |
| Microsoft.AspNetCore.Components.Authorization | 10.0.x | Auth components |
| System.IdentityModel.Tokens.Jwt | 8.x | JWT parsing |
| SonarAnalyzer.CSharp | 10.x | Análisis estático |

## 4. Configuration
- `wwwroot/appsettings.json` - Runtime config
- `Properties/launchSettings.json` - Dev environment
- Keys: `API_BASE_URL`, `Api:BaseUrl`

## 5. Authentication Flow
1. User submits credentials → AuthService.LoginAsync()
2. POST to API → Receives JWT token
3. Token stored in cookie → CookieService.SetCookieAsync()
4. AuthState notified → ApiAuthenticationStateProvider
5. HttpClient header set → Authorization: Bearer {token}

## 6. Development Setup
1. Clone repository
2. Ensure API is running on configured port
3. `dotnet run` in RugbyEngine.Client folder
4. Navigate to https://localhost:7255
