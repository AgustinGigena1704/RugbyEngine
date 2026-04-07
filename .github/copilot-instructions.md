# RugbyEngine — Copilot Instructions

Solución .NET 10 full-stack para gestión de una institución de rugby. Ver [README.md](../README.md) y [RugbyEngine.Api/README.md](../RugbyEngine.Api/README.md) para visión general.

## Build & Run

```bash
# Build toda la solución
dotnet build RugbyEngine.slnx

# Iniciar cliente (también hace build)
dotnet build RugbyEngine.slnx && dotnet run --no-build --project RugbyEngine.Client/RugbyEngine.Client.csproj

# Migraciones EF Core (desde la raíz del repo)
dotnet ef migrations add <Nombre> --project RugbyEngine.Api --startup-project RugbyEngine.Api
dotnet ef database update --project RugbyEngine.Api
```

- `TreatWarningsAsErrors = true` en los tres proyectos; corregir todos los warnings antes de asumir que compila.
- Variables de entorno para JWT: `JWT_ISSUER`, `JWT_AUDIENCE`, `JWT_SECRET_KEY`, `JWT_EXPIRATION_MINUTES`. No van en `appsettings.json`.

## Arquitectura

| Proyecto | Rol |
|---|---|
| `RugbyEngine.Api` | ASP.NET Core Web API, EF Core + PostgreSQL, JWT, sirve el WASM en producción |
| `RugbyEngine.Client` | Blazor WebAssembly, MudBlazor |
| `RugbyEngine.Shared` | DTOs y contratos compartidos |

Specs detalladas del cliente en `RugbyEngine.Client/specs/` y `RugbyEngine.Client/plans/`.

## Backend — Convenciones

### Controladores
- Heredan de `GenericController` (provee `GetCurrentUserId()` / `GetCurrentUserAsync()`).
- Rutas: `[Route("api/[controller]")]`, retornos `ActionResult<T>`, decorados con `[ProducesResponseType]`.
- **Los endpoints de listado/búsqueda devuelven `List<T>` puro (JSON), nunca `TableResponse` ni HTML.**
- Conteo paginado: endpoint separado `GET /api/<entidad>/count?search=X`.

### Repositorios y EntityManager
- Base genérica `GenericRepository<TEntity>`: soft-delete (`BorradoLogico`), audit (`IAudithory`), paginación `Skip/Take`.
- Búsqueda case-insensitive con `EF.Functions.ILike()` (PostgreSQL).
- Repos especializados registrados con `[Repository(typeof(Entidad))]` en la entidad; resueltos vía `EntityManager.GetRepository<TRepo>()`.
- Servicios registrados automáticamente con `[Service]` (scoped por defecto, interfaz inferida por convención `I + NombreClase`).

### Auditoría
- Entidades que implementan `IAudithory` tienen sus campos `CreatedBy/UpdatedBy/DeletedBy` + timestamps auto-poblados por el repositorio.

### Respuestas HTTP
- `200 OK` datos / `201 Created` / `400` validación / `401` auth / `404` no encontrado / `503` DB caída.

## Cliente — Convenciones

### Servicios HTTP
- **Siempre usar `HttpClient` directamente** (base address configurado en `Program.cs`); **no usar `CreateAuthorizedRequestAsync`**.
- El token JWT se gestiona automáticamente via `ApiAuthenticationStateProvider` (se inyecta en el header de `HttpClient`).
- Pasar siempre `CancellationToken`; re-lanzar `OperationCanceledException` separado de otras excepciones.
- En error, retornar colección vacía (`[]`), nunca `null`.

### Autenticación
- JWT persistido en cookie de browser (`re_access_token`) via JS interop.
- Auto-refresh silencioso: si el token expira en < 15 min, el middleware de API retorna uno nuevo en el header `jwt-session`; el cliente debe leerlo y actualizarlo.
- `[Authorize]` en páginas protegidas; `@layout LoginLayout` en páginas de auth.

### Componente `<Tabla>`
- Componente genérico en `Components/Tabla/` para tablas paginadas server-side.
- Requiere `SearchFunc: Func<PaginacionDto, CancellationToken, Task<List<T>>>` y opcionalmente `CountFunc`.
- **Tablas sin tamaños fijos; diseño responsive para mobile y desktop.**
- **Formato del contador de paginación: `x / x2` (compacto), tanto en mobile como en desktop.**
- Cancela requests anteriores antes de ejecutar el siguiente (patrón `CancellationTokenSource` con `CancelAsync()`).

### Páginas y Formularios
- Código en bloques `@code {}` dentro del `.razor` (no archivos `.cs` separados).
- Formularios: `<EditForm>` + `<DataAnnotationsValidator>`; validación con `[Required]`, `[MaxLength]`, etc.
- Modales inline condicionales (`@if (_dialogVisible)`), no componentes de diálogo genérico.
- Menú dinámico cargado desde `GET /api/Menu`; cacheado en `SessionStorage`.

### Estructura de páginas
- `Pages/<Dominio>/` — páginas funcionales agrupadas por módulo.
- `Layout/` — layouts, navmenú, drawers móviles, guardia de rutas.
- `Services/` — pares `IServicio` / `Servicio` para todas las llamadas HTTP.
