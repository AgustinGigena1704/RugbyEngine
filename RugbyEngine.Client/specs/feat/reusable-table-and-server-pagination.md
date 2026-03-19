# Reusable Table + Server-side Pagination (Personas y Usuarios)

<purpose>
Unificar el render y la paginación de listados administrativos con un componente reusable `<Tabla>`, manteniendo la estética existente y moviendo paginación/filtros de Usuarios al backend (repositorio/API).
</purpose>

<requirements>
- Componente reusable `<Tabla>` con `@typeparam T`.
- Definición declarativa:
  - `<Header>` + `<Columna Nombre="..." />`
  - `<Rows>` + `<Row T="..." Context="...">...</Row>`
- Parámetros del componente:
  - `SearchFunc: Func<PaginacionDto, Task<List<T>>>`
  - `CountFunc: Func<Task<int>>`
  - `Paginacion` (bool)
  - `PageSizes`
  - `EmptyText`
- Paginación dentro de la tabla (`tfoot`) con:
  - contador `x / x2`
  - selector de filas
  - navegación `<` y `>`
- `Personas` y `Usuarios` deben usar `<Tabla>`.
- En `Usuarios`, filtros y paginación se ejecutan en backend (repositorio).
</requirements>

<implementation>
### Cliente: componente Tabla
Archivos:
- `Components/Tabla/Tabla.razor`
- `Components/Tabla/Tabla.razor.css`
- `Components/Tabla/ITablaRegistration.cs`
- `Components/Tabla/Header.razor`
- `Components/Tabla/Rows.razor`
- `Components/Tabla/Columna.razor`
- `Components/Tabla/Row.razor`

Características implementadas:
- Registro de columnas/celdas por cascada (`ITablaRegistration`).
- Render de encabezados y celdas según orden de declaración.
- Paginación integrada en `tfoot`.
- Método público `RefreshAsync(resetPage)` para recargas externas.

### Cliente: integración en páginas
- `Pages/Administracion/Gestion/Personas.razor`
  - Reemplaza tabla manual por `<Tabla T="PersonaResponse">`.
  - Búsqueda por `oninput` y recarga con `RefreshAsync(resetPage: true)`.

- `Pages/Administracion/Gestion/Usuarios.razor`
  - Reemplaza tabla manual por `<Tabla T="UsuarioResponse">`.
  - Mantiene columnas complejas (perfiles, estado, acciones).
  - Búsqueda por `oninput` + recarga de tabla.

### Backend: Usuarios server-side (repositorio + controlador)
- `Api/Data/Repositories/UsuarioRepository.cs`
  - `SearchAsync(string? searchText, PaginacionDto paginacion, ...)`
  - `CountAsync(string? searchText, ...)`
  - Filtrado con `EF.Functions.Like(...)` y paginación con `Skip/Take`.

- `Api/Controllers/UsuarioController.cs`
  - `GET api/Usuario/search?search=...&Pagina=...&RegistrosPorPagina=...`
  - `POST api/Usuario/search?search=...` (compatibilidad)
  - `GET api/Usuario/count?search=...`

### Cliente: servicio de Usuarios
- `Services/IUsuarioService.cs`
  - `SearchAsync(PaginacionDto paginacion, string? searchText, ...)`
  - `CountAsync(string? searchText, ...)`

- `Services/UsuarioService.cs`
  - Consume endpoints `search/count`.
  - Compatibilidad de respuestas (array o `TableResponse<T>`).
  - Fallback defensivo cuando el backend publicado no expone endpoints nuevos o responde HTML.

### Compatibilidad en Personas
- `Services/PersonaService.cs`
  - `GetTableAsync` prioriza `GET` paginado y usa `POST api/persona/table` como fallback.
</implementation>

<testing>
Verification plan:
- `Personas`: listado, cambio de página, cambio de tamaño, búsqueda y acciones CRUD.
- `Usuarios`: listado, búsqueda, paginación y conteo con datos desde servidor.
- `Usuarios`: asignar/retirar perfiles mantiene actualización visual de tabla.
- Layout responsive: paginación integrada en tabla en desktop/mobile.
- Compatibilidad deploy parcial: no rompe ante 405 o payload HTML en endpoints nuevos.
</testing>
