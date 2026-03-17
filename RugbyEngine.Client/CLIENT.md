# RugbyEngine.Client

## Cambios recientes

- Loader reutilizable y consistente en Blazor e index.html, con animación y tamaño adaptativo.
- Menús desplegables: el <li> de cada item ocupa el 100% del ancho, con bordes redondeados y fondo activo que cubre todo el botón.
- Contraste visual mejorado en el fondo de la página de login.
- Sombra más notoria en el menú lateral (desktop y mobile) para mayor separación visual.
- Tablas administrativas responsive, botones de acción grandes y consistentes en desktop/mobile.
- Layouts principales y navegación implementados con Bootstrap 5 y CSS propio, sin MudBlazor en la shell principal.

## Descripción

`RugbyEngine.Client` es el frontend `Blazor WebAssembly` de RugbyEngine. Su responsabilidad es ofrecer la experiencia de usuario de la plataforma, integrando autenticación, navegación dinámica basada en permisos, vistas administrativas, manejo de sesión y una capa visual con **Bootstrap 5** y estilos CSS propios.

---

## Objetivos del cliente

El cliente está diseñado para:

- consumir la API REST de `RugbyEngine.Api`
- persistir y reutilizar la sesión autenticada del usuario
- renderizar menús según permisos efectivos
- proteger rutas desde la UI
- ofrecer una experiencia responsive para `desktop` y `mobile`
- operar sin dependencias de componentes de terceros de UI (solo Bootstrap CDN)

---

## Stack principal

- `.NET 10`
- `Blazor WebAssembly`
- `Bootstrap 5.3` (CDN)
- `JWT` persistido en cookie del navegador
- `JS interop` para cookies y sincronización de sesión
- HTML + CSS propio (`app.css`) sin librerías de UI pesadas

---

## Estructura relevante

### Layout

- `Layout/MainLayout.razor`
  - shell principal: topbar fixed, drawers (izquierdo/derecho/side) CSS puros
  - sin dependencia de MudBlazor
- `Layout/LoginLayout.razor`
  - layout centrado para la página de login, fondo con mayor contraste
- `Layout/NavMenu.razor`
  - navegación superior desktop con Bootstrap dropdowns y menús desplegables con <li> de ancho completo y bordes redondeados
- `Layout/MobileNavMenu.razor`
  - navegación mobile con Bootstrap collapse
- `Layout/MobileUserDrawer.razor`
  - panel de usuario autenticado en drawer derecho
- `Layout/MenuRouteGuard.razor`
  - guarda de roles por ruta con alerta Bootstrap

### Pages

- `Pages/Home/Home.razor`
  - dashboard inicial con hero card
- `Pages/Administracion/Gestion/Usuarios.razor`
  - gestión CRUD de usuarios con tabla Bootstrap, modales CSS y toast
- `Pages/Administracion/Gestion/Personas.razor`
  - gestión CRUD de personas con tabla Bootstrap, modales CSS y toast
- `Pages/Account/Profile.razor`
  - perfil del usuario con gestión de perfiles
- `Pages/Utility/MenuLateral.razor`
  - menú secundario lateral con búsqueda en input nativo
- `Pages/Auth/Login.razor`
  - formulario de login con `EditForm` nativo de Blazor, fondo con mayor contraste
- `Pages/Auth/NotAuth.razor`
  - página de acceso denegado

### Services

- `AuthService` / `IAuthService`
- `ApiAuthenticationStateProvider`
- `MainMenuService`
- `CookieService` / `ICookieService`
- `PersonaService`, `UsuarioService`, `PerfilService`

### Assets y estilos

- `wwwroot/css/app.css`
  - variables CSS propias, topbar, drawers, side menu (con sombra mejorada), modales, toasts, login (fondo contrastado), tablas, botones
  - responsive mobile/desktop sin dependencias de framework de UI
- `wwwroot/js/cookieInterop.js` — lectura/escritura de cookies
- `wwwroot/js/sessionMonitor.js` — sincronización de sesión entre pestañas
- `wwwroot/index.html` — Bootstrap 5.3 CSS+JS via CDN, Inter font, loader consistente

---

## Funcionamiento general del cliente

## 1. Inicio de la aplicación

Cuando se carga el cliente:

