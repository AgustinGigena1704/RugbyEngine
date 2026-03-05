using Microsoft.EntityFrameworkCore;
using RugbyEngine.Api.Data.Entities;

namespace RugbyEngine.Api.Data.Repositories
{
    public class MenuRepository : GenericRepository<Menu>
    {
        public MenuRepository(ApiDbContext context, ILogger<GenericRepository<Menu>> logger) : base(context, logger)
        {
        }

        public async Task<List<Menu>> GetMenusByUser(Usuario usuario, CancellationToken cancellationToken = default)
        {
            // Cargar los IDs de permisos desde la BD para evitar depender de
            // propiedades de navegación que pueden no estar cargadas (Perfiles ? Permisos).
            var permisosIds = await _context.Set<Usuario>()
                .Where(u => u.Id == usuario.Id)
                .SelectMany(u => u.Perfiles!)
                .SelectMany(p => p.Permisos!)
                .Select(per => per.Id)
                .Distinct()
                .ToListAsync(cancellationToken);

            // Traer menús sin permiso (generales) o con permisos que el usuario tiene
            return await _dbSet
                .Where(m => m.DeletedAt == null &&
                           (m.PermisoId == null || permisosIds.Contains(m.PermisoId.Value)))
                .ToListAsync(cancellationToken);
        }
    }
}
