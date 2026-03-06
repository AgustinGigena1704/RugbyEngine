using Microsoft.EntityFrameworkCore;
using RugbyEngine.Api.Data.Entities;
using RugbyEngine.Shared.Menus;

namespace RugbyEngine.Api.Data.Repositories
{
    public class MenuRepository : GenericRepository<Menu>
    {
        public MenuRepository(ApiDbContext context, ILogger<GenericRepository<Menu>> logger) : base(context, logger)
        {
        }

        public async Task<List<Menu>> GetMenusByUser(Usuario usuario, CancellationToken cancellationToken = default)
        {
            var permisosIds = await _context.Set<Usuario>()
                .Where(u => u.Id == usuario.Id)
                .SelectMany(u => u.Perfiles!)
                .SelectMany(p => p.Permisos!)
                .Select(per => per.Id)
                .Distinct()
                .ToListAsync(cancellationToken);

            return await _dbSet
                .Where(m => m.DeletedAt == null &&
                           (m.PermisoId == null || permisosIds.Contains(m.PermisoId.Value)))
                .ToListAsync(cancellationToken);
        }

        public async Task<List<RouteRoleDTO>> GetAllRouteRolesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(m => m.DeletedAt == null && !string.IsNullOrEmpty(m.Ruta) && m.PermisoId != null)
                .Select(m => new RouteRoleDTO
                {
                    Route = m.Ruta!,
                    Role = m.Permiso!.Codigo
                })
                .ToListAsync(cancellationToken);
        }
    }
}


