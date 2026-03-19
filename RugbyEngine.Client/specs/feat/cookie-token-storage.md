# Cookie-based Token Storage

<purpose>
Persistir token JWT del cliente en cookie para mantener sesión entre recargas y reconstruir autenticación en Blazor WASM.
</purpose>

<requirements>
- Cookie principal: `re_access_token`.
- Operaciones CRUD de cookie vía `ICookieService` + JS interop.
- Valor codificado/decodificado en operaciones de lectura/escritura.
- Expiración alineada con `ValidTo` del JWT cuando está disponible.
</requirements>

<implementation>
Componentes:
- `Services/ICookieService.cs`
- `Services/CookieService.cs`
- `wwwroot/js/cookieInterop.js`

Uso principal:
- `AuthService` guarda/elimina token en login/logout.
- `ApiAuthenticationStateProvider` lee token para reconstruir sesión.

Atributos de cookie:
- `path=/`
- `SameSite=Lax`
- `Secure` condicional en HTTPS
</implementation>

<testing>
Verification plan:
- Token se guarda al autenticar.
- Token se elimina al cerrar sesión.
- Sesión persiste tras refresh cuando token es válido.
</testing>
