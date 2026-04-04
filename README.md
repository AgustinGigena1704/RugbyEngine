# RugbyEngine

## Descripción general

`RugbyEngine` es una solución `full stack` en `.NET 10` orientada a la gestión operativa de una institución de rugby. El repositorio está organizado como monorepo y actualmente incluye:

- una API REST en `ASP.NET Core` con `Entity Framework Core` y autenticación `JWT`
- un cliente `Blazor WebAssembly` con `MudBlazor`
- una librería compartida con DTOs y contratos comunes

La solución implementa hoy el núcleo de autenticación, autorización por roles/permisos, menú dinámico, gestión de personas, gestión de usuarios y asignación de perfiles.

---

## Estructura de la solución

| Proyecto | Tipo | Responsabilidad |
|---|---|---|
| `RugbyEngine.Api` | `ASP.NET Core Web API` | Backend, autenticación, datos, repositorios, servicios y publicación final del cliente WASM |
| `RugbyEngine.Client` | `Blazor WebAssembly` | Interfaz de usuario, navegación, sesión, consumo de API y experiencia responsive/PWA |
| `RugbyEngine.Shared` | Biblioteca de clases | DTOs, respuestas y contratos compartidos entre cliente y servidor |

### Carpetas relevantes

#### `RugbyEngine.Api`

- `Controllers/`: endpoints HTTP
- `Data/`: `DbContext`, entidades, repositorios, `EntityManager` y extensiones
- `Services/`: autenticación, hashing, usuario actual y sesión
- `Middleware/`: refresco automático de JWT
- `Migrations/`: migraciones de `EF Core`
- `Dockerfile`, `docker-compose.yml`, `Caddyfile`: despliegue

#### `RugbyEngine.Client`

- `Pages/`: páginas funcionales
- `Layout/`: layouts, navegación superior, drawers móviles y guardas de ruta
- `Services/`: clientes HTTP y manejo de autenticación
- `wwwroot/`: `index.html`, `manifest`, `service-worker` y recursos estáticos

#### `RugbyEngine.Shared`

- `Auth/`: login y respuesta de autenticación
- `Personas/`: alta/edición/listado de personas
- `Usuarios/`: alta/edición/listado de usuarios
- `Perfiles/`: perfiles de acceso
- `Menus/`: menú jerárquico y mapeo ruta/rol
- `Health/`: respuesta del health check

---

## Stack tecnológico

### Backend

- `.NET 10`
- `ASP.NET Core`
- `Entity Framework Core 10`
- `Npgsql` para PostgreSQL
- `JWT Bearer Authentication`
- `BCrypt.Net-Next` para hashing de contraseñas
- `Swagger` + `Scalar` para documentación interactiva

### Frontend

- `Blazor WebAssembly`
- `MudBlazor`
- `CodeBeam.MudBlazor.Extensions`
- autenticación basada en `JWT` persistido en cookie del navegador

### Calidad y automatización

- analyzers habilitados
- `TreatWarningsAsErrors`
- `SonarAnalyzer.CSharp`
- `dotnet format` en CI
- auditoría de paquetes vulnerables en GitHub Actions

---

## Arquitectura funcional

### Flujo general

1. El usuario inicia sesión desde `RugbyEngine.Client`.
2. `AuthService` llama a `api/Auth/Login`.
3. La API valida credenciales con `SessionService` y genera un token con `JwtService`.
4. El cliente guarda el token en la cookie `re_access_token`.
5. `ApiAuthenticationStateProvider` reconstruye la identidad a partir del JWT.
6. `MainMenuService` consulta `api/Menu` y `api/Menu/RouteRoles`.
7. La UI muestra solo menús y rutas habilitadas según los permisos incluidos en el token.
8. `SessionMonitor` revalida/refresca la sesión al reanudar la aplicación.

### Modelo de acceso

La seguridad está organizada en tres niveles:

- `Usuario`: credenciales y vínculo con una `Persona`
- `Perfil`: agrupador funcional asignable a usuarios
- `Permiso`: rol/código utilizado para autorización y filtrado de menús