1. Blazor inicializa `App` y el árbol de componentes.
2. `Program.cs` registra dependencias, clientes HTTP y servicios de autenticación.
3. `MainLayout` monta el shell visual general.
4. `ApiAuthenticationStateProvider` reconstruye el usuario actual usando el token guardado en cookie.
5. `MainMenuService` obtiene el árbol de navegación permitido desde la API.
6. Las vistas protegidas se renderizan según autenticación y permisos.

---

## 2. Autenticación

La autenticación del cliente se basa en un `JWT` persistido en cookie.

### Flujo resumido

1. el usuario inicia sesión
2. `AuthService` llama a `api/Auth/Login`
3. la API devuelve el token
4. el cliente guarda el token en cookie
5. `ApiAuthenticationStateProvider` convierte el token en `ClaimsPrincipal`
6. la UI usa esos claims para autorización y visibilidad

### Cookie de acceso

La cookie principal del acceso es:

- `re_access_token`

La persistencia se realiza mediante:

- `CookieService`
- `cookieInterop.js`

### Automatización asistida del login

Para pruebas guiadas desde Copilot Chat existen dos skills complementarias:

- `browser-test-launcher`
  - inicia la ejecución local del cliente con el perfil `https`
  - abre la aplicación en `https://localhost:7255`
  - usa `http://localhost:5268` como fallback local
- `browser-test-login`
  - navega a `/Auth/Login`
  - escribe credenciales en inputs separados de usuario y contraseña
  - dispara el botón `Ingresar` y deja la sesión lista para continuar validaciones

Regla operativa: en cualquier automatización de login del workspace, el usuario y la contraseña deben cargarse en inputs distintos y nunca concatenarse en un mismo campo.

---

## 3. Menús y navegación

La navegación del cliente no es estática: depende del usuario autenticado y de sus permisos.

### Fuente de verdad

El árbol de menús y el mapa de rutas/roles provienen de:

- `api/Menu`
- `api/Menu/RouteRoles`

### Servicio principal

`MainMenuService`:

- obtiene el árbol desde la API
- lo cachea
- detecta cuál es el menú activo según la ruta
- expone eventos para refrescar layout y menús laterales

### Composición visual

#### Desktop

- `NavMenu.razor` renderiza el nivel principal en la app bar
- `MenuLateral.razor` muestra navegación secundaria contextual
- la navegación superior desktop debe renderizarse solo en `md+`
- los popovers superiores usan una superficie propia con más contraste, mejor espaciado y estados hover consistentes
- los popovers superiores deben respetar el ancho disponible del contenedor para evitar desbordes cuando cambia el tamaño del activador
- los botones del menú superior deben resolver el hover sobre una sola capa visual para evitar dobles tonos
- los items desplegados por el menú superior deben heredar el mismo ancho visual que el botón padre

#### Mobile

- `MobileNavMenu.razor` renderiza grupos colapsables
- el menú lateral derecho puede abrirse con drawer temporal
- el usuario tiene un drawer propio con acciones de cuenta
- el menú desktop no debe renderizarse en mobile
- el menú lateral debe mostrar primero el buscador y luego los items, anclados arriba del drawer
- el shell mobile debe permitir scroll vertical del contenido y del drawer lateral sin bloquear el documento completo
- el drawer lateral debe mantener todos sus contenedores internos alineados arriba también en desktop

---

## 4. Protección de rutas

La UI protege rutas en dos niveles:

### a. autenticación

Se usa:

- `[Authorize]`
- `[Authorize(Roles = "...")]`
- `AuthorizeView`

### b. permisos de menú

Se usa además:

- `MenuRouteGuard`

Este componente consulta el mapa `ruta -> rol` y evita que el usuario vea contenido de una ruta que no le corresponde aunque intente navegar manualmente.

---

## 5. Sistema visual general

El cliente usa `MudBlazor` como base, pero aplica una capa propia de diseño en `wwwroot/css/app.css`.

### Objetivos del sistema visual

- mejorar contraste entre shell, contenido y superficies
- dar una estética más moderna
- mantener coherencia entre desktop y mobile
- permitir convivir tema claro y oscuro
- estilizar navegación, tablas, formularios y overlays con la misma identidad visual
- evitar que el modo oscuro use negros absolutos cuando eso reduzca la separación visual entre capas

