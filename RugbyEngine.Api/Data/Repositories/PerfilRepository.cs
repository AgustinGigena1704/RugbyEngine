using Microsoft.EntityFrameworkCore;
using RugbyEngine.Api.Data.Entities;

namespace RugbyEngine.Api.Data.Repositories
{
    public class PerfilRepository : GenericRepository<Perfil>
    {
        public PerfilRepository(ApiDbContext context, ILogger<GenericRepository<Perfil>> logger)
            : base(context, logger)
        {
        }

        public async Task<List<Perfil>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Usuario>()
                .Where(u => u.Id == userId)
                .SelectMany(u => u.Perfiles!)
                .Where(p => !p.BorradoLogico)
                .ToListAsync(cancellationToken);
        }

        public async Task AssignToUserAsync(int perfilId, int userId, CancellationToken cancellationToken = default)
        {
            var user = await _context.Set<Usuario>()
                .Include(u => u.Perfiles)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            var perfil = await _dbSet.FindAsync([perfilId], cancellationToken);

            if (user == null || perfil == null) return;

            if (user.Perfiles!.All(p => p.Id != perfilId))
            {
                user.Perfiles!.Add(perfil);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task RemoveFromUserAsync(int perfilId, int userId, CancellationToken cancellationToken = default)
        {
            var user = await _context.Set<Usuario>()
                .Include(u => u.Perfiles)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user == null) return;

            var perfil = user.Perfiles?.FirstOrDefault(p => p.Id == perfilId);
            if (perfil != null)
            {
                user.Perfiles!.Remove(perfil);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
