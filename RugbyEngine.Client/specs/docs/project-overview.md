# RugbyEngine Client - Project Overview

<purpose>
Documentar la arquitectura general, estructura de carpetas, dependencias y flujos principales del proyecto cliente Blazor WebAssembly para facilitar el onboarding de desarrolladores y mantenimiento del código.
</purpose>

<scope>
What will be documented:
- Arquitectura general del cliente Blazor WebAssembly (.NET 10)
- Estructura de carpetas y convenciones de nombrado
- Dependencias NuGet y su propósito
- Configuración de desarrollo y producción
- Flujo completo de autenticación JWT
- Sistema de layout con MudBlazor
- Protección de rutas y autorización
</scope>

<content_outline>
## 1. Overview
- **Tecnología**: Blazor WebAssembly standalone
- **Target Framework**: .NET 10
- **UI Framework**: MudBlazor 9.x
- **Autenticación**: JWT con cookies

## 2. Project Structure
```
RugbyEngine.Client/
├── Layout/
│   ├── MainLayout.razor      # Layout principal con MudBlazor
│   └── NavMenu.razor         # Navegación superior
├── Models/
│   └── Auth/
│       ├── LoginRequest.cs   # DTO de login
│       └── LoginResponse.cs  # DTO de respuesta
├── Pages/
│   ├── Auth/
│   │   └── Login.razor       # Página de login
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
│   └── index.html            # Host HTML
├── App.razor                 # Router y auth wrapper
├── Program.cs                # Configuración de servicios
└── _Imports.razor            # Usings globales
```

## 3. Dependencies (NuGet)
| Package | Version | Purpose |
|---------|---------|---------|
| Microsoft.AspNetCore.Components.WebAssembly | 10.0.x | Runtime Blazor WASM |
| Microsoft.AspNetCore.Components.Authorization | 10.0.x | Auth components |
| MudBlazor | 9.0.x | UI components |
| System.IdentityModel.Tokens.Jwt | 7.5.x | JWT parsing |

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

Audience: Desarrolladores nuevos al proyecto
Tone: Técnico, conciso, con ejemplos de código donde aplique
</content_outline>

<completion_criteria>
How to verify documentation is complete:
- [ ] Un desarrollador nuevo puede levantar el proyecto en < 15 minutos
- [ ] La estructura de carpetas está documentada con propósito de cada una
- [ ] Todas las dependencias listadas con versión y propósito
- [ ] El flujo de autenticación está explicado paso a paso
- [ ] Configuración de desarrollo y producción diferenciada
- [ ] Diagramas de arquitectura incluidos donde sea útil
- [ ] Links a specs de features para detalles de implementación
</completion_criteria>