Los menús pueden tener un `PermisoId`, lo que permite:

- ocultar opciones no autorizadas
- proteger rutas en cliente con `MenuRouteGuard`
- autorizar endpoints en servidor con `[Authorize]` y `[Authorize(Roles = "...")]`

---

## Proyecto `RugbyEngine.Api`

### Inicialización de la aplicación

`Program.cs` configura:

- controladores con serialización JSON ignorando ciclos
- `CORS` abierto
- `DbContext` con `UseLazyLoadingProxies()` y `UseNpgsql(...)`
- autenticación y autorización JWT
- registro automático de repositorios mediante `AddRepositories()`
- servicios de sesión, JWT, hashing y usuario actual
- prueba de conexión a base de datos al iniciar
- aplicación automática de migraciones con `Database.MigrateAsync()`
- publicación de `Swagger` y `Scalar` en `/swagger` y `/docs`
- `middleware` de refresh automático de token
- archivos estáticos del cliente Blazor publicados en `wwwroot`
- `fallback` SPA a `index.html`

### Controladores disponibles

| Controlador | Ruta base | Responsabilidad |
|---|---|---|
| `AuthController` | `api/Auth` | login, logout, validación y refresh de token |
| `HealthController` | `api/Health` | estado de salud de la aplicación |
| `MenuController` | `api/Menu` | árbol de menú del usuario y mapeo ruta/rol |
| `PersonaController` | `api/Persona` | ABM lógico de personas |
| `PerfilController` | `api/Perfil` | consulta y asignación de perfiles |
| `UsuarioController` | `api/Usuario` | ABM lógico de usuarios y asignación de perfiles |

### Endpoints principales

#### Autenticación

| Método | Ruta | Auth | Descripción |
|---|---|---|---|
| `POST` | `api/Auth/Login` | No | Autentica usuario y devuelve JWT |
| `POST` | `api/Auth/LogOut` | Sí | Cierre lógico de sesión del lado cliente |
| `GET` | `api/Auth/Validate` | Sí | Verifica que el token actual sea válido |
| `POST` | `api/Auth/Refresh` | Sí | Refresca el JWT si está próximo a vencer |

#### Salud

| Método | Ruta | Auth | Descripción |
|---|---|---|---|
| `GET` | `api/Health` | No | Health check para monitoreo |

#### Personas

| Método | Ruta | Auth | Descripción |
|---|---|---|---|
| `GET` | `api/Persona` | Sí | Lista personas activas |
| `GET` | `api/Persona/{id}` | Sí | Obtiene una persona |
| `POST` | `api/Persona` | Sí | Crea una persona |
| `PUT` | `api/Persona/{id}` | Sí | Actualiza una persona |
| `DELETE` | `api/Persona/{id}` | Sí | Baja lógica de persona |

#### Perfiles

| Método | Ruta | Auth | Descripción |
|---|---|---|---|
| `GET` | `api/Perfil` | `admin` | Lista todos los perfiles |
| `GET` | `api/Perfil/mine` | Sí | Lista perfiles del usuario autenticado |
| `POST` | `api/Perfil/{id}/assign` | `admin` | Asigna perfil al usuario autenticado |
| `DELETE` | `api/Perfil/{id}/unassign` | `admin` | Retira perfil del usuario autenticado |

#### Usuarios

| Método | Ruta | Auth | Descripción |
|---|---|---|---|
| `GET` | `api/Usuario` | `admin` | Lista usuarios activos |
| `GET` | `api/Usuario/{id}` | `admin` | Obtiene usuario con persona y perfiles |
| `POST` | `api/Usuario` | `admin` | Crea usuario |
| `PUT` | `api/Usuario/{id}` | `admin` | Edita usuario |
| `DELETE` | `api/Usuario/{id}` | `admin` | Baja lógica de usuario |
| `POST` | `api/Usuario/{usuarioId}/perfiles/{perfilId}` | `admin` | Asigna perfil a usuario |
| `DELETE` | `api/Usuario/{usuarioId}/perfiles/{perfilId}` | `admin` | Retira perfil de usuario |

