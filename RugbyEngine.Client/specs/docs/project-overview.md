# RugbyEngine Client - Project Overview

## Cambios recientes
- Loader consistente y reutilizable en Blazor e index.html, con animación y tamaño adaptativo.
- Menús desplegables: <li> ocupa el 100% del ancho, bordes redondeados y fondo activo completo.
- Contraste visual mejorado en el fondo de la página de login.
- Sombra más notoria en el menú lateral (desktop y mobile).
- Layouts principales y navegación implementados con Bootstrap 5 y CSS propio, sin MudBlazor en la shell principal.

## 1. Overview
- **Tecnología**: Blazor WebAssembly standalone
- **Target Framework**: .NET 10
- **UI**: Bootstrap 5.3 (CDN) + CSS propio — sin MudBlazor en layouts principales
- **Autenticación**: JWT con cookies

## 2. Project Structure
```
RugbyEngine.Client/
├── Layout/
│   ├── MainLayout.razor      # Layout principal con Bootstrap y CSS propio
│   └── NavMenu.razor         # Navegación superior con dropdowns y <li> de ancho completo
├── Models/
│   └── Auth/
│       ├── LoginRequest.cs   # DTO de login
│       └── LoginResponse.cs  # DTO de respuesta
├── Pages/
│   ├── Auth/
│   │   └── Login.razor       # Página de login, fondo contrastado
│   ├── Extra/
│   │   ├── Counter.razor     # Demo contador
│   │   └── Weather.razor     # Demo weather
│   ├── Home/
│   │   └── Home.razor        # Página principal
│   └── NotFound.razor        # 404
├── Services/
│   ├── AuthService.cs        # Lógica de autenticación
│   ├── IAuthService.cs       # Interface
│   ├── ApiAuthenticationStateProvider.cs
│   ├── CookieService.cs      # JS Interop para cookies
│   └── ICookieService.cs     # Interface
├── skills/
│   └── spec-manager/         # Skill de gestión de specs
├── specs/
│   ├── feat/                 # Specs de features
│   └── docs/                 # Specs de documentación
├── wwwroot/
│   ├── js/
│   │   └── cookieInterop.js  # Funciones JS para cookies
│   ├── appsettings.json      # Config (API_BASE_URL)
│   └── index.html            # Host HTML, loader consistente
├── App.razor                 # Router y auth wrapper
├── Program.cs                # Configuración de servicios
└── _Imports.razor            # Usings globales
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

(El resto de la documentación se mantiene igual, solo se han resaltado los cambios visuales y estructurales recientes.)
