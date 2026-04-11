using Microsoft.EntityFrameworkCore;
using RugbyEngine.Api.Data.Entities;
using RugbyEngine.Shared.Tablas;

namespace RugbyEngine.Api.Data.Repositories
{
    public class PosicionRepository : GenericRepository<Posicion>
    {
        public PosicionRepository(ApiDbContext context, ILogger<PosicionRepository> logger) : base(context, logger) { }

        public async Task<List<Posicion>> SearchAsync(string? searchText, PaginacionDto paginacion, CancellationToken cancellationToken = default)
        {
            return await BuildSearchQuery(searchText)
                .OrderBy(p => p.Numero)
                .Skip((paginacion.Pagina - 1) * paginacion.RegistrosPorPagina)
                .Take(paginacion.RegistrosPorPagina)
                .ToListAsync(cancellationToken);
        }

        public Task<int> CountAsync(string? searchText, CancellationToken cancellationToken = default)
            => BuildSearchQuery(searchText).CountAsync(cancellationToken);

        private IQueryable<Posicion> BuildSearchQuery(string? searchText)
        {
            var query = _dbSet.AsNoTracking().Where(p => !p.BorradoLogico);
            if (string.IsNullOrWhiteSpace(searchText)) return query;
            var text = searchText.Trim();
            int.TryParse(text, out var num);
            return query.Where(p =>
                EF.Functions.ILike(p.Nombre, $"%{text}%") ||
                (num > 0 && p.Numero == num));
        }
    }
}
