using Microsoft.EntityFrameworkCore;
using RugbyEngine.Api.Data.Entities;
using RugbyEngine.Shared.Tablas;

namespace RugbyEngine.Api.Data.Repositories
{
    public class PersonaRepository : GenericRepository<Persona>
    {
        public PersonaRepository(ApiDbContext context, ILogger<PersonaRepository> logger) : base(context, logger)
        {
        }

        public async Task<Persona?> GetByDocumentoAsync(string dni, bool borradoLogico = false, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(p => p.Documento == dni);
            if (!borradoLogico)
            {
                query = query.Where(p => !p.BorradoLogico);
            }
            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> ExistsDocumentoAsync(string dni, bool borradoLogico = false, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(p => p.Documento == dni);
            if (!borradoLogico)
            {
                query = query.Where(p => !p.BorradoLogico);
            }
            return await query.AnyAsync(cancellationToken);
        }

        public async Task<List<Persona>> SearchAsync(string? searchText, PaginacionDto paginacion, CancellationToken cancellationToken = default)
        {
            var query = BuildSearchQuery(searchText)
                .OrderBy(p => p.Apellidos)
                .ThenBy(p => p.Nombres);

            return await query
                .Skip((paginacion.Pagina - 1) * paginacion.RegistrosPorPagina)
                .Take(paginacion.RegistrosPorPagina)
                .ToListAsync(cancellationToken);
        }

        public Task<int> CountAsync(string? searchText, CancellationToken cancellationToken = default)
        {
            return BuildSearchQuery(searchText).CountAsync(cancellationToken);
        }

        private IQueryable<Persona> BuildSearchQuery(string? searchText)
        {
            var query = _dbSet.AsNoTracking().Where(p => !p.BorradoLogico);

            if (string.IsNullOrWhiteSpace(searchText))
                return query;

            var text = searchText.Trim();
            return query.Where(p =>
                EF.Functions.ILike(p.Nombres, $"%{text}%") ||
                EF.Functions.ILike(p.Apellidos, $"%{text}%") ||
                EF.Functions.ILike(p.Documento, $"%{text}%"));
        }
    }
}
