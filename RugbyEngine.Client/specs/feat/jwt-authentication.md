# JWT Authentication

<purpose>
Autenticación JWT en cliente Blazor con persistencia en cookie y sincronización del estado de autenticación en `AuthenticationStateProvider`.
</purpose>

<requirements>
- Login por `POST api/Auth/Login`.
- Persistencia de token en cookie `re_access_token`.
- Validación de expiración JWT.
- Logout local + notificación opcional a API.
- Header `Authorization` gestionado por `ApiAuthenticationStateProvider`.
</requirements>

<implementation>
Servicios:
- `AuthService`
  - Login, logout, refresh de sesión
  - construcción de URIs con `BuildApiUri`
- `ApiAuthenticationStateProvider`
  - reconstruye principal desde cookie
  - valida expiración
  - sincroniza `HttpClient.DefaultRequestHeaders.Authorization`

Persistencia:
- `CookieService` + `cookieInterop.js`
- token cookie: `re_access_token`

Flujo:
1. Login devuelve token.
2. Se guarda cookie con expiración.
3. Provider notifica usuario autenticado.
4. Logout limpia cookie y estado.
</implementation>

<testing>
Verification plan:
- Login válido: cookie creada + principal autenticado.
- Logout: cookie eliminada + principal anónimo.
- Token vencido: sesión inválida y limpieza automática.
</testing>
