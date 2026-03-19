# Layout System (Bootstrap 5 — sin MudBlazor)

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
- `Layout/MainLayout.razor`
- `Layout/NavMenu.razor`
- `Layout/MobileNavMenu.razor`
- `Layout/MobileUserDrawer.razor`
- `Layout/MenuRouteGuard.razor`

Estilos principales:
- `wwwroot/css/app.css`

Comportamiento:
- Topbar fija con altura controlada por variable CSS.
- `app-body` ocupa alto restante del viewport.
- `app-main` concentra scroll vertical del contenido.
- Drawers abren/cerran por estado local y backdrop.

Notas:
- El layout actual no depende de MudBlazor.
- Se usa Bootstrap para utilidades + CSS propio para identidad visual.
</implementation>

<testing>
Verification plan:
- Verificar topbar fija sin scroll global extra.
- Verificar side menu desktop sticky.
- Verificar drawers mobile y backdrop.
- Verificar navegación principal visible según autenticación.
</testing>
