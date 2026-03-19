# API Communication Configuration

<purpose>
Definir cómo el cliente Blazor consume la API actual de RugbyEngine con `HttpClient` directo, configuración por `IConfiguration` y contratos vigentes de búsqueda/paginación.
</purpose>

<requirements>
- Consumir API con `HttpClient` directo en servicios.
- URL base configurable por `API_BASE_URL` o `Api:BaseUrl`.
- Endpoints de tabla por `GET` con query `search/page/pageSize`.
- Endpoint de conteo separado (`/count`).
- Manejo de cancelación con `CancellationToken` en capa cliente.
</requirements>

<implementation>
Program.cs:
- Registra `HttpClient` scoped.
- Lee base URL desde configuración o fallback al host actual.

Servicios cliente:
- `PersonaService` y `UsuarioService` usan:
  - `GetFromJsonAsync` para GET de búsqueda y conteo.
  - `PostAsJsonAsync`, `PutAsJsonAsync`, `DeleteAsync` para CRUD.
- No se usa `CreateAuthorizedRequestAsync`.

Contratos vigentes de endpoints:
- Personas
  - `GET api/persona/search?search={text}&page={n}&pageSize={m}`
  - `GET api/persona/count?search={text}`
- Usuarios
  - `GET api/usuario/search?search={text}&page={n}&pageSize={m}`
  - `GET api/usuario/count?search={text}`

Notas:
- El backend devuelve listas JSON para búsqueda.
- El cliente compone `TableResponse<T>` localmente con registros + total.
</implementation>

<testing>
Verification plan:
- Confirmar que `search` recibe `page/pageSize` por query.
- Confirmar que `count` se invoca una sola vez por refresh.
- Confirmar que cancelaciones no pisan resultados de requests más nuevos.
</testing>
