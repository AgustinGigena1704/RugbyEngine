# API Communication Configuration

<purpose>
Definir cómo el cliente Blazor consume la API actual de RugbyEngine, incluyendo todos los endpoints vigentes, patrones de consumo y configuración.
</purpose>

<requirements>
- Consumir API con `HttpClient` directo en servicios (`PersonaService`, `UsuarioService`).
- `PerfilService` y `AuthService` usan `CreateAuthorizedRequestAsync` / `BuildApiUri` con `HttpRequestMessage` manual y header Bearer.
- URL base hardcodeada en `Program.cs`: producción (`https://rugbyengine.agigena.com`), desarrollo (`https://localhost:7083`).
- Endpoints de tabla por `GET` con query `search/page/pageSize`.
- Endpoint de conteo separado (`/count`).
- Manejo de cancelación con `CancellationToken` en capa cliente.
- `OperationCanceledExceptionDelegatingHandler` registrado como handler HTTP.
</requirements>

<implementation>
Program.cs:
- Registra `HttpClient` scoped con `BaseAddress` hardcodeada según entorno.
- Registra `OperationCanceledExceptionDelegatingHandler` como `TransientDelegatingHandler`.

Servicios cliente:
- `PersonaService` y `UsuarioService` usan:
  - `GetFromJsonAsync` para GET de búsqueda y conteo.
  - `PostAsJsonAsync`, `PutAsJsonAsync`, `DeleteAsync` para CRUD.
  - No usan `CreateAuthorizedRequestAsync`.
- `AuthService` usa `BuildApiUri` + `HttpRequestMessage` manual para Login, Logout y Refresh.
- `PerfilService` usa `CreateAuthorizedRequestAsync` con token Bearer para todos sus endpoints.
- `MainMenuService` usa `HttpClient.GetAsync` directo para menú y roles de ruta.

Contratos vigentes de endpoints:

Personas:
- `GET api/persona` — listado completo
- `GET api/persona/{id}` — detalle
- `GET api/persona/search?search={text}&page={n}&pageSize={m}` — búsqueda paginada
- `GET api/persona/count?search={text}` — conteo
- `POST api/persona` — crear
- `PUT api/persona/{id}` — actualizar
- `DELETE api/persona/{id}` — eliminar

Usuarios:
- `GET api/usuario` — listado completo
- `GET api/usuario/{id}` — detalle
- `GET api/usuario/search?search={text}&page={n}&pageSize={m}` — búsqueda paginada
- `GET api/usuario/count?search={text}` — conteo
- `POST api/usuario` — crear
- `PUT api/usuario/{id}` — actualizar
- `DELETE api/usuario/{id}` — eliminar
- `POST api/usuario/{id}/perfiles/{perfilId}` — asignar perfil
- `DELETE api/usuario/{id}/perfiles/{perfilId}` — retirar perfil

Autenticación:
- `POST api/Auth/Login` — login
- `POST api/Auth/LogOut` — logout (con token Bearer)
- `POST api/Auth/Refresh` — validar/refrescar sesión (con token Bearer)

Perfiles:
- `GET api/Perfil` — listado completo
- `GET api/Perfil/mine` — perfiles del usuario actual
- `POST api/Perfil/{id}/assign` — asignar perfil
- `DELETE api/Perfil/{id}/unassign` — retirar perfil

Menú:
- `GET api/Menu` — árbol de menú
- `GET api/Menu/RouteRoles` — mapa de roles por ruta

Notas:
- El backend devuelve listas JSON para búsqueda.
- El cliente compone `TableResponse<T>` localmente con registros + total.
</implementation>

<testing>
Verification plan:
- Confirmar que `search` recibe `page/pageSize` por query.
- Confirmar que `count` se invoca una sola vez por refresh.
- Confirmar que cancelaciones no pisan resultados de requests más nuevos.
- Confirmar endpoints de Auth, Perfil y Menu responden correctamente.
</testing>
