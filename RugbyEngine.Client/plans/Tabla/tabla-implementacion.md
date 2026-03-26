# Implementación de `<Tabla>` reusable (estado actual)

## Estado
La implementación de `<Tabla>` está completada y en uso en `Personas` y `Usuarios`.

## Contrato vigente del componente
- `SearchFunc: Func<PaginacionDto, CancellationToken, Task<List<T>>>` (requerido)
- `CountFunc: Func<CancellationToken, Task<int>>?`
- `Paginacion: bool` (default `true`)
- `PageSizes: IReadOnlyList<int>?` (default `[10, 20, 50]`)
- `EmptyText: string` (default `"No se encontraron registros."`)

## Componentes del sistema de tabla
- `Tabla.razor` — componente principal que administra estado, paginación y cancelación.
- `Header.razor` / `TablaHeader.razor` — contenedores de encabezados (render passthrough).
- `Columna.razor` — registra nombre de columna via `ITablaRegistration.RegisterHeader`.
- `Rows.razor` / `TablaRows.razor` — contenedores de filas (render passthrough).
- `Row.razor` — registra renderer de celda via `ITablaRegistration.RegisterCell`; soporta `Value` (simple) o `ChildContent` (template).
- `ITablaRegistration.cs` — interfaz de registro (`ClearDefinition`, `RegisterHeader`, `RegisterCell`).
- `Tabla.razor.css` — estilos scoped del componente.

## Comportamiento actual
- Render declarativo con `Header/Columna` y `Rows/Row`.
- Paginación en `tfoot` con:
  - contador compacto `x / x2`
  - selector de filas
  - navegación `<` y `>`
- Cálculo de contador acumulado:
  - `DisplayedCount = min(total, ((pagina - 1) * pageSize) + rows.Count)`
- Soporte de cancelación en `RefreshAsync`:
  - cancela request anterior con `CancellationTokenSource`
  - evita aplicar resultados stale
- Loading overlay durante refresh sin limpiar filas previas.

## Integraciones activas
- `Pages/Administracion/Gestion/Personas.razor`
- `Pages/Administracion/Gestion/Usuarios.razor`

## Backend consumido por cliente
- `GET /api/persona/search?search=&page=&pageSize=`
- `GET /api/persona/count?search=`
- `GET /api/usuario/search?search=&page=&pageSize=`
- `GET /api/usuario/count?search=`

## Validación funcional esperada
- Cambio de filtro con debounce.
- Cambio de página y tamaño de página.
- Count invocado una vez por refresh.
- `search` devuelve lista JSON (sin `TableResponse` desde backend).
