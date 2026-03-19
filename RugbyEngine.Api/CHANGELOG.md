# CHANGELOG - RugbyEngine.Api

## Estado documentado (actualización)
- Se documenta contrato vigente de búsqueda y paginación para Personas y Usuarios.
- Se confirma que endpoints `search` devuelven listas JSON y `count` total numérico.
- Se registra uso de `ILike` en repositorios para filtros case-insensitive sobre PostgreSQL.

## Contrato vigente de tablas
- Personas:
  - `GET /api/persona/search?search=&page=&pageSize=`
  - `GET /api/persona/count?search=`
- Usuarios:
  - `GET /api/usuario/search?search=&page=&pageSize=`
  - `GET /api/usuario/count?search=`

## Observaciones
- El cliente construye `TableResponse` localmente.
- Backend mantiene respuesta en formato lista JSON para `search`.
