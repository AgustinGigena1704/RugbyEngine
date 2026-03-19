using Microsoft.EntityFrameworkCore;
using RugbyEngine.Api.Data.Entities;
using RugbyEngine.Shared.Tablas;

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

        public async Task<List<Usuario>> SearchAsync(string? searchText, PaginacionDto paginacion, CancellationToken cancellationToken = default)
        {
            var query = BuildSearchQuery(searchText);

            return await query
                .Skip((paginacion.Pagina - 1) * paginacion.RegistrosPorPagina)
                .Take(paginacion.RegistrosPorPagina)
                .ToListAsync(cancellationToken);
        }

        public Task<int> CountAsync(string? searchText, CancellationToken cancellationToken = default)
        {
            var query = BuildSearchQuery(searchText);
            return query.CountAsync(cancellationToken);
        }

        private IQueryable<Usuario> BuildSearchQuery(string? searchText)
        {
            var query = _dbSet
                .AsNoTracking()
                .Include(u => u.Persona)
                .Include(u => u.Perfiles)
                .Where(u => !u.BorradoLogico);

            if (string.IsNullOrWhiteSpace(searchText))
                return query;

            var text = searchText.Trim();
            return query.Where(u =>
                EF.Functions.ILike(u.Username, $"%{text}%") ||
                EF.Functions.ILike(u.Email ?? string.Empty, $"%{text}%") ||
                EF.Functions.ILike((u.Persona != null ? u.Persona.Nombres : string.Empty), $"%{text}%") ||
                EF.Functions.ILike((u.Persona != null ? u.Persona.Apellidos : string.Empty), $"%{text}%") ||
                EF.Functions.ILike((u.Persona != null ? u.Persona.Documento : string.Empty), $"%{text}%"));
        }
    }
}