#### Menús

| Método | Ruta | Auth | Descripción |
|---|---|---|---|
| `GET` | `api/Menu` | Implícita por usuario actual | Devuelve árbol de navegación permitido |
| `GET` | `api/Menu/RouteRoles` | Sí | Devuelve el rol requerido por ruta |

> Nota: `MenuController.GetMenuTree()` depende de `GetCurrentUserAsync()`. En la práctica está pensado para usuarios autenticados.

### Servicios principales

| Servicio | Función |
|---|---|
| `SessionService` | valida credenciales, migra contraseñas antiguas a `BCrypt` y actualiza `LastLogin` |
| `JwtService` | genera, valida y refresca tokens JWT; agrega claims de roles desde permisos |
| `CurrentUserService` | obtiene el usuario autenticado desde el `HttpContext` y la base de datos |
| `PasswordHasher` | encapsula hashing/verificación con `BCrypt` |

### Middleware

`JwtRefreshMiddleware` inspecciona el header `Authorization`, determina si el token necesita renovación y devuelve el token vigente o refrescado en el header `jwt-session`.

### Acceso a datos

#### `ApiDbContext`

Expone los siguientes `DbSet`:

- `Personas`
- `Usuarios`
- `Perfiles`
- `Permisos`
- `Menus`
- `Cuentas`
- `PersonaCuentas`
- `TiposMovimiento`
- `Movimientos`
- `MovimientoItems`
- `TiposEvento`
- `Eventos`
- `EventoMovimientos`
- `EventoCuentas`

#### Entidades centrales ya activas en la aplicación

| Entidad | Descripción |
|---|---|
| `Persona` | datos personales, documento, cobertura, contacto y domicilio |
| `Usuario` | credenciales, email, último acceso y vínculo con `Persona` |
| `Perfil` | agrupación funcional de acceso |
| `Permiso` | rol/código utilizado en autorización y menús |
| `Menu` | opción de navegación jerárquica con permiso opcional |

#### Entidades financieras/eventos modeladas

El contexto ya incluye entidades para evolución funcional futura:

- `Cuenta`
- `PersonaCuenta`
- `TipoMovimiento`
- `Movimiento`
- `MovimientoItem`
- `TipoEvento`
- `Evento`
- `EventoMovimientos`
- `EventoCuenta`

Actualmente esas entidades están modeladas y mapeadas, pero la superficie funcional visible del sistema todavía se concentra en autenticación, administración y navegación.

### Repositorios

La capa de persistencia utiliza:

- `GenericRepository<TEntity>` para operaciones comunes de alta, consulta, modificación y baja lógica
- repositorios específicos declarados con `[Repository(...)]`
- `EntityManager` para resolver dinámicamente repositorios concretos desde DI

Repositorios específicos implementados:

| Repositorio | Especialización |
|---|---|
| `PersonaRepository` | búsquedas por documento y nombre |
| `UsuarioRepository` | búsquedas por username y validación de `PersonaId` único |
| `PerfilRepository` | obtención/asignación/retiro de perfiles por usuario |
| `MenuRepository` | árbol de menú por permisos y mapeo ruta/rol |

### Auditoría y baja lógica

Las entidades que implementan `IAudithory` registran:

- `CreatedById`, `CreatedAt`
- `UpdatedById`, `UpdatedAt`
- `DeletedById`, `DeletedAt`
- `BorradoLogico`

`ApiDbContext.ConfigureAuditoryEntities()` aplica estas relaciones de forma automática mediante reflexión para todas las entidades auditables derivadas de `GenericEntity`.

---

## Proyecto `RugbyEngine.Client`

### Descripción

El cliente es una SPA en `Blazor WebAssembly` con componentes `MudBlazor`, autenticación basada en JWT, layout responsive y soporte base para experiencia tipo PWA.

### Composición principal

