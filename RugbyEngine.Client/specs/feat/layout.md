# Layout System (Bootstrap 5)

<purpose>
Documentar el layout actual de la aplicación cliente: topbar fija, drawers móviles, side menu opcional y contenido principal en Bootstrap + CSS propio.
</purpose>

<requirements>
- Topbar fija (`app-topbar`) con navegación principal.
- Drawers móviles para menú y cuenta.
- Side menu contextual opcional (desktop fijo, mobile deslizable).
- Scroll principal controlado en `app-main`.
- Sin componentes MudBlazor en shell.
</requirements>

<implementation>
Componentes principales:
- `Layout/MainLayout.razor` — shell principal con topbar, body y drawers.
- `Layout/LoginLayout.razor` — layout dedicado para páginas de autenticación (login, reset password).
- `Layout/NavMenu.razor` — menú de navegación principal (desktop).
- `Layout/MobileNavMenu.razor` — drawer de menú en mobile.
- `Layout/MobileUserDrawer.razor` — drawer de usuario en mobile.
- `Layout/AccountDrawer.razor` — drawer para acciones de cuenta de usuario.
- `Layout/MenuRouteGuard.razor` — protección de rutas por rol usando `MainMenuService.GetRequiredRoleForRoute`.

Estilos principales:
- `wwwroot/css/app.css`
- `Layout/MainLayout.razor.css`
- `Layout/NavMenu.razor.css`

Comportamiento:
- Topbar fija con altura controlada por variable CSS.
- `app-body` ocupa alto restante del viewport.
- `app-main` concentra scroll vertical del contenido.
- Drawers abren/cerran por estado local y backdrop.
- `MenuRouteGuard` verifica el rol requerido para la ruta actual antes de renderizar el contenido.

Notas:
- El layout actual no depende de MudBlazor.
- Se usa Bootstrap para utilidades + CSS propio para identidad visual.
- `LoginLayout` se usa en páginas con `[AllowAnonymous]` como Login y ResetPassword.
</implementation>

<testing>
Verification plan:
- Verificar topbar fija sin scroll global extra.
- Verificar side menu desktop sticky.
- Verificar drawers mobile y backdrop.
- Verificar navegación principal visible según autenticación.
</testing>
