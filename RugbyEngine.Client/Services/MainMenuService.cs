using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using RugbyEngine.Shared.Menus;
using System.Net.Http.Json;

namespace RugbyEngine.Client.Services
{
    public class MainMenuService : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;
        private readonly NavigationManager _navigationManager;

        private List<MenuDTO> _menuTree = new();
        private bool _menuLoaded = false;

        // Menú activo en cada capa (0 = top, 1 = desplegable, 2 = lateral)
        public MenuDTO? ActiveLevel0 { get; private set; }
        public MenuDTO? ActiveLevel1 { get; private set; }
        public MenuDTO? ActiveLevel2 { get; private set; }

        /// <summary>Se dispara cuando el menú activo cambia (navegación o recarga del árbol).</summary>
        public event Action? ActiveMenuChanged;

        public MainMenuService(HttpClient httpClient, IAuthService authService, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _authService = authService;
            _navigationManager = navigationManager;
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
            await LoadMenuTreeAsync();
        }

        /// <summary>Limpia la caché sin hacer petición (usar al cerrar sesión).</summary>
        public void ClearCache()
        {
            _menuLoaded = false;
            _menuTree = new();
            ActiveLevel0 = null;
            ActiveLevel1 = null;
            ActiveLevel2 = null;
            ActiveMenuChanged?.Invoke();
        }

        private async Task LoadMenuTreeAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Menu");
                if (response.IsSuccessStatusCode)
                    _menuTree = await response.Content.ReadFromJsonAsync<List<MenuDTO>>() ?? new();
            }
            catch (Exception)
            {
                _menuTree = new();
            }

            _menuLoaded = true;
            UpdateActiveMenu(_navigationManager.Uri);
            ActiveMenuChanged?.Invoke();
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
