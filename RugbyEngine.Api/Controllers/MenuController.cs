using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RugbyEngine.Api.Data;
using RugbyEngine.Api.Data.Entities;
using RugbyEngine.Api.Data.Repositories;
using RugbyEngine.Api.Services.Interfaces;
using RugbyEngine.Shared.Menus;

namespace RugbyEngine.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : GenericController
    {
        public MenuController(EntityManager entityManager, ICurrentUserService currentUserService) : base(entityManager, currentUserService)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetMenuTree()
        {
            var usuario = await GetCurrentUserAsync();
            if (usuario == null) return NotFound();

            List<Menu> menus = await _entityManager.GetRepository<MenuRepository>().GetMenusByUser(usuario);

            Dictionary<int, MenuDTO> menuDictionary = new Dictionary<int, MenuDTO>();
            List<MenuDTO> rootMenus = new List<MenuDTO>();

            // Pasada 1: crear todos los DTOs y registrarlos en el diccionario (Route = segmento propio)
            foreach (var menu in menus)
            {
                menuDictionary[menu.Id] = new MenuDTO
                {
                    Title = menu.Titulo,
                    ToolTip = menu.ToolTip ?? string.Empty,
                    Icon = menu.Icono ?? string.Empty,
                    Route = menu.Ruta ?? string.Empty,
                    Role = menu.Permiso?.Codigo,
                    Items = new List<MenuDTO>()
                };
            }

            // Pasada 2: construir la jerarquía (independiente del orden de la lista)
            foreach (var menu in menus)
            {
                if (menu.MenuPadreId == null)
                {
                    rootMenus.Add(menuDictionary[menu.Id]);
                }
                else if (menuDictionary.ContainsKey(menu.MenuPadreId.Value))
                {
                    menuDictionary[menu.MenuPadreId.Value].Items.Add(menuDictionary[menu.Id]);
                }
            }

            // Pasada 3: calcular rutas completas concatenando segmentos (Ruta lvl0/lvl1/lvl2/...)
            static void ComputeRoutes(List<MenuDTO> items, string parentRoute)
            {
                foreach (var item in items)
                {
                    var segment = (item.Route ?? string.Empty).Trim('/');
                    item.Route = !string.IsNullOrEmpty(segment)
                        ? $"{parentRoute}/{segment}"
                        : parentRoute;
                    if (item.Items.Count > 0)
                        ComputeRoutes(item.Items, item.Route);
                }
            }
            ComputeRoutes(rootMenus, string.Empty);

            return Ok(rootMenus);
        }

        [HttpGet("RouteRoles")]
        [Authorize]
        public async Task<IActionResult> GetRouteRoles()
        {
            var routeRoles = await _entityManager.GetRepository<MenuRepository>().GetAllRouteRolesAsync();
            return Ok(routeRoles);
        }
    }
}
