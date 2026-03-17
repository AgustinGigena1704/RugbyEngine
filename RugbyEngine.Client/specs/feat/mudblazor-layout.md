# Layout System (Bootstrap 5 — sin MudBlazor)

## Cambios recientes
- Loader consistente y reutilizable en Blazor e index.html, con animación y tamaño adaptativo.
- Menús desplegables: <li> ocupa el 100% del ancho, bordes redondeados y fondo activo completo.
- Contraste visual mejorado en el fondo de la página de login.
- Sombra más notoria en el menú lateral (desktop y mobile).
- Layouts principales y navegación implementados con Bootstrap 5 y CSS propio, sin MudBlazor en la shell principal.

<purpose>
Sistema de layout con Bootstrap 5.3 y CSS propio, reemplazando completamente MudBlazor. Proporciona topbar fija, navegación horizontal en desktop, drawers CSS puros (izquierdo, derecho, lateral), menú lateral y área de contenido principal.
</purpose>

<requirements>
- Topbar fija con gradiente oscuro/azul, 56px de alto
- Navegación desktop: Bootstrap dropdowns en topbar, <li> de ancho completo y bordes redondeados
- Navegación mobile: drawer izquierdo con Bootstrap collapse para grupos
- Drawer de usuario/cuenta a la derecha
- Side menu opcional: persistente en desktop (aside, con sombra notoria), temporal en mobile (drawer derecho, con sombra notoria)
- Contenido en contenedor con `max-width: 1440px`
- Layout responsive: mobile-first con Bootstrap 5 breakpoints
- Fuente Inter via Google Fonts
- Sin dependencias de MudBlazor ni MudExtensions

<implementation>
Arquitectura:

```
MainLayout.razor
├── <nav class="app-topbar"> (topbar fija)
│   ├── Hamburger button (mobile)
│   ├── <NavMenu /> (brand + dropdowns desktop)
│   └── Account button → toggle right drawer
├── Left drawer (mobile nav)  → <MobileNavMenu />
├── Right drawer (account)    → <MobileUserDrawer />
├── <div class="app-body">
│   ├── <main class="app-main">
│   │   └── <MenuRouteGuard> @Body </MenuRouteGuard>
│   └── Side menu (optional)
│       ├── Desktop: <aside class="app-sidemenu--desktop"> (con sombra)
│       └── Mobile: drawer right + tab button (con sombra)
└── Drawers CSS: transform translateX transition
```

LoginLayout.razor:
```
<div class="login-shell"> (fondo contrastado)
    @Body
</div>
```

NavMenu internals:
- `<span class="app-brand">` para el título
- `AuthorizeView > Authorized`: Bootstrap dropdowns y links por cada MenuDTO
- Menús con hijos: `<div class="dropdown">` + `data-bs-toggle="dropdown"`, <li> de ancho completo y bordes redondeados
- Menús sin hijos: `<a class="nav-menu-btn">`

Key CSS classes:
- `.app-topbar` — barra superior fija
- `.app-body` — flex container post-topbar
- `.app-drawer` — drawers laterales CSS puro
- `.app-sidemenu--desktop` / `.app-sidemenu--mobile` — side menu con sombra
- `.app-modal` / `.app-modal-backdrop` — modales sin JS externo
- `.app-toast` — notificaciones tipo snackbar
- `.login-shell` / `.login-card` — layout de login, fondo contrastado

Key files:
- `Layout/MainLayout.razor` — estructura completa
- `Layout/LoginLayout.razor` — layout centrado para auth, fondo contrastado
- `Layout/NavMenu.razor` — nav horizontal con BS dropdowns y <li> de ancho completo
- `Layout/MobileNavMenu.razor` — nav mobile con BS collapse
- `Layout/MobileUserDrawer.razor` — panel cuenta
- `Layout/MenuRouteGuard.razor` — guard de roles con alert Bootstrap
- `wwwroot/css/app.css` — todos los estilos propios, incluyendo loader, menús, contraste y sombras
- `wwwroot/index.html` — Bootstrap 5.3 CDN links/scripts, loader consistente
- `Program.cs` — sin AddMudServices ni AddMudExtensions
- `_Imports.razor` — sin @using MudBlazor

<testing>
Verification plan:
- Topbar azul/oscuro visible en todas las páginas
- Título "RugbyEngine" en blanco a la izquierda
- Dropdowns funcionan en desktop (requiere Bootstrap JS), <li> de ancho completo y bordes redondeados
- Hamburger abre drawer izquierdo en mobile
- Botón cuenta abre drawer derecho
- Side menu visible cuando hay nivel 1 activo, con sombra notoria
- Login centrado en pantalla, fondo contrastado
- Modales inline se abren/cierran correctamente
- Toasts aparecen y desaparecen en 3.5s
- Loader consistente en Blazor e index.html