### Capas visuales principales

#### Shell

Incluye:

- app bar superior
- fondo general del layout
- contenedor principal del contenido
- drawers laterales

#### Superficies

Incluye:

- tarjetas tipo `hero`
- secciones administrativas
- tablas
- diálogos
- popovers y desplegables

#### Interacciones

Incluye:

- estados hover
- relieve visual con sombras
- bordes suaves y radios consistentes
- botones de acción uniformes
- links laterales y accesos del drawer deben conservar bordes redondeados aun fuera de hover

---

## Sistema de tema claro / oscuro

## Objetivo

El sistema de tema permite alternar entre:

- tema claro
- tema oscuro

La preferencia debe:

- cambiar el comportamiento visual de `MudBlazor`
- afectar los estilos CSS custom del proyecto
- persistirse entre sesiones

---

## Componentes involucrados

### 1. `MainLayout.razor`

Es el orquestador principal del tema.

Responsabilidades:

- declarar el `MudThemeProvider`
- mantener el estado `_isDarkMode`
- leer preferencia previa desde cookie
- guardar el cambio del usuario
- sincronizar el estado visual global

### 2. `MudThemeProvider`

Se configura con:

- `Theme="_theme"`
- `@bind-IsDarkMode="_isDarkMode"`

Esto permite que `MudBlazor` use:

- `PaletteLight`
- `PaletteDark`

según el valor del estado actual.

### 3. `CookieService`

Guarda la preferencia del usuario en cookie.

Cookie utilizada:

- `re_theme`

Valores esperados:

- `light`
- `dark`

### 4. `cookieInterop.js`

Además de operar cookies, expone una función para aplicar visualmente el tema en el `body`:

- `setBodyTheme(isDarkMode)`

Esto es necesario porque parte del diseño custom del proyecto depende de selectores CSS como:

- `body.dark-theme ...`

### 5. `app.css`

Contiene reglas para:

- claro por defecto
- oscuro bajo `body.dark-theme`

Esto permite que no solo cambie `MudBlazor`, sino también:

- fondos generales
- drawers
- heroes
- tablas
- popovers
- inputs
- hover states

---

## Flujo completo del tema

## Al iniciar la app

1. `MainLayout` se inicializa.
2. Se lee la cookie `re_theme` con `CookieService`.
3. Si la cookie vale `dark`, `_isDarkMode = true`.
4. `MudThemeProvider` usa la paleta oscura.
5. En `OnAfterRenderAsync`, se llama a `cookieInterop.setBodyTheme(_isDarkMode)`.
6. `body` recibe o pierde la clase `dark-theme`.
7. Los estilos CSS custom reaccionan a esa clase.

## Al hacer click en el botón de tema

1. el usuario pulsa el ícono de luna/sol en la app bar
2. `ToggleThemeAsync()` invierte `_isDarkMode`
3. se guarda la cookie `re_theme`
4. se llama a `setBodyTheme(...)`
5. se fuerza `StateHasChanged()`
6. el layout y los estilos se actualizan inmediatamente

---

## Personalización del tema

El tema está definido en `MainLayout.razor` dentro de `MudTheme _theme`.

### Tema claro

Define, entre otras cosas:

- `Primary`
- `Secondary`
- `Background`
- `Surface`
- `DrawerBackground`
- `TextPrimary`
- `TextSecondary`
- `Divider`

### Tema oscuro

Define su contraparte con contraste optimizado para fondos oscuros.

### Otras propiedades

#### `LayoutProperties`

- altura de app bar
- border radius por defecto

#### `Shadows`

Sombras personalizadas para una estética más moderna y con mayor profundidad visual.

#### `Typography`

Fuente principal:

- `Inter`
- fallback a `Roboto`, `Helvetica`, `Arial`, `sans-serif`

---

## Reglas de desarrollo para el sistema de tema

Al extender o modificar el cliente, conviene respetar estas reglas:

### 1. no depender solo de MudBlazor

Si un componente usa estilos custom, debe contemplar variantes para:

- claro
- oscuro

Preferentemente con selectores como:

- `body.dark-theme .mi-clase { ... }`

### 2. no hardcodear colores aislados

Cuando sea posible, usar:

