using Microsoft.EntityFrameworkCore;
using RugbyEngine.Api.Data.Entities;
using RugbyEngine.Shared.Tablas;

namespace RugbyEngine.Api.Data.Repositories
{
    public class JugadorRepository : GenericRepository<Jugador>
    {
        public JugadorRepository(ApiDbContext context, ILogger<JugadorRepository> logger) : base(context, logger) { }

        public async Task<List<Jugador>> SearchAsync(string? searchText, PaginacionDto paginacion, CancellationToken cancellationToken = default)
        {
            return await BuildSearchQuery(searchText)
                .OrderBy(j => j.Persona.Apellidos)
                .ThenBy(j => j.Persona.Nombres)
                .Skip((paginacion.Pagina - 1) * paginacion.RegistrosPorPagina)
                .Take(paginacion.RegistrosPorPagina)
                .ToListAsync(cancellationToken);
        }

        public Task<int> CountAsync(string? searchText, CancellationToken cancellationToken = default)
            => BuildSearchQuery(searchText).CountAsync(cancellationToken);

        public async Task<List<Jugador>> GetByCategoriaAsync(int categoriaId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(j => !j.BorradoLogico && j.CategoriaId == categoriaId)
                .OrderBy(j => j.Persona.Apellidos)
                .ThenBy(j => j.Persona.Nombres)
                .ToListAsync(cancellationToken);
        }

        private IQueryable<Jugador> BuildSearchQuery(string? searchText)
        {
            var query = _dbSet.Where(j => !j.BorradoLogico);
            if (string.IsNullOrWhiteSpace(searchText)) return query;
            var text = searchText.Trim();
            return query.Where(j =>
                EF.Functions.ILike(j.Persona.Nombres, $"%{text}%") ||
                EF.Functions.ILike(j.Persona.Apellidos, $"%{text}%") ||
                EF.Functions.ILike(j.Categoria.Nombre, $"%{text}%"));
        }
    }
}
