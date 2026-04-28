using Microsoft.EntityFrameworkCore;
using RugbyEngine.Api.Data.Entities;
using RugbyEngine.Shared.Tablas;

namespace RugbyEngine.Api.Data.Repositories
{
    public class EntrenamientoRepository : GenericRepository<Entrenamiento>
    {
        public EntrenamientoRepository(ApiDbContext context, ILogger<EntrenamientoRepository> logger) : base(context, logger) { }

        public async Task<List<Entrenamiento>> SearchAsync(string? searchText, PaginacionDto paginacion, CancellationToken cancellationToken = default)
        {
            return await BuildSearchQuery(searchText)
                .OrderByDescending(e => e.Fecha)
                .Skip((paginacion.Pagina - 1) * paginacion.RegistrosPorPagina)
                .Take(paginacion.RegistrosPorPagina)
                .ToListAsync(cancellationToken);
        }

        public Task<int> CountAsync(string? searchText, CancellationToken cancellationToken = default)
            => BuildSearchQuery(searchText).CountAsync(cancellationToken);

        private IQueryable<Entrenamiento> BuildSearchQuery(string? searchText)
        {
            var query = _dbSet.Where(e => !e.BorradoLogico);
            if (string.IsNullOrWhiteSpace(searchText)) return query;
            var text = searchText.Trim();
            return query.Where(e => EF.Functions.ILike(e.Categoria.Nombre, $"%{text}%"));
        }
    }
}
