# RugbyEngine.Client

Documentación operativa del cliente Blazor WebAssembly de RugbyEngine.

Este documento describe **cómo está implementado y configurado** el proyecto `RugbyEngine.Client.csproj` y cómo se conecta con las specs funcionales del cliente.

---

## 1) Identidad del proyecto

- Proyecto: `RugbyEngine.Client`
- SDK: `Microsoft.NET.Sdk.BlazorWebAssembly`
- Target framework: `net10.0`
- Nullable: `enable`
- Implicit usings: `enable`
- Versión del ensamblado: `0.0.1`

Impacto:
- El cliente ejecuta completamente en navegador (WASM).
- El código se compila con referencias nulas estrictas y usings implícitos.

---

## 2) Configuración de compilación y calidad

En `Debug` y `Release`:
- `TreatWarningsAsErrors=True`
- `RunAOTCompilation=False`

A nivel global:
- `EnforceCodeStyleInBuild=True`
- `DebugType=portable`
- `DebugSymbols=True`

Impacto:
- Cualquier warning rompe build (disciplina de calidad alta).
- Se prioriza estabilidad/debug por sobre AOT en el estado actual.
- El análisis de estilo forma parte del build, no solo del IDE.

---

## 3) Activos web y PWA

### 3.1 App settings en `wwwroot`
El `.csproj` elimina explícitamente:
- `wwwroot/appsettings.json`
- `wwwroot/appsettings.Production.json`

Impacto:
- La configuración no se basa en esos archivos empaquetados.
- El cliente depende de configuración leída desde otras fuentes (por ejemplo `IConfiguration` + variables de entorno disponibles en runtime de hosting).

### 3.2 Service Worker
Registrado en el proyecto:
- `wwwroot/service-worker.js`
- `PublishedContent="wwwroot/service-worker.published.js"`

Impacto:
- El cliente está preparado para flujo PWA/publicación con service worker.

### 3.3 Ícono de aplicación
- `ApplicationIcon=wwwroot\images\loadingPelota.ico`

---

## 4) Paquetes NuGet usados

### Runtime/UI Blazor
- `Microsoft.AspNetCore.Components.WebAssembly (10.0.5)`
- `Microsoft.AspNetCore.Components.Authorization (10.0.5)`

### Desarrollo local
- `Microsoft.AspNetCore.Components.WebAssembly.DevServer (10.0.5)` con `PrivateAssets=all`

### Seguridad/Auth
- `System.IdentityModel.Tokens.Jwt (8.16.0)` — lectura y validación de tokens JWT en cliente.

### Almacenamiento en sesión
- `Blazor.Storage (5.0.0)` — provee `Blazored.SessionStorage` como dependencia transitiva.
  - Uso principal: `MainMenuService` cachea el árbol de menú y mapa de roles de rutas en `SessionStorage` del navegador para evitar requests repetidos durante la sesión.

### Calidad estática
- `SonarAnalyzer.CSharp (10.21.0.135717)` con assets privados (no se propaga a consumidores)

Impacto:
- El análisis Sonar participa en el pipeline local/CI y, combinado con warnings-as-errors, endurece reglas de calidad.

---

## 5) Estructura declarada en el .csproj y estructura real