| Archivo / componente | Función |
|---|---|
| `App.razor` | configura router, `CascadingAuthenticationState` y redirección a login |
| `MainLayout.razor` | layout principal, `AppBar`, drawers y contenedor principal |
| `NavMenu.razor` | menú superior desktop basado en árbol remoto |
| `MobileNavMenu.razor` | navegación móvil |
| `MobileUserDrawer.razor` | acciones de cuenta del usuario |
| `MenuRouteGuard.razor` | bloquea visualmente rutas sin rol requerido |
| `SessionMonitor.razor` | revalida sesión al reanudar la app |

### Páginas actuales

| Página | Ruta | Estado |
|---|---|---|
| `Login.razor` | `/Auth/Login` | implementada |
| `Home.razor` | `/` | implementada |
| `Profile.razor` | `/account/profile` | implementada |
| `Personas.razor` | `/Admin/Registro/Personas` | implementada |
| `Usuarios.razor` | `/Admin/Registro/Usuarios` | implementada |
| `Pagos.razor` | módulo de tesorería | placeholder inicial |

### Servicios cliente

| Servicio | Responsabilidad |
|---|---|
| `AuthService` | login, logout, persistencia del JWT, refresh y sesión |
| `ApiAuthenticationStateProvider` | genera el `ClaimsPrincipal` desde el token almacenado |
| `MainMenuService` | obtiene y cachea el árbol de menús y roles por ruta |
| `PersonaService` | consumo de `api/Persona` |
| `PerfilService` | consumo de `api/Perfil` |
| `UsuarioService` | consumo de `api/Usuario` |
| `CookieService` | lectura/escritura de cookies vía JS interop |

### Navegación y permisos

- el menú superior se genera dinámicamente desde la API
- el menú lateral se activa según el nodo actual del árbol
- `MenuRouteGuard` consulta el mapa `ruta -> rol`
- la UI utiliza `AuthorizeView` y roles del JWT

### Responsive y PWA

- `MainLayout` adapta la navegación según breakpoint
- existe `manifest.webmanifest`
- el `service-worker` de desarrollo no cachea offline para facilitar iteración
- la aplicación define `theme-color` y metadatos móviles en `wwwroot/index.html`

---

## Proyecto `RugbyEngine.Shared`

### Contratos comunes

| Archivo | Propósito |
|---|---|
| `ApiResponse` | respuesta simple de éxito/error |
| `Auth/LoginDto` | credenciales de login |
| `Auth/LoginResponse` | resultado del login o refresh |
| `Personas/PersonaRequest` | alta/edición de persona |
| `Personas/PersonaResponse` | salida de persona |
| `Perfiles/PerfilDto` | perfil resumido |
| `Menus/MenuDTO` | nodo de menú jerárquico |
| `Menus/RouteRoleDto` | relación ruta/rol |
| `Usuarios/UsuarioCreateDto` | alta de usuario |
| `Usuarios/UsuarioUpdateDto` | edición de usuario |
| `Usuarios/UsuarioResponse` | salida de usuario con perfiles |
| `Health/HealthResponse` | respuesta del health check |

### Validaciones destacadas

Los DTOs incluyen validaciones con `DataAnnotations` para:

- obligatoriedad de campos críticos
- longitud máxima de textos
- validación de email
- longitud mínima de contraseña
- selección obligatoria de persona asociada a un usuario

---

## Configuración

### Variables de entorno soportadas por la API

| Variable | Descripción |
|---|---|
| `DB_HOST` | host de PostgreSQL |
| `DB_PORT` | puerto de PostgreSQL |
| `DB_NAME` | nombre de base |
| `DB_USER` | usuario de base |
| `DB_PASS` | contraseña de base |
| `DB_CERTIFICATE_PATH` | ruta al certificado raíz para conexión SSL |
| `JWT_ISSUER` | issuer del token |
| `JWT_AUDIENCE` | audience del token |
| `JWT_SECRET_KEY` | clave simétrica del token |
| `JWT_EXPIRATION_MINUTES` | duración del token |
| `JWT_KEEP_ALIVE_MINUTES` | umbral de refresh |
| `ASPNETCORE_ENVIRONMENT` | ambiente de ejecución |

