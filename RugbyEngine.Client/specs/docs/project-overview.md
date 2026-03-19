# RugbyEngine Client - Project Overview

## Estado actual
- Componente reusable `<Tabla>` en producción para `Personas` y `Usuarios`.
- Paginación/filtrado server-side consumidos por cliente con endpoints `search` y `count`.
- Formato de contador compacto en tabla: `x / x2` (acumulado hasta la página actual).
- Servicios cliente consumen API directamente con `HttpClient` (sin `CreateAuthorizedRequestAsync`).
- Manejo de cancelación en cliente con `CancellationToken` y descarte de resultados stale.
- UI principal con Bootstrap 5 + CSS propio.

## 1. Overview
- Tecnología: Blazor WebAssembly standalone
- Target Framework: .NET 10
- UI: Bootstrap 5.3 + CSS propio
- Autenticación: JWT en cookie

## 2. Estructura relevante
```
RugbyEngine.Client/
├── Components/Tabla/
│   ├── Tabla.razor
│   ├── Tabla.razor.css
│   ├── Header.razor
│   ├── Columna.razor
│   ├── Rows.razor
│   ├── Row.razor
│   └── ITablaRegistration.cs
├── Pages/Administracion/Gestion/
│   ├── Personas.razor
│   └── Usuarios.razor
├── Services/
│   ├── PersonaService.cs
│   ├── UsuarioService.cs
│   ├── AuthService.cs
│   └── ...
├── Layout/
│   ├── MainLayout.razor
│   └── ...
├── wwwroot/css/app.css
├── Program.cs
└── App.razor
```

## 3. Comunicación API (actual)
- `Program.cs` registra `HttpClient` con `BaseAddress` desde `API_BASE_URL | Api:BaseUrl`.
- Servicios usan `HttpClient` directo (`GetFromJsonAsync`, `PostAsJsonAsync`, `PutAsJsonAsync`, `DeleteAsync`).
- Endpoints de tabla:
  - Personas: `GET api/persona/search?search=...&page=...&pageSize=...`
  - Usuarios: `GET api/usuario/search?search=...&page=...&pageSize=...`
  - Count: `GET api/persona/count`, `GET api/usuario/count`

## 4. Tabla reusable
- `SearchFunc: Func<PaginacionDto, CancellationToken, Task<List<T>>>`
- `CountFunc: Func<CancellationToken, Task<int>>?`
- `RefreshAsync` cancela request anterior y conserva UX con overlay.
- Contador `x / x2` calcula acumulado: `((pagina - 1) * pageSize) + registrosPagina`.

## 5. Notas de UX actuales
- Filtro con debounce en páginas administrativas.
- Loading overlay centrado en pantalla en tabla.
- Paginación visual integrada en `tfoot`.
