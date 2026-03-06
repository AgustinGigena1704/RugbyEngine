using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using RugbyEngine.Shared.Menus;
using System.Net.Http.Json;
using System.Text.Json;

namespace RugbyEngine.Client.Services
{
    public class MainMenuService : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;
        private readonly NavigationManager _navigationManager;
        private readonly IJSRuntime _jsRuntime;

        private const string SessionKey = "rugby_menu";
        private const string SessionKeyRouteRoles = "rugby_route_roles";

        private List<MenuDTO> _menuTree = new();
        private Dictionary<string, string> _routeRoles = new();
        private bool _menuLoaded = false;

        // Menú activo en cada capa (0 = top, 1 = desplegable, 2 = lateral)
        public MenuDTO? ActiveLevel0 { get; private set; }
        public MenuDTO? ActiveLevel1 { get; private set; }
        public MenuDTO? ActiveLevel2 { get; private set; }

        /// <summary>Se dispara cuando el menú activo cambia (navegación o recarga del árbol).</summary>
        public event Action? ActiveMenuChanged;

        public MainMenuService(HttpClient httpClient, IAuthService authService, NavigationManager navigationManager, IJSRuntime jsRuntime)
        {
            _httpClient = httpClient;
            _authService = authService;
            _navigationManager = navigationManager;
            _jsRuntime = jsRuntime;
            _navigationManager.LocationChanged += OnLocationChanged;
        }

        /// <summary>Devuelve el árbol de menús, cargándolo desde la API la primera vez.</summary>
        public async Task<List<MenuDTO>> GetMenuTreeAsync()
        {
            if (!_menuLoaded)
                await LoadMenuTreeAsync();

            return _menuTree;
        }

        /// <summary>Invalida la caché y recarga el árbol desde la API.</summary>
        public async Task InvalidateAsync()
        {
            _menuLoaded = false;
            _menuTree = new();
            _routeRoles = new();
            await LoadMenuTreeAsync();
        }

        /// <summary>Limpia la caché sin hacer petición (usar al cerrar sesión).</summary>
        public void ClearCache()
        {
            _menuLoaded = false;
            _menuTree = new();
            _routeRoles = new();
            ActiveLevel0 = null;
            ActiveLevel1 = null;
            ActiveLevel2 = null;
            _ = _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", SessionKey).AsTask();
            _ = _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", SessionKeyRouteRoles).AsTask();
            ActiveMenuChanged?.Invoke();
        }

        private async Task LoadMenuTreeAsync()
        {
            // Intentar leer ambas cachés desde sessionStorage
            try
            {
                var cachedMenu = await _jsRuntime.InvokeAsync<string?>("sessionStorage.getItem", SessionKey);
                var cachedRoles = await _jsRuntime.InvokeAsync<string?>("sessionStorage.getItem", SessionKeyRouteRoles);
                if (!string.IsNullOrEmpty(cachedMenu) && !string.IsNullOrEmpty(cachedRoles))
                {
                    _menuTree = JsonSerializer.Deserialize<List<MenuDTO>>(cachedMenu) ?? new();
                    _routeRoles = BuildRouteRoles(JsonSerializer.Deserialize<List<RouteRoleDTO>>(cachedRoles) ?? new());
                    _menuLoaded = true;
                    UpdateActiveMenu(_navigationManager.Uri);
                    ActiveMenuChanged?.Invoke();
                    return;
                }
            }
            catch { /* sessionStorage no disponible */ }

            // Consultar la API: menú del usuario y rutas protegidas en paralelo
            try
            {
                var menuTask = _httpClient.GetAsync("api/Menu");
                var rolesTask = _httpClient.GetAsync("api/Menu/RouteRoles");
                await Task.WhenAll(menuTask, rolesTask);

                if (menuTask.Result.IsSuccessStatusCode)
                    _menuTree = await menuTask.Result.Content.ReadFromJsonAsync<List<MenuDTO>>() ?? new();

                if (rolesTask.Result.IsSuccessStatusCode)
                {
                    var list = await rolesTask.Result.Content.ReadFromJsonAsync<List<RouteRoleDTO>>() ?? new();
                    _routeRoles = BuildRouteRoles(list);
                    try
                    {
                        await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", SessionKeyRouteRoles,
                            JsonSerializer.Serialize(list));
                    }
                    catch { /* ignorar errores de almacenamiento */ }
                }

                try
                {
                    await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", SessionKey,
                        JsonSerializer.Serialize(_menuTree));
                }
                catch { /* ignorar errores de almacenamiento */ }
            }
            catch (Exception)
            {
                _menuTree = new();
            }

            _menuLoaded = true;
            UpdateActiveMenu(_navigationManager.Uri);
            ActiveMenuChanged?.Invoke();
        }

        private static Dictionary<string, string> BuildRouteRoles(List<RouteRoleDTO> list)
            => list.Where(r => !string.IsNullOrWhiteSpace(r.Route))
                   .GroupBy(r => r.Route.Trim('/').ToLowerInvariant())
                   .Where(g => !string.IsNullOrEmpty(g.Key))
                   .ToDictionary(g => g.Key, g => g.First().Role);

        /// <summary>Devuelve el rol requerido para acceder a una ruta según el mapa global de rutas protegidas.
        /// Null si la ruta no requiere un rol específico.</summary>
        public string? GetRequiredRoleForRoute(string route)
        {
            if (!_menuLoaded || string.IsNullOrWhiteSpace(route)) return null;
            var normalizedPath = route.Trim('/').ToLowerInvariant();
            if (string.IsNullOrEmpty(normalizedPath)) return null;

            // Buscar coincidencia exacta primero, luego prefijo más largo
            if (_routeRoles.TryGetValue(normalizedPath, out var exactRole))
                return exactRole;

            foreach (var (key, role) in _routeRoles)
            {
                if (normalizedPath.StartsWith(key + "/", StringComparison.Ordinal))
                    return role;
            }

            return null;
        }

        private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            if (!_menuLoaded) return;
            UpdateActiveMenu(e.Location);
            ActiveMenuChanged?.Invoke();
        }

        private void UpdateActiveMenu(string uri)
        {
            var path = new Uri(uri).AbsolutePath.TrimEnd('/').ToLowerInvariant();

            ActiveLevel0 = null;
            ActiveLevel1 = null;
            ActiveLevel2 = null;

            foreach (var lvl0 in _menuTree)
            {
                foreach (var lvl1 in lvl0.Items)
                {
                    foreach (var lvl2 in lvl1.Items)
                    {
                        if (IsRouteMatch(lvl2.Route, path))
                        {
                            ActiveLevel0 = lvl0;
                            ActiveLevel1 = lvl1;
                            ActiveLevel2 = lvl2;
                            return;
                        }
                    }

                    if (IsRouteMatch(lvl1.Route, path))
                    {
                        ActiveLevel0 = lvl0;
                        ActiveLevel1 = lvl1;
                        return;
                    }
                }

                if (IsRouteMatch(lvl0.Route, path))
                {
                    ActiveLevel0 = lvl0;
                    return;
                }
            }
        }

        private static bool IsRouteMatch(string? route, string currentPath)
        {
            if (string.IsNullOrWhiteSpace(route)) return false;
            var normalized = "/" + route.Trim('/').ToLowerInvariant();
            return currentPath == normalized
                || currentPath.StartsWith(normalized + "/", StringComparison.Ordinal);
        }

        public void Dispose()
        {
            _navigationManager.LocationChanged -= OnLocationChanged;
        }
    }
}
