# Route Protection with Authorization

<purpose>
Proteger rutas del cliente con autenticación y redirigir a login cuando el usuario no tiene sesión válida.
</purpose>

<requirements>
- Uso de `CascadingAuthenticationState`.
- Resolución de rutas con `AuthorizeRouteView`.
- Redirección automática a `/Auth/Login?returnUrl=...` cuando no autorizado.
- Soporte de `[Authorize]`, `[Authorize(Roles=...)]` y `[AllowAnonymous]`.
</requirements>

<implementation>
Archivo:
- `App.razor`

Flujo actual:
1. Router encuentra ruta.
2. `AuthorizeRouteView` evalúa autorización.
3. Si no autorizado, ejecuta bloque `NotAuthorized` y navega a login con `returnUrl` codificada.

Estado de autenticación:
- `ApiAuthenticationStateProvider` reconstruye principal desde cookie JWT.
- Si token inválido o vencido, elimina cookie y notifica usuario anónimo.
- Sincroniza header `Authorization` en `HttpClient`.
</implementation>

<testing>
Verification plan:
- Ruta protegida sin login redirige a `/Auth/Login`.
- Login exitoso vuelve a `returnUrl`.
- Token vencido invalida sesión automáticamente.
</testing>
