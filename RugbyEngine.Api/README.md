# RugbyEngine.Api

API backend de RugbyEngine (ASP.NET Core).

## Estado actual
- Endpoints REST para Personas, Usuarios, Perfiles, Auth y Menú.
- Búsqueda paginada en Personas y Usuarios por query string.
- Endpoints de conteo separados para tablas (`/count`).
- Respuestas de búsqueda devuelven listas JSON.

## Endpoints de tabla (actuales)
### Personas
- `GET /api/persona/search?search={text}&page={n}&pageSize={m}`
- `GET /api/persona/count?search={text}`

### Usuarios
- `GET /api/usuario/search?search={text}&page={n}&pageSize={m}`
- `GET /api/usuario/count?search={text}`

## Notas de implementación
- Repositorios aplican filtrado por texto y paginación con `Skip/Take`.
- Búsqueda case-insensitive para PostgreSQL usando `EF.Functions.ILike`.
- Endpoints de búsqueda devuelven `List<ResponseDto>` (sin `TableResponse`).

## Autenticación/autorización
- API protegida con JWT.
- Controladores con atributos `[Authorize]` y roles según módulo.

## Convención de respuesta
- Endpoints de listados para tablas devuelven JSON con listas de datos.
- El total se obtiene por endpoint `count`.
