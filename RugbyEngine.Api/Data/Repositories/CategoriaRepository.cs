using Microsoft.EntityFrameworkCore;
using RugbyEngine.Api.Data.Entities;
using RugbyEngine.Shared.Tablas;

namespace RugbyEngine.Api.Data.Repositories
{
    public class CategoriaRepository : GenericRepository<Categoria>
    {
        public CategoriaRepository(ApiDbContext context, ILogger<CategoriaRepository> logger) : base(context, logger) { }

        public async Task<List<Categoria>> SearchAsync(string? searchText, PaginacionDto paginacion, CancellationToken cancellationToken = default)
        {
            return await BuildSearchQuery(searchText)
                .OrderBy(c => c.Nombre)
                .Skip((paginacion.Pagina - 1) * paginacion.RegistrosPorPagina)
                .Take(paginacion.RegistrosPorPagina)
                .ToListAsync(cancellationToken);
        }

        public Task<int> CountAsync(string? searchText, CancellationToken cancellationToken = default)
            => BuildSearchQuery(searchText).CountAsync(cancellationToken);

        private IQueryable<Categoria> BuildSearchQuery(string? searchText)
        {
            var query = _dbSet.AsNoTracking().Where(c => !c.BorradoLogico);
            if (string.IsNullOrWhiteSpace(searchText)) return query;
            var text = searchText.Trim();
            return query.Where(c =>
                EF.Functions.ILike(c.Nombre, $"%{text}%") ||
                EF.Functions.ILike(c.Abreviatura, $"%{text}%"));
        }
    }
}
