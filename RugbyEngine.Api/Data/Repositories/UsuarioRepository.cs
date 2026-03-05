using Microsoft.EntityFrameworkCore;

using RugbyEngine.Api.Data.Entities;

namespace RugbyEngine.Api.Data.Repositories
{
    public class UsuarioRepository : GenericRepository<Usuario>
    {
        public UsuarioRepository(ApiDbContext context, ILogger<GenericRepository<Usuario>> logger)
            : base(context, logger)
        {
        }

        /// <summary>
        /// Obtiene un usuario por su username
        /// </summary>
        /// <param name="username">Username del usuario</param>
        /// <param name="borradoLogico">Si es true, incluye usuarios eliminados lógicamente</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        public async Task<Usuario?> GetByUsernameAsync(string username, bool borradoLogico = false, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(u => u.Username == username);

            if (!borradoLogico)
            {
                query = query.Where(u => u.BorradoLogico == false);
            }

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Verifica si existe un usuario con el username especificado
        /// </summary>
        /// <param name="username">Username a verificar</param>
        /// <param name="borradoLogico">Si es true, incluye usuarios eliminados lógicamente</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        public async Task<bool> ExistsAsync(string username, bool borradoLogico = false, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(u => u.Username == username);

            if (!borradoLogico)
            {
                query = query.Where(u => u.BorradoLogico == false);
            }

            return await query.AnyAsync(cancellationToken);
        }
    }
}

