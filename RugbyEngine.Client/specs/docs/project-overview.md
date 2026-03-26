# RugbyEngine Client - Project Overview

## Estado actual
- Componente reusable `<Tabla>` en producción para `Personas` y `Usuarios`.
- Paginación/filtrado server-side consumidos por cliente con endpoints `search` y `count`.
- Formato de contador compacto en tabla: `x / x2` (acumulado hasta la página actual).
- Servicios cliente consumen API directamente con `HttpClient` (`PersonaService`, `UsuarioService`, `MainMenuService`).
- `PerfilService` y `AuthService` usan `CreateAuthorizedRequestAsync` / `BuildApiUri` con token Bearer manual.
- Manejo de cancelación en cliente con `CancellationToken` y descarte de resultados stale.
- UI principal con Bootstrap 5 + CSS propio.
- Menú dinámico desde API con caché en SessionStorage y resolución de roles por ruta.
- Sesión monitoreada por `SessionMonitor` con refresco vía `ValidateSessionAsync`.

## 1. Overview
- Tecnología: Blazor WebAssembly standalone
- Target Framework: .NET 10
- UI: Bootstrap 5.3 + CSS propio
- Autenticación: JWT en cookie (`re_access_token`)
- Menú: dinámico desde API con caché en SessionStorage

## 2. Estructura relevante
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
│   └── images/loadingPelota.ico
├── Program.cs
└── App.razor
```

## 3. Comunicación API (actual)
- `Program.cs` registra `HttpClient` con `BaseAddress` hardcodeada: producción `https://rugbyengine.agigena.com`, desarrollo `https://localhost:7083`.
- Servicios usan `HttpClient` directo (`GetFromJsonAsync`, `PostAsJsonAsync`, `PutAsJsonAsync`, `DeleteAsync`).
- `PerfilService` y `AuthService` usan `CreateAuthorizedRequestAsync` / `BuildApiUri` con token Bearer manual.

Endpoints de tabla:
- Personas: `GET api/persona/search?search=...&page=...&pageSize=...`
- Usuarios: `GET api/usuario/search?search=...&page=...&pageSize=...`
- Count: `GET api/persona/count`, `GET api/usuario/count`

Endpoints de autenticación:
- `POST api/Auth/Login`, `POST api/Auth/LogOut`, `POST api/Auth/Refresh`

Endpoints de menú:
- `GET api/Menu`, `GET api/Menu/RouteRoles`

Endpoints de perfiles:
- `GET api/Perfil`, `GET api/Perfil/mine`, `POST api/Perfil/{id}/assign`, `DELETE api/Perfil/{id}/unassign`

Endpoints de usuarios (gestión de perfiles):
- `POST api/usuario/{id}/perfiles/{perfilId}`, `DELETE api/usuario/{id}/perfiles/{perfilId}`

## 4. Tabla reusable
- `SearchFunc: Func<PaginacionDto, CancellationToken, Task<List<T>>>`
- `CountFunc: Func<CancellationToken, Task<int>>?`
- `Paginacion: bool` (default `true`)
- `PageSizes: IReadOnlyList<int>?` (default `[10, 20, 50]`)
- `EmptyText: string`
- `RefreshAsync` cancela request anterior y conserva UX con overlay.
- Contador `x / x2` calcula acumulado: `((pagina - 1) * pageSize) + registrosPagina`.

## 5. Notas de UX actuales
- Filtro con debounce en páginas administrativas.
- Loading overlay centrado en pantalla en tabla.
- Paginación visual integrada en `tfoot` con selector de filas, navegación `<`/`>` e indicador de página.
- Menú dinámico de 3 niveles con caché en SessionStorage.
- `SessionMonitor` verifica la sesión activa periódicamente.