- `ProjectReference` a `RugbyEngine.Shared` (contratos DTO/objetos compartidos).
- Carpetas declaradas explícitamente en el `.csproj`:
  - `Middleware\`
  - `Pages\Extra\`

### Estructura real del proyecto

```
RugbyEngine.Client/
├── Components/Tabla/
│   ├── Tabla.razor / Tabla.razor.css
│   ├── Header.razor
│   ├── Columna.razor
│   ├── Rows.razor
│   ├── Row.razor
│   ├── TablaHeader.razor
│   ├── TablaRows.razor
│   └── ITablaRegistration.cs
├── Handlers/
│   └── OperationCanceledExceptionDelegatingHandler.cs
├── Layout/
│   ├── MainLayout.razor / MainLayout.razor.css
│   ├── LoginLayout.razor
│   ├── NavMenu.razor / NavMenu.razor.css
│   ├── MobileNavMenu.razor
│   ├── MobileUserDrawer.razor
│   ├── AccountDrawer.razor
│   └── MenuRouteGuard.razor
├── Pages/
│   ├── Auth/
│   │   ├── Login.razor
│   │   ├── ResetPassword.razor
│   │   ├── NotAuth.razor
│   │   └── Component.razor
│   ├── Home/
│   │   └── Home.razor
│   ├── Administracion/Gestion/
│   │   ├── Personas.razor / Personas.razor.css
│   │   └── Usuarios.razor
│   ├── Account/
│   │   └── Profile.razor
│   ├── Tesoreria/3T/
│   │   └── Pagos.razor
│   ├── Utility/
│   │   ├── Loading.razor / Loading.razor.css
│   │   ├── MenuLateral.razor
│   │   └── SessionMonitor.razor
│   └── NotFound.razor
├── Services/
│   ├── IAuthService.cs / AuthService.cs
│   ├── ICookieService.cs / CookieService.cs
│   ├── IPersonaService.cs / PersonaService.cs
│   ├── IUsuarioService.cs / UsuarioService.cs
│   ├── IPerfilService.cs / PerfilService.cs
│   ├── MainMenuService.cs
│   └── ApiAuthenticationStateProvider.cs
├── wwwroot/
│   ├── css/app.css
│   ├── js/cookieInterop.js
│   ├── images/loadingPelota.ico
│   ├── service-worker.js
│   └── service-worker.published.js
├── Program.cs
└── App.razor
```

Nota:
- `PerfilService` todavía usa `CreateAuthorizedRequestAsync` (patrón manual con `HttpRequestMessage` + header Bearer).
- El resto de los servicios (`PersonaService`, `UsuarioService`) consumen la API directamente con `HttpClient`.

---

## 6) Relación entre .csproj y comportamiento funcional actual

### 6.1 Comunicación API
- El cliente consume endpoints por `HttpClient` directo en la mayoría de servicios.
- Excepción: `PerfilService` y `AuthService` usan `CreateAuthorizedRequestAsync` / `BuildApiUri` con `HttpRequestMessage` manual.
- Contratos actuales de tabla:
  - `GET api/persona/search?search=&page=&pageSize=`
  - `GET api/persona/count?search=`
  - `GET api/usuario/search?search=&page=&pageSize=`
  - `GET api/usuario/count?search=`
- Endpoints de autenticación:
  - `POST api/Auth/Login`
  - `POST api/Auth/LogOut`
  - `POST api/Auth/Refresh`
- Endpoints de menú:
  - `GET api/Menu`
  - `GET api/Menu/RouteRoles`
- Endpoints de perfiles:
  - `GET api/Perfil`
  - `GET api/Perfil/mine`
  - `POST api/Perfil/{id}/assign`
  - `DELETE api/Perfil/{id}/unassign`
- Endpoints de usuarios (CRUD + perfiles):
  - `POST api/usuario/{id}/perfiles/{perfilId}`
  - `DELETE api/usuario/{id}/perfiles/{perfilId}`

Referencia: `specs/feat/api-communication.md`

### 6.2 Tabla reusable y paginación
- Componente reusable `<Tabla>` con cancelación por `CancellationToken`.
- Contador compacto acumulado `x / x2`.
- Loading overlay durante refresh.

Referencia: `specs/feat/reusable-table-and-server-pagination.md`

### 6.3 Autenticación JWT y rutas protegidas
- Token en cookie `re_access_token`.
- Estado de autenticación sincronizado con `AuthenticationStateProvider`.
- Rutas protegidas con redirección a login y `returnUrl`.
- Validación/refresco de sesión con `ValidateSessionAsync` (`POST api/Auth/Refresh`).
- `SessionMonitor` componente que monitorea la sesión activa.

Referencias:
- `specs/feat/jwt-authentication.md`
- `specs/feat/cookie-token-storage.md`
- `specs/feat/route-protection.md`

### 6.4 Layout/UI
- Shell principal Bootstrap + CSS propio (sin MudBlazor en shell).
- `MainLayout` con topbar fija, drawers móviles y menú lateral.
- `LoginLayout` para páginas de autenticación (login, reset password).
- `AccountDrawer` para acciones de cuenta de usuario.
- `MenuRouteGuard` para protección de rutas por rol (usa `MainMenuService`).

Referencia: `specs/feat/layout.md`

### 6.5 Menú dinámico y roles de ruta
- `MainMenuService` carga el árbol de menú y mapa de roles desde la API (`api/Menu`, `api/Menu/RouteRoles`).
- Caché en `SessionStorage` del navegador para evitar requests repetidos.
- Resolución de menú activo por nivel (0=top, 1=desplegable, 2=lateral) según la ruta actual.
- `GetRequiredRoleForRoute` devuelve el rol necesario para acceder a una ruta.

### 6.6 Perfiles
- `PerfilService` consume `api/Perfil` para listados y asignación/desasignación de perfiles.
- Usa `CreateAuthorizedRequestAsync` con token Bearer manual.

### 6.7 Páginas adicionales
- `Pages/Auth/ResetPassword.razor` — reseteo de contraseña.
- `Pages/Account/Profile.razor` — perfil de usuario.
- `Pages/Tesoreria/3T/Pagos.razor` — página de pagos (tesorería).
- `Pages/Home/Home.razor` — página principal.
- `Pages/Auth/NotAuth.razor` — página de no autorizado.

Referencia: `specs/feat/login-page.md`

---

## 7) Resumen técnico del estado actual

`RugbyEngine.Client.csproj` define un cliente WASM .NET 10 con:
- reglas estrictas de calidad (warnings-as-errors + Sonar + code style en build),
- soporte PWA (service worker),
- autenticación JWT en cookie con refresco de sesión,
- consumo API directo por HttpClient (excepto `PerfilService` y `AuthService` que usan `CreateAuthorizedRequestAsync`),
- UI administrativa con tabla reusable y paginación server-side,
- menú dinámico desde API con caché en SessionStorage y protección de rutas por rol,
- gestión de perfiles de usuario,
- páginas de tesorería (pagos), perfil de cuenta y reseteo de contraseña.

Este documento debe mantenerse alineado con el código y con las specs vinculadas arriba.
