# JWT Authentication

<purpose>
Sistema de autenticación basado en JWT que permite a los usuarios iniciar sesión, mantener su sesión activa mediante cookies, y cerrar sesión de forma segura. El sistema valida la expiración del token y sincroniza el estado de autenticación con el header Authorization del HttpClient.
</purpose>

<requirements>
- El usuario puede iniciar sesión con credenciales (usuario/contraseña) via POST a `api/Auth/Login`
- El token JWT se almacena en cookie `re_access_token` con expiración sincronizada al claim `ValidTo`
- El estado de autenticación se propaga via `AuthenticationStateProvider` y `CascadingAuthenticationState`
- El logout notifica al servidor via POST a `api/Auth/LogOut` con header Bearer antes de eliminar la cookie
- Las rutas protegidas con `[Authorize]` redirigen automáticamente al login si no hay sesión válida
- El `HttpClient.DefaultRequestHeaders.Authorization` se actualiza automáticamente con el token
</requirements>

<implementation>
Arquitectura:
- **AuthService**: Orquesta login/logout, construye URIs absolutas, persiste tokens
- **ApiAuthenticationStateProvider**: Lee token de cookies, parsea JWT, valida expiración, notifica cambios de estado
- **CookieService**: Abstracción de cookies via JS interop con encoding URI

Flujo de Login:
1. `AuthService.LoginAsync()` llama a `POST api/Auth/Login` con `LoginRequest`
2. Si exitoso, extrae token de `LoginResponse` y llama a `PersistTokenAsync()`
3. `PersistTokenAsync()` parsea el JWT para obtener `ValidTo`, calcula expiración de cookie
4. Llama a `CookieService.SetCookieAsync()` y `NotifyUserAuthentication()`
5. `NotifyUserAuthentication()` crea `ClaimsPrincipal` desde JWT y actualiza header Authorization

Flujo de Logout:
1. `AuthService.LogoutAsync()` recupera token de cookie
2. Si `notifyServer=true`, envía POST a `api/Auth/LogOut` con Authorization header
3. Elimina cookie via `CookieService.DeleteCookieAsync()`
4. Llama a `NotifyUserLogout()` que limpia Authorization header y notifica estado anónimo

Key files:
- `Services/AuthService.cs` - Constante `TokenCookieName = "re_access_token"`, helpers `BuildApiUri()`, `NormalizeBaseUri()`
- `Services/ApiAuthenticationStateProvider.cs` - `BuildPrincipalFromCookieAsync()`, `TryCreatePrincipal()` con validación de expiración
- `Services/CookieService.cs` - Encoding/decoding URI para valores de cookie
- `wwwroot/js/cookieInterop.js` - Funciones `set`, `get`, `delete` con `SameSite=Lax` y `Secure` condicional
</implementation>

<testing>
Verification plan:
- Login exitoso: Token almacenado en cookie, `HttpClient.DefaultRequestHeaders.Authorization` configurado, redirige a returnUrl
- Login fallido: `LoginResponse.Success = false`, muestra mensaje de error, no almacena cookie
- Logout: Cookie eliminada, Authorization header limpiado, servidor notificado, redirige a login
- Token expirado: `TryCreatePrincipal()` retorna null si `jwt.ValidTo <= DateTime.UtcNow`, cookie eliminada
- Navegación sin sesión: `GetAuthenticationStateAsync()` retorna principal anónimo, `AuthorizeRouteView` redirige
- Refresh de página: Token recuperado de cookie, `ClaimsPrincipal` reconstruido, Authorization header configurado
</testing>