### Configuración del cliente

El cliente utiliza estas claves para determinar la URL base de la API:

- `API_BASE_URL`
- `Api:BaseUrl`

### Recomendación de seguridad

Para desarrollo y producción, usar secretos reales por `User Secrets`, variables de entorno o archivos externos no versionados. No documentar ni reutilizar claves sensibles reales en el repositorio.

---

## Ejecución local

### Requisitos

- `SDK .NET 10`
- acceso a una base `PostgreSQL`
- certificado de base si el servidor lo requiere

### Restaurar dependencias

```powershell
dotnet restore RugbyEngine.slnx
```

### Ejecutar la API

```powershell
dotnet run --project RugbyEngine.Api/RugbyEngine.Api.csproj
```

Perfiles de inicio disponibles en `launchSettings.json`:

- `http` → `http://localhost:5000`
- `https` → `https://localhost:7083` y `http://localhost:5000`
- `background` → sin abrir navegador

### Ejecutar el cliente en desarrollo

```powershell
dotnet run --project RugbyEngine.Client/RugbyEngine.Client.csproj
```

Si el cliente se ejecuta por separado, configurar `API_BASE_URL` apuntando a la URL de la API.

### Migraciones

La API intenta aplicar migraciones automáticamente al iniciar si puede conectarse a la base. Para administrar migraciones manualmente:

```powershell
dotnet ef migrations add NombreMigracion --project RugbyEngine.Api --startup-project RugbyEngine.Api
dotnet ef database update --project RugbyEngine.Api --startup-project RugbyEngine.Api
```

---

## Documentación interactiva de la API

Una vez iniciada la API:

- `Swagger`: `https://localhost:7083/swagger`
- `Scalar`: `https://localhost:7083/docs`
- `Health`: `https://localhost:7083/api/Health`

---

## Despliegue

### Docker

El `Dockerfile` utiliza tres etapas:

1. publicación del cliente `Blazor WebAssembly`
2. publicación de la API
3. imagen final `aspnet` con el cliente copiado en `wwwroot`

### Docker Compose

`docker-compose.yml` expone la app en `127.0.0.1:8080` y monta el certificado de base en `/app/Secrets/db.pem`.

### Reverse proxy

`Caddyfile` publica el dominio `rugbyengine.agigena.com` y reenvía tráfico a `localhost:8080`, propagando headers `X-Forwarded-*`.

### GitHub Actions

#### `static-analysis.yml`

Realiza:

- `restore`
- `build` en `Release`
- análisis con Roslyn/SonarAnalyzer
- auditoría de paquetes vulnerables
- verificación de formato con `dotnet format`

#### `deploy.yml`

Realiza:

- build y push de imagen a `GHCR`
- copia de `docker-compose.yml` y `Caddyfile` a la VM
- `docker compose pull` + `up -d`
- recarga de `Caddy`

---

## Estado funcional actual

### Implementado

- autenticación JWT
- refresh de sesión
- autorización por roles/permisos
- menús jerárquicos por usuario
- ABM de personas
- ABM de usuarios
- asignación y retiro de perfiles
- layout responsive para desktop y mobile
- base PWA
- despliegue automatizado

### Modelado pero no expuesto completamente en UI/API

- cuentas
- movimientos
- eventos
- parte del módulo de tesorería

---

## Observaciones de diseño

- la API y el cliente comparten contratos a través de `RugbyEngine.Shared`
- la publicación productiva sirve el cliente estático desde la propia API
- el sistema usa baja lógica en entidades auditables
- los permisos también gobiernan la navegación, no solo los endpoints
- el cliente rehidrata autenticación desde cookie y no desde almacenamiento local

---

## Próximos puntos naturales de evolución

- completar módulos de tesorería y eventos
- ampliar tests automáticos
- endurecer configuración de `CORS` para producción
- centralizar manejo de errores en cliente y servidor
- separar documentación pública/operativa/arquitectónica si el proyecto sigue creciendo
