# Login Page

<purpose>
Página de inicio de sesión implementada con componentes nativos de Blazor (`EditForm`) y estilos Bootstrap/CSS propio, con validación, estado de envío y redirección por `returnUrl`.
</purpose>

<requirements>
- Ruta: `/Auth/Login`.
- Página anónima con `[AllowAnonymous]`.
- Layout: `LoginLayout`.
- Formulario con `EditForm` + `DataAnnotationsValidator`.
- Campos: usuario y contraseña.
- Botón submit con spinner durante envío.
- Mensaje de error/success con `alert` Bootstrap.
- Redirección post-login a `returnUrl` o `/`.
</requirements>

<implementation>
Componente:
- `Pages/Auth/Login.razor`

Flujo:
1. `OnInitializedAsync()` lee `returnUrl`.
2. Si ya autenticado (`AuthService.IsAuthenticatedAsync`) redirige.
3. `HandleLoginAsync()` llama `AuthService.LoginAsync`.
4. Si éxito: `NavigateTo(returnUrl ?? "/", true)`.
5. Si falla: muestra mensaje en `alert-danger`.

UI actual:
- `InputText` para usuario y contraseña.
- `button` submit con spinner (`spinner-border`).
- Mensajería con `alert` de Bootstrap.

Notas:
- No usa MudBlazor.
- Mantiene compatibilidad con flujo `returnUrl` codificado.
</implementation>

<testing>
Verification plan:
- Sin autenticación, `/Auth/Login` muestra formulario.
- Login válido redirige a `returnUrl` si existe.
- Login inválido muestra mensaje de error.
- Botón queda deshabilitado durante `_isSubmitting`.
</testing>
