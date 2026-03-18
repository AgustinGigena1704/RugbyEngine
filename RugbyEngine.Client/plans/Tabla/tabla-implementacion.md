# Plan detallado de implementación - Componente `<Tabla>` reutilizable

## Objetivo
Construir un componente genérico de Blazor WebAssembly llamado `<Tabla>` que replique el comportamiento y estética actual de la paginación de `Personas`, con API declarativa y reusable para múltiples pantallas.

## Alcance funcional acordado
- Componente genérico por tipo de fila: `T`.
- Soporte de paginación opcional con `Paginacion` (`bool`).
- Fuente de datos paginada mediante:
  - `SearchFunc`: `Func<PaginacionDto, Task<List<T>>>`
  - `CountFunc`: `Func<Task<int>>` (separado para total).
- Sintaxis declarativa objetivo:
  - `<Tabla T="..." Paginacion="true" SearchFunc="..." CountFunc="...">`
  - Secciones de estructura: `Header/Columna` y `Rows/Row`.
- Paginación visual integrada dentro de la tabla (`tfoot`).
- Contador compacto `x / x2` tanto en mobile como desktop.
- Botones de navegación con símbolos `<` y `>`.

## Restricciones de diseño del proyecto
- No hardcodear tamaños fijos en tablas.
- Mantener comportamiento responsive en mobile y desktop.
- Conservar estética general de la tabla actual de Personas.

## Diseño técnico propuesto

### 1) Componentes a crear
1. `RugbyEngine.Client/Components/Tabla/Tabla.razor`
2. `RugbyEngine.Client/Components/Tabla/Tabla.razor.cs` (opcional, si se separa lógica)
3. `RugbyEngine.Client/Components/Tabla/Tabla.razor.css`
4. `RugbyEngine.Client/Components/Tabla/TablaHeader.razor`
5. `RugbyEngine.Client/Components/Tabla/Columna.razor`
6. `RugbyEngine.Client/Components/Tabla/TablaRows.razor`
7. `RugbyEngine.Client/Components/Tabla/Row.razor`
8. Modelos auxiliares en `RugbyEngine.Client/Components/Tabla/Models/` (si son necesarios)

### 2) API pública del componente `Tabla<T>`
Parámetros mínimos:
- `bool Paginacion { get; set; }`
- `Func<PaginacionDto, Task<List<T>>> SearchFunc { get; set; } = default!;`
- `Func<Task<int>>? CountFunc { get; set; }`
- `IReadOnlyList<int>? PageSizes { get; set; }` (default interno: 10, 20, 50)
- `RenderFragment? ChildContent { get; set; }`
- `string EmptyText { get; set; } = "No se encontraron registros."`

Estado interno sugerido:
- `List<T> _rows`
- `bool _isLoading`
- `PaginacionDto _paginacion = new() { Pagina = 1, RegistrosPorPagina = 10 }`
- `int _total`

### 3) API declarativa de hijos
- `TablaHeader` contiene `Columna`.
- `Columna` define metadata visual (`Nombre`) y/o selector/template.
- `TablaRows<T>` contiene definición de filas.
- `Row<T>` define el valor/template por columna.

Notas:
- Se puede resolver con `CascadingValue` para registrar columnas/filas en `Tabla<T>`.
- Alternativamente, usar templates fuertemente tipados para simplificar runtime.

## Flujo de datos

### Carga inicial
1. Si `Paginacion = true`:
   - Ejecutar `SearchFunc(_paginacion)`.
   - Ejecutar `CountFunc()` para total (si existe).
2. Si `Paginacion = false`:
   - Ejecutar `SearchFunc(new PaginacionDto { Pagina = 1, RegistrosPorPagina = int.MaxValue })`.
   - Total = cantidad de elementos retornados.

### Cambio de página (`<` / `>`)
1. Validar límites [1..TotalPaginas].
2. Actualizar `_paginacion.Pagina`.
3. Ejecutar `SearchFunc(_paginacion)`.
4. Mantener `_total` previamente obtenido o refrescar con `CountFunc` según estrategia.

### Cambio de tamaño de página
1. Actualizar `_paginacion.RegistrosPorPagina`.
2. Reset de página a 1.
3. Ejecutar `SearchFunc`.
4. Recalcular `TotalPaginas` usando `_total`.

## Comportamiento visual de paginación
- Render en `tfoot` para estar dentro de la tabla.
- Elementos:
  - Contador de página de datos: `rows.Count / total`.
  - Selector "Filas".
  - Navegación `<` y `>`.
  - Indicador de página actual: `actual de totalPaginas`.
- Tipografía en negrita, consistente con la tabla.
- Una sola línea utilizable en mobile/desktop (con `overflow-x` si se requiere).

## Plan de migración de Personas
1. Reemplazar markup manual de tabla en `Pages/Administracion/Gestion/Personas.razor` por `<Tabla T="PersonaResponse" ...>`.
2. Conectar:
   - `SearchFunc` -> función local que llama a `PersonaService` con `PaginacionDto`.
   - `CountFunc` -> función local para total (endpoint de conteo o fallback).
3. Mantener columna de acciones (Editar/Eliminar) con template de fila.
4. Verificar que UX final conserve:
   - contador `x / x2`
   - `tfoot` integrado
   - `<>` para navegación

## Validación y pruebas
- Build completo del workspace.
- Validación funcional manual:
  1. Carga inicial con registros.
  2. Cambio de página.
  3. Cambio de filas por página.
  4. Escenario sin datos.
  5. Responsive mobile.
- Confirmar que no se introducen tamaños fijos no responsivos.

## Riesgos y mitigaciones
- **Riesgo:** API declarativa `Header/Rows` compleja para binding dinámico.
  - **Mitigación:** comenzar con plantilla tipada estable y luego extender DSL.
- **Riesgo:** divergencia entre backend legado y nuevo (conteo/datos).
  - **Mitigación:** encapsular compatibilidad en funciones proveedoras (`SearchFunc`/`CountFunc`).
- **Riesgo:** regresión visual en mobile.
  - **Mitigación:** CSS scoped + revisión en breakpoints.

## Entregables
1. Nuevo componente `<Tabla>` reusable.
2. Subcomponentes declarativos `Header/Columna` y `Rows/Row`.
3. Estilos scoped del componente.
4. Integración de `Personas` al nuevo componente.
5. Compilación validada y checklist de comportamiento cubierto.
