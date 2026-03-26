# Route Protection with Authorization

<purpose>
Proteger rutas del cliente con autenticación y redirigir a login cuando el usuario no tiene sesión válida.
</purpose>

<requirements>
- Uso de `CascadingAuthenticationState`.
- Resolución de rutas con `AuthorizeRouteView`.
- Redirección automática a `/Auth/Login?returnUrl=...` cuando no autorizado.
- Soporte de `[Authorize]`, `[Authorize(Roles=...)]` y `[AllowAnonymous]`.
- Protección por rol basada en mapa de rutas cargado desde API (`MainMenuService`).
- Monitoreo periódico de sesión con `SessionMonitor`.
</requirements>

<implementation>
Archivos:
- `App.razor` — router con `AuthorizeRouteView`.
- `Layout/MenuRouteGuard.razor` — protección por rol usando `MainMenuService`.
- `Pages/Utility/SessionMonitor.razor` — monitoreo periódico de sesión.

Flujo actual:
1. Router encuentra ruta.
2. `AuthorizeRouteView` evalúa autorización.
3. Si no autorizado, ejecuta bloque `NotAuthorized` y navega a login con `returnUrl` codificada.
4. `MenuRouteGuard` consulta `MainMenuService.GetRequiredRoleForRoute` para verificar el rol requerido.

Estado de autenticación:
- `ApiAuthenticationStateProvider` reconstruye principal desde cookie JWT.
- Si token inválido o vencido, elimina cookie y notifica usuario anónimo.
- Sincroniza header `Authorization` en `HttpClient`.

Monitoreo de sesión:
- `SessionMonitor` verifica la sesión activa periódicamente con `AuthService.ValidateSessionAsync`.
- Si la sesión expira o el refresh falla, limpia cookie y redirige a login.

Protección por rol:
- `MainMenuService` carga mapa de roles desde `GET api/Menu/RouteRoles`.
- `GetRequiredRoleForRoute` devuelve el rol necesario para una ruta (coincidencia exacta + prefijo).
- `MenuRouteGuard` bloquea renderizado si el usuario no tiene el rol requerido.
</implementation>

<testing>
Verification plan:
- Ruta protegida sin login redirige a `/Auth/Login`.
- Login exitoso vuelve a `returnUrl`.
- Token vencido invalida sesión automáticamente.
- Ruta con rol requerido bloquea acceso sin el rol correcto.
- `SessionMonitor` detecta expiración y redirige a login.
</testing>
