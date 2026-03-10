using Microsoft.EntityFrameworkCore;
using RugbyEngine.Api.Data.Entities;

namespace RugbyEngine.Api.Data.Repositories
{
    public class UsuarioRepository : GenericRepository<Usuario>
    {
        public UsuarioRepository(ApiDbContext context, ILogger<UsuarioRepository> logger)
            : base(context, logger)
        {
        }

        /// <summary>Obtiene un usuario por su username.</summary>
        public async Task<Usuario?> GetByUsernameAsync(string username, bool borradoLogico = false, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(u => u.Username == username);
            if (!borradoLogico)
                query = query.Where(u => !u.BorradoLogico);
            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>Verifica si existe un usuario con el username especificado.</summary>
        public async Task<bool> ExistsAsync(string username, bool borradoLogico = false, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(u => u.Username == username);
            if (!borradoLogico)
                query = query.Where(u => !u.BorradoLogico);
            return await query.AnyAsync(cancellationToken);
        }

        /// <summary>Verifica si el PersonaId ya esta vinculado a otro usuario activo.</summary>
        public async Task<bool> PersonaYaTieneUsuarioAsync(int personaId, int? excludeUsuarioId = null, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(u => u.PersonaId == personaId && !u.BorradoLogico);
            if (excludeUsuarioId.HasValue)
                query = query.Where(u => u.Id != excludeUsuarioId.Value);
            return await query.AnyAsync(cancellationToken);
        }
    }
}
