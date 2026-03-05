# MudBlazor Layout System

<purpose>
Sistema de layout moderno usando MudBlazor que proporciona una interfaz de usuario consistente con barra de aplicación superior, navegación horizontal y área de contenido principal con tema personalizado.
</purpose>

<requirements>
- Layout principal con `MudLayout`, `MudAppBar` (elevation 4), y `MudMainContent`
- Tema personalizado definido en código con paleta de colores:
  - Primary: `#1976D2` (azul)
  - Secondary: `#FF4081` (rosa)
  - Background: `#f5f5f5` (gris claro)
  - Surface: `#ffffff` (blanco)
  - AppbarText: `#ffffff` (blanco)
- Tipografía Roboto como font family principal
- Navegación horizontal en la barra superior con `MudButton` y iconos Material
- Contenido renderizado en `MudContainer` con `MaxWidth.Large`
- Providers globales: `MudThemeProvider`, `MudPopoverProvider`, `MudDialogProvider`, `MudSnackbarProvider`
</requirements>

<implementation>
Arquitectura:
- `MainLayout.razor` es el layout base que hereda de `LayoutComponentBase`
- El tema se define como `MudTheme` en el bloque `@code` con `PaletteLight` y `Typography`
- Los providers de MudBlazor son auto-cerrados y se colocan antes del `MudLayout`
- `MudMainContent` tiene clase `mt-16` para compensar el AppBar fijo y `pa-4` para padding

Estructura del Layout:
```
MudThemeProvider (tema personalizado)
MudPopoverProvider
MudDialogProvider  
MudSnackbarProvider
MudLayout
├── MudAppBar (Color.Primary, Elevation=4)
│   └── NavMenu (componente de navegación)
└── MudMainContent (background gris)
    └── MudContainer (MaxWidth.Large)
        └── @Body
```

NavMenu internals:
- `MudText` con Typo.h6 para el título "RugbyEngine"
- `AuthorizeView` para mostrar/ocultar elementos según autenticación
- `Authorized`: Links de navegación + botón "Salir"
- `NotAuthorized`: `MudIconButton` con icono de cuenta
- `MudSpacer` para empujar elementos a la derecha
- `MudButton` con `Color.Inherit` para heredar color del AppBar

Key files:
- `Layout/MainLayout.razor` - Estructura y tema
- `Layout/NavMenu.razor` - Navegación y auth controls
- `wwwroot/index.html` - Referencias CSS/JS de MudBlazor, fuentes Google
- `Program.cs` - `builder.Services.AddMudServices()`
- `_Imports.razor` - `@using MudBlazor`
</implementation>

<testing>
Verification plan:
- AppBar azul (#1976D2) visible en todas las páginas con elevación/sombra
- Título "RugbyEngine" en blanco alineado a la izquierda
- Links de navegación (Inicio, Counter, Weather) solo visibles cuando autenticado
- Icono de cuenta visible cuando no autenticado
- Botón "Salir" a la derecha cuando autenticado
- Fondo gris claro en área de contenido
- Contenido centrado con ancho máximo Large
- Fuente Roboto aplicada en toda la UI
- Dialogs y Snackbars funcionan correctamente (requieren providers)
</testing>