- variables del palette de MudBlazor
- tokens visuales definidos en `app.css`

Ejemplos:

- `var(--mud-palette-text-primary)`
- `var(--mud-palette-divider)`
- variables `--app-*`

### 3. mantener jerarquía visual consistente

Las nuevas vistas deberían usar el sistema existente:

- `page-modern`
- `page-modern__hero`
- `page-modern__section`
- `page-modern__search`
- `page-modern__primary-action`

### 4. sincronizar siempre CSS + tema MudBlazor

Si se agrega una nueva parte visual que depende del tema, validar que:

- responda al `MudThemeProvider`
- y/o responda a `body.dark-theme`

### 5. persistencia

Toda preferencia visual del usuario que deba sobrevivir recargas debe guardarse con una estrategia compatible con WASM; en este proyecto se usa cookie mediante `CookieService`.

---

## Menús desplegables

Los menús desplegables superiores están implementados con `MudMenu`.

### Objetivo visual

Deben verse:

- más modernos
- más contrastados
- mejor separados del app bar
- coherentes con tema claro y oscuro

### Estilos asociados

Se aplican clases específicas como:

- `app-nav-popover`
- `app-nav-popover__list`
- `app-nav-popover__item`

Esto permite controlar:

- fondo del popover
- radios
- padding
- sombra
- hover
- contraste del texto
- mejor integración con la app bar en desktop

### Navegación superior desktop

La app bar superior distingue visualmente tres capas:

- marca
- acciones de navegación principales
- acciones globales como tema y usuario

Las entradas superiores deben:

- verse como controles de primer nivel
- tener spacing consistente
- mostrar estado activo claro
- mantener buena legibilidad sobre la app bar

### Menú lateral contextual

El menú lateral contextual debe mantener esta estructura visual:

1. buscador arriba
2. divisor
3. lista de opciones debajo

No debe:

- empujar los items al fondo del drawer
- depender de distribución vertical con espacio sobrante
- mezclar comportamiento desktop y mobile de forma implícita

---

## Tablas administrativas

Las tablas administrativas siguen reglas de UX ya establecidas en el proyecto.

### Reglas actuales

- deben ser responsive para mobile y desktop
- evitar tamaños fijos hardcodeados en estructura general de tablas
- usar virtualización y scroll interno cuando aplique
- paginación por defecto de 10 con opciones hasta 50 cuando se implemente
- los botones de acción deben:
  - ser más grandes
  - mantener el mismo tamaño
  - mostrar texto en desktop
  - verse más grandes en mobile

### Clases visuales relacionadas

- `admin-modern-table`
- `admin-table-actions`
- `admin-table-action-button`
- `admin-table-action-icon`

---

## Cómo extender el cliente correctamente

### Si agregas una nueva vista

Recomendado:

1. crear un hero superior con título y subtítulo
2. envolver bloques funcionales en superficies modernas
3. reutilizar clases del sistema visual
4. validar claro y oscuro
5. validar mobile y desktop

### Si agregas un nuevo menú

Recomendado:

1. respetar el árbol entregado por `MainMenuService`
2. no hardcodear navegación paralela si ya existe en API
3. si usas desplegables o popovers, aplicar clases del sistema visual

### Si agregas una nueva preferencia visual

Recomendado:

1. persistirla con `CookieService`
2. sincronizarla con CSS global si afecta estructura o apariencia general
3. documentarla aquí en `CLIENT.md`

---

## Checklist rápido para cambios visuales

Antes de dar por terminado un cambio de UI, verificar:

- ¿se ve bien en desktop?
- ¿se ve bien en mobile?
- ¿tiene contraste suficiente?
- ¿funciona en tema claro?
- ¿funciona en tema oscuro?
- ¿usa clases reutilizables y no estilos inline innecesarios?
- ¿mantiene consistencia con shell, secciones y tablas existentes?

---

## Resumen

`RugbyEngine.Client` combina:

- autenticación basada en JWT en cookie
- navegación dinámica por permisos
- protección de rutas
- componentes MudBlazor
- sistema visual propio
- tema claro/oscuro persistente

El objetivo del frontend no es solo renderizar datos, sino ofrecer una experiencia administrativa consistente, moderna, responsive y fácil de extender.
