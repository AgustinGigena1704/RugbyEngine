# Login Page

<purpose>
Página de inicio de sesión con formulario moderno usando componentes MudBlazor, validación de campos requeridos, feedback visual durante el proceso de autenticación y soporte para redirección post-login.
</purpose>

<requirements>
- Ruta: `/Auth/Login` con atributo `[AllowAnonymous]`
- Formulario centrado y responsive usando `MudGrid` con breakpoints (xs=12, sm=8, md=6, lg=4)
- Card con `MudPaper` (Elevation=4) y padding interno
- Campos `MudTextField` con:
  - Variante `Outlined`
  - Iconos de adorno (Person, Lock)
  - Validación `Required` con mensajes en español
  - `InputType.Password` para contraseña
- Botón `MudButton` con:
  - `Variant.Filled`, `Color.Primary`, `Size.Large`, `FullWidth`
  - `MudProgressCircular` inline cuando `_isSubmitting`
  - Texto dinámico "Ingresar" / "Ingresando..."
- `MudAlert` con `Severity.Error` para errores de autenticación
- Redirección automática si usuario ya autenticado
- Soporte para `returnUrl` en query string
</requirements>

<implementation>
Estructura visual:
```
MudGrid (Justify.Center, mt-8)
└── MudItem (responsive breakpoints)
    └── MudPaper (Elevation=4, pa-6)
        └── MudStack (Spacing=4)
            ├── MudText (h4, Primary) - "RugbyEngine"
            ├── MudText (h6) - "Inicia sesión para continuar"
            ├── MudAlert (condicional, Severity.Error)
            ├── MudTextField (Usuario, Person icon)
            ├── MudTextField (Contraseña, Lock icon)
            └── MudButton (Submit con spinner)
```

Estado del componente:
- `_model: LoginRequest` - DTO con Username y Password
- `_isSubmitting: bool` - Bloquea UI durante login
- `_statusMessage: string?` - Mensaje de error a mostrar
- `_statusSeverity: Severity` - Siempre Error para este caso
- `_returnUrl: string?` - URL para redirección post-login

Flujo de login:
1. `OnInitializedAsync()` extrae `returnUrl` de query string
2. Si ya autenticado (`IsAuthenticatedAsync()`), redirige inmediatamente
3. Usuario completa formulario y hace clic en "Ingresar"
4. `HandleLoginAsync()` activa spinner, llama a `AuthService.LoginAsync()`
5. Si exitoso, redirige a `returnUrl` o "/" con `forceLoad: true`
6. Si fallido, muestra `MudAlert` con mensaje de error

Extracción de returnUrl:
- Parsea `NavigationManager.Uri` para obtener query string
- Busca parámetro `returnUrl` (case-insensitive)
- Decodifica con `Uri.UnescapeDataString()`

Key files:
- `Pages/Auth/Login.razor` - Página completa
- `Models/Auth/LoginRequest.cs` - `{ Username, Password }`
- `Models/Auth/LoginResponse.cs` - `{ Success, Message, Token }`
</implementation>

<testing>
Verification plan:
- Responsive: Card se ajusta a diferentes tamaños (xs=full, lg=4 columnas)
- Campos vacíos: Muestra "El usuario es requerido" / "La contraseña es requerida"
- Spinner: `MudProgressCircular` aparece mientras `_isSubmitting = true`
- Error de auth: `MudAlert` roja con mensaje del servidor
- Login exitoso: Redirige a `/` o `returnUrl` con page reload
- Ya autenticado: Redirige inmediatamente sin mostrar formulario
- returnUrl: `/Auth/Login?returnUrl=%2Fcounter` redirige a `/counter` post-login
- Iconos: Person (usuario) y Lock (contraseña) visibles en los campos
</testing>
