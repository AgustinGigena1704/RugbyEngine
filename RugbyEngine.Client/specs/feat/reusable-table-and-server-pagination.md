# Reusable Table + Server-side Pagination (Personas y Usuarios)

<purpose>
Unificar listados administrativos con `<Tabla>` reusable y paginación server-side real para Personas y Usuarios.
</purpose>

<requirements>
- Componente genérico `<Tabla T="...">`.
- Definición declarativa con `Header/Columna` y `Rows/Row`.
- Paginación integrada en `tfoot` con contador compacto `x / x2`.
- Cancelación de requests previos en refresh.
- Loading overlay sin limpiar registros previos.

Firmas actuales:
- `SearchFunc: Func<PaginacionDto, CancellationToken, Task<List<T>>>`
- `CountFunc: Func<CancellationToken, Task<int>>?`
</requirements>

<implementation>
Cliente:
- `Components/Tabla/Tabla.razor` administra:
  - estado de paginación
  - cancelación por `CancellationTokenSource`
  - cálculo de `DisplayedCount`
- Componentes declarativos de definición:
  - `Header.razor` / `TablaHeader.razor` — contenedores de encabezados
  - `Columna.razor` — registra nombre de columna via `ITablaRegistration.RegisterHeader`
  - `Rows.razor` / `TablaRows.razor` — contenedores de filas
  - `Row.razor` — registra renderer de celda via `ITablaRegistration.RegisterCell`; soporta `Value` (simple) o `ChildContent` (template)
  - `ITablaRegistration.cs` — interfaz de registro entre `Tabla` y sus hijos
- `Personas.razor` y `Usuarios.razor` usan debounce en filtro + `RefreshAsync(resetPage: true)`.

Parámetros del componente `<Tabla T="...">`:
- `SearchFunc: Func<PaginacionDto, CancellationToken, Task<List<T>>>` (requerido)
- `CountFunc: Func<CancellationToken, Task<int>>?`
- `Paginacion: bool` (default `true`)
- `PageSizes: IReadOnlyList<int>?` (default `[10, 20, 50]`)
- `EmptyText: string` (default `"No se encontraron registros."`)

Backend:
- `PersonaController.Search` y `UsuarioController.Search` usan:
  - `GET search?search=&page=&pageSize=`
- `Count` separado por endpoint `/count`.

Repositorio:
- búsqueda por texto con `ILike` (case-insensitive) en PostgreSQL.

Contador:
- `DisplayedCount = min(total, ((pagina-1)*pageSize)+rows.Count)`

Paginación UI (en `tfoot`):
- Contador compacto `x / x2`
- Selector de filas por página
- Botones `<` / `>` con navegación
- Indicador `Pagina de TotalPaginas`
</implementation>

<testing>
Verification plan:
- Filtro incremental (`g` -> `gi`) devuelve resultados esperados.
- `count` se llama una vez por refresh.
- Cambios rápidos de filtro cancelan requests anteriores sin borrar lista previa.
- Paginación y contador compactos consistentes en desktop y mobile.
</testing>
