# RugbyEngine.Client

Documentación operativa del cliente Blazor WebAssembly de RugbyEngine.

Este documento describe **cómo está implementado y configurado** el proyecto `RugbyEngine.Client.csproj` y cómo se conecta con las specs funcionales del cliente.

---

## 1) Identidad del proyecto

- Proyecto: `RugbyEngine.Client`
- SDK: `Microsoft.NET.Sdk.BlazorWebAssembly`
- Target framework: `net10.0`
- Nullable: `enable`
- Implicit usings: `enable`
- Versión del ensamblado: `0.0.1`

Impacto:
- El cliente ejecuta completamente en navegador (WASM).
- El código se compila con referencias nulas estrictas y usings implícitos.

---

## 2) Configuración de compilación y calidad

En `Debug` y `Release`:
- `TreatWarningsAsErrors=True`
- `RunAOTCompilation=False`

A nivel global:
- `EnforceCodeStyleInBuild=True`
- `DebugType=portable`
- `DebugSymbols=True`

Impacto:
- Cualquier warning rompe build (disciplina de calidad alta).
- Se prioriza estabilidad/debug por sobre AOT en el estado actual.
- El análisis de estilo forma parte del build, no solo del IDE.

---

## 3) Activos web y PWA

### 3.1 App settings en `wwwroot`
El `.csproj` elimina explícitamente:
- `wwwroot/appsettings.json`
- `wwwroot/appsettings.Production.json`

Impacto:
- La configuración no se basa en esos archivos empaquetados.
- El cliente depende de configuración leída desde otras fuentes (por ejemplo `IConfiguration` + variables de entorno disponibles en runtime de hosting).

### 3.2 Service Worker
Registrado en el proyecto:
- `wwwroot/service-worker.js`
- `PublishedContent="wwwroot/service-worker.published.js"`

Impacto:
- El cliente está preparado para flujo PWA/publicación con service worker.

### 3.3 Ícono de aplicación
- `ApplicationIcon=wwwroot\images\loadingPelota.ico`

---

## 4) Paquetes NuGet usados

### Runtime/UI Blazor
- `Microsoft.AspNetCore.Components.WebAssembly (10.0.5)`
- `Microsoft.AspNetCore.Components.Authorization (10.0.5)`

### Desarrollo local
- `Microsoft.AspNetCore.Components.WebAssembly.DevServer (10.0.5)` con `PrivateAssets=all`

### Seguridad/Auth
- `System.IdentityModel.Tokens.Jwt (8.16.0)`

### Persistencia adicional
- `Blazor.Storage (5.0.0)`

### Calidad estática
- `SonarAnalyzer.CSharp (10.21.0.135717)` con assets privados (no se propaga a consumidores)

Impacto:
- El análisis Sonar participa en el pipeline local/CI y, combinado con warnings-as-errors, endurece reglas de calidad.

---

## 5) Estructura declarada en el .csproj

- `ProjectReference` a `RugbyEngine.Shared` (contratos DTO/objetos compartidos).
- Carpetas declaradas:
  - `Middleware\`
  - `Pages\Extra\`

Nota:
- Aunque existe la carpeta `Middleware`, el estado actual del cliente usa consumo directo con `HttpClient` en servicios (sin patrón `CreateAuthorizedRequestAsync`).

---

## 6) Relación entre .csproj y comportamiento funcional actual

### 6.1 Comunicación API
- El cliente consume endpoints por `HttpClient` directo.
- Contratos actuales de tabla:
  - `GET api/persona/search?search=&page=&pageSize=`
  - `GET api/persona/count?search=`
  - `GET api/usuario/search?search=&page=&pageSize=`
  - `GET api/usuario/count?search=`

Referencia: `specs/feat/api-communication.md`

### 6.2 Tabla reusable y paginación
- Componente reusable `<Tabla>` con cancelación por `CancellationToken`.
- Contador compacto acumulado `x / x2`.
- Loading overlay durante refresh.

Referencia: `specs/feat/reusable-table-and-server-pagination.md`

### 6.3 Autenticación JWT y rutas protegidas
- Token en cookie `re_access_token`.
- Estado de autenticación sincronizado con `AuthenticationStateProvider`.
- Rutas protegidas con redirección a login y `returnUrl`.

Referencias:
- `specs/feat/jwt-authentication.md`
- `specs/feat/cookie-token-storage.md`
- `specs/feat/route-protection.md`

### 6.4 Layout/UI
- Shell principal Bootstrap + CSS propio (sin MudBlazor en shell).

Referencias:
- `specs/feat/mudblazor-layout.md`
- `specs/feat/login-page.md`

---

## 7) Resumen técnico del estado actual

`RugbyEngine.Client.csproj` define un cliente WASM .NET 10 con:
- reglas estrictas de calidad (warnings-as-errors + Sonar + code style en build),
- soporte PWA (service worker),
- autenticación JWT en cookie,
- consumo API directo por HttpClient,
- UI administrativa con tabla reusable y paginación server-side.

Este documento debe mantenerse alineado con el código y con las specs vinculadas arriba.
