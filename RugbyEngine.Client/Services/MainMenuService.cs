using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using RugbyEngine.Client.Handlers;
using RugbyEngine.Shared.Menus;
using System.Net.Http.Json;

namespace RugbyEngine.Client.Services
{
    public class MainMenuService : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly NavigationManager _navigationManager;
        private readonly IStorageService _storage;

        private List<MenuDto> _menuTree = new();
        private Dictionary<string, string> _routeRoles = new();
        private bool _menuLoaded = false;

        // Menú activo en cada capa (0 = top, 1 = desplegable, 2 = lateral)
        public MenuDto? ActiveLevel0 { get; private set; }
        public MenuDto? ActiveLevel1 { get; private set; }
        public MenuDto? ActiveLevel2 { get; private set; }

        /// <summary>Se dispara cuando el menú activo cambia (navegación o recarga del árbol).</summary>
        public event Action? ActiveMenuChanged;

        public MainMenuService(HttpClient httpClient, NavigationManager navigationManager, IStorageService storage)
        {
            _httpClient = httpClient;
            _navigationManager = navigationManager;
            _storage = storage;
            _navigationManager.LocationChanged += OnLocationChanged;
        }

        /// <summary>Devuelve el árbol de menús, cargándolo desde la API la primera vez.</summary>
        public async Task<List<MenuDto>> GetMenuTreeAsync()
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
            await _storage.RemoveSessionItemAsync("menuTree");
            await _storage.RemoveSessionItemAsync("routeRoles");
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
            _ = Task.Run(async () =>
            {
                await _storage.RemoveSessionItemAsync("menuTree");
                await _storage.RemoveSessionItemAsync("routeRoles");
            });
            ActiveMenuChanged?.Invoke();
        }

        private async Task LoadMenuTreeAsync()
        {
            try
            {
                var cachedMenu = await _storage.GetSessionItemAsync<List<MenuDto>>("menuTree");
                var cachedRoles = await _storage.GetSessionItemAsync<Dictionary<string, string>>("routeRoles");

                if (cachedMenu != null && cachedMenu.Count > 0 && cachedRoles != null)
                {
                    _menuTree = cachedMenu;
                    _routeRoles = cachedRoles;
                    _menuLoaded = true;
                    UpdateActiveMenu(_navigationManager.Uri);
                    ActiveMenuChanged?.Invoke();
                    return;
                }


                var menuRequest = new HttpRequestMessage(HttpMethod.Get, "api/Menu");
                menuRequest.Options.Set(OperationCanceledExceptionDelegatingHandler.SuppressNotification, true);
                var rolesRequest = new HttpRequestMessage(HttpMethod.Get, "api/Menu/RouteRoles");
                rolesRequest.Options.Set(OperationCanceledExceptionDelegatingHandler.SuppressNotification, true);

                var menuTask = _httpClient.SendAsync(menuRequest);
                var rolesTask = _httpClient.SendAsync(rolesRequest);
                await Task.WhenAll(menuTask, rolesTask);

                if (menuTask.Result.IsSuccessStatusCode)
                    _menuTree = await menuTask.Result.Content.ReadFromJsonAsync<List<MenuDto>>() ?? new();

                if (rolesTask.Result.IsSuccessStatusCode)
                {
                    var list = await rolesTask.Result.Content.ReadFromJsonAsync<List<RouteRoleDto>>() ?? new();
                    _routeRoles = BuildRouteRoles(list);
                }

                if (_menuTree.Count > 0)
                {
                    await _storage.SetSessionItemAsync("menuTree", _menuTree);
                    await _storage.SetSessionItemAsync("routeRoles", _routeRoles);
                }
            }
            catch (Exception)
            {
                _menuTree = new();
            }

            _menuLoaded = true;
            UpdateActiveMenu(_navigationManager.Uri);
            ActiveMenuChanged?.Invoke();
        }

        private static Dictionary<string, string> BuildRouteRoles(List<RouteRoleDto> list)
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

            var previousLevel0 = ActiveLevel0;
            var previousLevel1 = ActiveLevel1;
            var previousLevel2 = ActiveLevel2;

            UpdateActiveMenu(e.Location);

            // Only invoke ActiveMenuChanged if the active menu has actually changed
            if (previousLevel0 != ActiveLevel0 || previousLevel1 != ActiveLevel1 || previousLevel2 != ActiveLevel2)
            {
                ActiveMenuChanged?.Invoke();
            }
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
                    var matchedLvl2 = lvl1.Items.FirstOrDefault(lvl2 => IsRouteMatch(lvl2.Route, path));
                    if (matchedLvl2 != null)
                    {
                        ActiveLevel0 = lvl0;
                        ActiveLevel1 = lvl1;
                        ActiveLevel2 = matchedLvl2;
                        return;
                    }

                    if (IsRouteMatch(lvl1.Route, path))
                    {
                        ActiveLevel0 = lvl0;
                        ActiveLevel1 = lvl1;

                        var firstChild = FindFirstRoutableDescendant(lvl1);
                        if (firstChild != null)
                            ActiveLevel2 = firstChild;

                        return;
                    }
                }

                if (IsRouteMatch(lvl0.Route, path))
                {
                    ActiveLevel0 = lvl0;

                    var firstLevel1 = lvl0.Items.FirstOrDefault();
                    if (firstLevel1 != null)
                    {
                        ActiveLevel1 = firstLevel1;
                        ActiveLevel2 = FindFirstRoutableDescendant(firstLevel1);
                    }
                    else
                    {
                        // Si un lvl0 no tiene hijos de lvl1, no seleccionar un lvl1 automático
                        // Dejar ActiveLevel1/2 en null para que la navegación entre en el lvl0 (ej: Entrenamientos)
                        ActiveLevel1 = null;
                        ActiveLevel2 = null;
                    }

                    return;
                }
            }
        }

        private static MenuDto? FindFirstRoutableDescendant(MenuDto item)
        {
            if (item.Items is not { Count: > 0 }) return null;

            foreach (var child in item.Items)
            {
                if (!string.IsNullOrWhiteSpace(child.Route))
                    return child;

                var nested = FindFirstRoutableDescendant(child);
                if (nested != null)
                    return nested;
            }

            return null;
        }

        private static bool IsRouteMatch(string? route, string currentPath)
        {
            if (string.IsNullOrWhiteSpace(route)) return false;
            var normalized = "/" + route.Trim('/').ToLowerInvariant();
            return currentPath == normalized
                || currentPath.StartsWith(normalized + "/", StringComparison.Ordinal);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _navigationManager.LocationChanged -= OnLocationChanged;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
