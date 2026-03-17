---
name: browser-test-login
description: Automatiza el inicio de sesión de RugbyEngine.Client dentro del navegador de prueba. Usar cuando el usuario pida iniciar sesión, validar credenciales o dejar una sesión autenticada lista para continuar pruebas.
allowed-tools: browser_mcp_browser_navigate, browser_mcp_browser_wait_for_selector, browser_mcp_browser_focus, browser_mcp_browser_type, browser_mcp_browser_click, browser_mcp_browser_press_key, browser_mcp_browser_wait, browser_mcp_browser_read_page, browser_mcp_browser_get_text, browser_mcp_browser_screenshot
---

# Browser Test Login

Automatiza el login del cliente Blazor en la ruta `/Auth/Login`.

## Precondiciones

- Ya existe una pestaña abierta con la app del cliente.
- El usuario aporta `username` y `password`.
- Si la app no está abierta todavía, usar primero `browser-test-launcher`.

## Selectores base

- usuario: `input[type='text']`
- contraseña: `input[type='password']`
- acción principal: `button:has-text("Ingresar")`

## Flujo recomendado

1. Navegar a `/Auth/Login` si la página actual no está en login.
2. Esperar a que existan ambos inputs del formulario.
3. Cargar el usuario en `input[type='text']`.
4. Cargar la contraseña en `input[type='password']`.
5. Verificar que usuario y contraseña quedaron en campos separados.
6. Activar el botón `Ingresar`.
7. Esperar navegación o cambio visual posterior al login.

## Regla obligatoria de automatización

Cuando se ejecute esta skill, el usuario y la contraseña deben escribirse en inputs distintos. Nunca concatenar ambos valores en un mismo campo ni reutilizar el selector del usuario para la contraseña.

## Señales de éxito

- La pantalla deja `/Auth/Login` o muestra layout autenticado.
- No aparece alerta de error de autenticación.
- El flujo queda listo para continuar con validaciones funcionales.

## Señales de fallo

- La ruta sigue en `/Auth/Login` con alerta visible.
- Falta uno de los dos inputs esperados.
- La app no responde luego del submit.

## Uso sugerido

```text
@workspace browser-test-login: inicia sesión con el usuario admin y la contraseña secreta
```

```text
@workspace #file:RugbyEngine.Client/skills/browser-test-login/SKILL.md
Abre login y autentica una sesión de prueba con las credenciales que te pase
```
