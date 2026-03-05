# Route Protection with Authorization

<purpose>
Sistema de protección de rutas que requiere autenticación para acceder a páginas específicas, redirigiendo automáticamente al login cuando no hay sesión válida y preservando la URL original para redirección post-login.
</purpose>

<requirements>
- Las páginas pueden marcarse con `[Authorize]` para requerir autenticación
- Las páginas públicas usan `[AllowAnonymous]` para permitir acceso sin sesión
- Navegación a ruta protegida sin sesión redirige a `/Auth/Login?returnUrl={encodedUrl}`
- El `returnUrl` se codifica con `Uri.EscapeDataString()` para preservar caracteres especiales
- `CascadingAuthenticationState` propaga el estado de auth a toda la app
- `AuthorizeView` en componentes para renderizado condicional basado en auth
</requirements>

<implementation>
Configuración en App.razor:
```razor
<CascadingAuthenticationState>
    <Router AppAssembly="@typeof(App).Assembly" NotFoundPage="typeof(Pages.NotFound)">
        <Found Context="routeData">
            <AuthorizeRouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)">
                <NotAuthorized>
                    @{
                        var returnUrl = Uri.EscapeDataString(NavManager.Uri);
                        NavManager.NavigateTo($"/Auth/Login?returnUrl={returnUrl}");
                    }
                </NotAuthorized>
            </AuthorizeRouteView>
            <FocusOnNavigate RouteData="@routeData" Selector="h1" />
        </Found>
    </Router>
</CascadingAuthenticationState>
```

Componentes clave:
- `CascadingAuthenticationState` - Proporciona `Task<AuthenticationState>` a descendientes
- `AuthorizeRouteView` - Evalúa `[Authorize]`/`[AllowAnonymous]` en la página de destino
- `NotAuthorized` template - Se ejecuta cuando no hay sesión válida
- `AuthorizeView` - Usado en NavMenu para mostrar/ocultar elementos

Páginas protegidas (requieren `[Authorize]`):
- `Pages/Home/Home.razor` - Ruta: `/`
- `Pages/Extra/Counter.razor` - Ruta: `/counter`
- `Pages/Extra/Weather.razor` - Ruta: `/weather`

Páginas públicas (usan `[AllowAnonymous]`):
- `Pages/Auth/Login.razor` - Ruta: `/Auth/Login`
- `Pages/NotFound.razor` - NotFoundPage del Router

Flujo de redirección:
1. Usuario navega a `/counter` sin sesión
2. `AuthorizeRouteView` detecta `[Authorize]` y ausencia de auth
3. Template `NotAuthorized` captura URL completa `NavManager.Uri`
4. Codifica URL y navega a `/Auth/Login?returnUrl=%2Fcounter`
5. Login.razor extrae y decodifica `returnUrl`
6. Post-login, navega a `/counter` con `forceLoad: true`

Key files:
- `App.razor` - Router con autorización
- `Pages/Home/Home.razor` - `@attribute [Authorize]`
- `Pages/Auth/Login.razor` - `@attribute [AllowAnonymous]`
- `Layout/NavMenu.razor` - `<AuthorizeView>` para UI condicional
</implementation>

<testing>
Verification plan:
- `/` sin sesión → Redirige a `/Auth/Login?returnUrl=%2F`
- `/counter` sin sesión → Redirige a `/Auth/Login?returnUrl=%2Fcounter`
- `/weather` sin sesión → Redirige a `/Auth/Login?returnUrl=%2Fweather`
- `/Auth/Login` sin sesión → Muestra formulario (no redirige)
- Post-login con returnUrl → Navega a la URL original
- Post-login sin returnUrl → Navega a `/`
- Usuario autenticado → Puede acceder a todas las páginas protegidas
- NavMenu: Links visibles solo en `<Authorized>`, icono cuenta en `<NotAuthorized>`
- URL con caracteres especiales preservada correctamente en returnUrl
</testing>
