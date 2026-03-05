# Cookie-based Token Storage

<purpose>
Sistema de almacenamiento de tokens JWT en cookies del navegador usando JavaScript Interop, proporcionando persistencia de sesión entre recargas de página con encoding seguro y expiración sincronizada.
</purpose>

<requirements>
- Los tokens JWT se almacenan en cookies con nombre constante `re_access_token`
- La expiración de la cookie se sincroniza con el claim `ValidTo` del JWT
- Valores de cookie codificados con `Uri.EscapeDataString()` para caracteres especiales
- Cookies configuradas con `path=/`, `SameSite=Lax`, y `Secure` condicional (solo HTTPS)
- El servicio `ICookieService` abstrae operaciones CRUD via `IJSRuntime`
- Operaciones asíncronas para compatibilidad con Blazor WASM
</requirements>

<implementation>
Interface ICookieService:
```csharp
public interface ICookieService
{
    Task SetCookieAsync(string name, string value, DateTimeOffset? expires = null);
    Task<string?> GetCookieAsync(string name);
    Task DeleteCookieAsync(string name);
}
```

CookieService implementation:
- Usa `IJSRuntime.InvokeVoidAsync/InvokeAsync` para llamar funciones JS
- `SetCookieAsync`: Codifica valor con `Uri.EscapeDataString()`, formatea fecha con RFC 1123 ("R")
- `GetCookieAsync`: Recupera valor codificado y decodifica con `Uri.UnescapeDataString()`
- `DeleteCookieAsync`: Establece cookie con fecha de expiración en el pasado

JavaScript interop (cookieInterop.js):
```javascript
window.cookieInterop = {
    set: function (name, value, expires) {
        var cookie = name + '=' + value + '; path=/; SameSite=Lax';
        if (expires) {
            cookie += '; expires=' + expires;
        }
        if (window.location.protocol === 'https:') {
            cookie += '; Secure';
        }
        document.cookie = cookie;
    },
    get: function (name) {
        var pattern = new RegExp('(?:^|; )' + name.replace(/([.$?*|{}()\[\]\\\/\+^])/g, '\\$1') + '=([^;]*)');
        var matches = document.cookie.match(pattern);
        return matches ? matches[1] : null;
    },
    delete: function (name) {
        document.cookie = name + '=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/; SameSite=Lax';
    }
};
```

Uso en AuthService:
- `PersistTokenAsync()` parsea JWT, extrae `ValidTo`, llama a `SetCookieAsync()`
- `LogoutAsync()` llama a `DeleteCookieAsync()`
- Token cookie name: `AuthService.TokenCookieName = "re_access_token"`

Cálculo de expiración:
```csharp
var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
var expiry = jwtToken.ValidTo > DateTime.UtcNow
    ? new DateTimeOffset(jwtToken.ValidTo)
    : DateTimeOffset.UtcNow.AddHours(1); // fallback
```

Key files:
- `Services/ICookieService.cs` - Interface del servicio
- `Services/CookieService.cs` - Implementación con JS interop
- `wwwroot/js/cookieInterop.js` - Funciones JS nativas
- `wwwroot/index.html` - `<script src="js/cookieInterop.js"></script>`
- `Program.cs` - `builder.Services.AddScoped<ICookieService, CookieService>()`
</implementation>

<testing>
Verification plan:
- `SetCookieAsync`: Cookie visible en DevTools con nombre, valor codificado, expiración
- `GetCookieAsync`: Retorna valor decodificado correctamente
- `DeleteCookieAsync`: Cookie removida de DevTools
- Persistencia: Cookie sobrevive refresh de página
- Expiración: Cookie expira cuando el token expira
- Encoding: Tokens con caracteres especiales (=, +, /) almacenados correctamente
- SameSite: Cookie no enviada en requests cross-site (Lax)
- Secure: Flag Secure presente solo en HTTPS
- Path: Cookie accesible en todas las rutas (path=/)
- Fallback: Si JWT inválido, expira en 1 hora
</testing>
