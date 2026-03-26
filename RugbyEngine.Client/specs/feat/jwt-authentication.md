# JWT Authentication

<purpose>
Autenticación JWT en cliente Blazor con persistencia en cookie y sincronización del estado de autenticación en `AuthenticationStateProvider`.
</purpose>

<requirements>
- Login por `POST api/Auth/Login`.
- Persistencia de token en cookie `re_access_token`.
- Validación de expiración JWT.
- Logout local + notificación a API por `POST api/Auth/LogOut`.
- Refresco de sesión por `POST api/Auth/Refresh` con `ValidateSessionAsync`.
- Header `Authorization` gestionado por `ApiAuthenticationStateProvider`.
- Evento `AuthenticationStateChanged` notifica cambios de sesión.
</requirements>

<implementation>
Servicios:
- `AuthService`
  - `LoginAsync` — envía credenciales a `POST api/Auth/Login`, persiste token en cookie.
  - `LogoutAsync` — notifica a API por `POST api/Auth/LogOut` (con Bearer token) y limpia cookie/estado.
  - `ValidateSessionAsync` — envía token actual a `POST api/Auth/Refresh`; si recibe nuevo token lo persiste, si falla limpia sesión.
  - `IsAuthenticatedAsync` — delega a `ApiAuthenticationStateProvider.HasValidTokenAsync`.
  - `GetTokenAsync` — lee token de cookie.
  - `PersistTokenAsync` — guarda token en cookie con expiración alineada al `ValidTo` del JWT (fallback configurable por `Jwt:ExpiryInMinutes` o `JWT_EXPIRATION_MINUTES`, default 60 min).
  - Usa `BuildApiUri` para construir URIs relativas a la base configurada.
  - Emite evento `AuthenticationStateChanged` en login/logout/refresh.

- `ApiAuthenticationStateProvider`
  - reconstruye principal desde cookie
  - valida expiración
  - sincroniza `HttpClient.DefaultRequestHeaders.Authorization`

Persistencia:
- `CookieService` + `cookieInterop.js`
- token cookie: `re_access_token`

Flujo login:
1. Login devuelve token.
2. Se guarda cookie con expiración.
3. Provider notifica usuario autenticado.

Flujo logout:
1. Notifica API con Bearer token (si `notifyServer=true`).
2. Limpia cookie.
3. Provider notifica usuario anónimo.

Flujo refresco:
1. `ValidateSessionAsync` envía token actual a `POST api/Auth/Refresh`.
2. Si respuesta exitosa con nuevo token: persiste nuevo token.
3. Si respuesta exitosa sin token: mantiene sesión actual.
4. Si falla: limpia cookie y notifica logout.
</implementation>

<testing>
Verification plan:
- Login válido: cookie creada + principal autenticado.
- Logout: cookie eliminada + principal anónimo + API notificada.
- Token vencido: sesión inválida y limpieza automática.
- `ValidateSessionAsync` con token válido: sesión refrescada.
- `ValidateSessionAsync` con token inválido: sesión limpiada y logout.
- Evento `AuthenticationStateChanged` se dispara en login, logout y refresh.
</testing>
