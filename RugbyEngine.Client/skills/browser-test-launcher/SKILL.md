---
name: browser-test-launcher
description: Inicia la ejecución local del cliente y luego abre una sesión de navegador de prueba para RugbyEngine.Client usando herramientas de terminal y browser automation. Usar cuando el usuario pida levantar el browser de prueba, abrir la app o preparar una validación visual.
allowed-tools: run_command_in_terminal, browser_mcp_browser_health_check, browser_mcp_browser_new_page, browser_mcp_browser_navigate, browser_mcp_browser_resize_window, browser_mcp_browser_wait, browser_mcp_browser_read_page, browser_mcp_browser_screenshot
---

# Browser Test Launcher

Inicia la ejecución local del cliente Blazor WebAssembly y luego abre el navegador de prueba sobre RugbyEngine.

## Objetivo

Preparar una sesión visual reproducible sobre el cliente en desarrollo sin depender de que la app ya estuviera levantada.

## URL objetivo

- primaria: `https://localhost:7255`
- fallback: `http://localhost:5268`

## Flujo recomendado

1. Verificar que el navegador de herramientas esté disponible.
2. Iniciar la ejecución local del cliente con el perfil `https` antes de abrir el navegador.
3. Esperar unos segundos para que la app publique las URLs locales.
4. Abrir una nueva pestaña de prueba.
5. Ajustar viewport desktop antes de inspeccionar la UI.
6. Navegar primero a `https://localhost:7255`.
7. Si la URL segura no responde, probar `http://localhost:5268`.
8. Esperar a que la página termine de estabilizarse.
9. Leer la página para confirmar si quedó en login, home o error de carga.

## Resultados esperados

- El navegador de prueba queda abierto sobre la app.
- El cliente queda iniciado localmente antes de la navegación.
- La URL activa coincide con una de las URLs de desarrollo del cliente.
- El estado visible de la página queda resumido antes de seguir con otras automatizaciones.

## Uso sugerido

```text
@workspace browser-test-launcher: abre el browser de prueba del cliente
```

```text
@workspace #file:RugbyEngine.Client/skills/browser-test-launcher/SKILL.md
Abre la app en el navegador de prueba y dime en qué pantalla quedó
```

## Notas

- Esta skill debe iniciar la ejecución local del cliente antes de abrir el browser.
- El perfil esperado es `https`, alineado con `Properties/launchSettings.json` del cliente.
- Si la app no responde en ninguna URL luego del arranque, detener la automatización y reportar que el cliente no quedó disponible.
