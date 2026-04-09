using Microsoft.EntityFrameworkCore;
using RugbyEngine.Api.Data.Entities;
using RugbyEngine.Api.Services;
using RugbyEngine.Shared.Menus;

namespace RugbyEngine.Api.Data.Repositories
{
    public class MenuRepository : GenericRepository<Menu>
    {
        public MenuRepository(ApiDbContext context, ILogger<MenuRepository> logger) : base(context, logger)
        {
        }

        public async Task<List<Menu>> GetMenusByUser(Usuario usuario, CancellationToken cancellationToken = default)
        {
            var perfiles = usuario.Perfiles;
            var permisosIds = perfiles?
                .SelectMany(p => p.Permisos ?? new List<Permiso>())
                .Select(pp => pp.Id)
                .ToList() ?? new List<int>();
            return await _dbSet
                .Where(m => m.DeletedAt == null &&
                           (m.PermisoId == null || permisosIds.Contains(m.PermisoId.Value)))
                .ToListAsync(cancellationToken);
        }

        public async Task<List<RouteRoleDto>> GetAllRouteRolesAsync(CancellationToken cancellationToken = default)
        {
            var menus = await _dbSet
                .Where(m => m.DeletedAt == null && m.PermisoId != null)
                .ToListAsync(cancellationToken);

            var dict = menus.ToDictionary(m => m.Id);
            var result = new List<RouteRoleDto>();

            foreach (var menu in menus)
            {
                if (menu.Permiso == null) continue;

                var parts = new List<string>();
                var current = menu;
                while (current != null)
                {
                    if (!string.IsNullOrEmpty(current.Ruta))
                        parts.Insert(0, current.Ruta);
                    current = current.MenuPadreId.HasValue && dict.ContainsKey(current.MenuPadreId.Value)
                        ? dict[current.MenuPadreId.Value]
                        : null;
                }

                if (parts.Count == 0) continue;

                result.Add(new RouteRoleDto
                {
                    Route = "/" + string.Join("/", parts),
                    Role = menu.Permiso.Codigo
                });
            }

            return result;
        }
    }
}


