using Microsoft.EntityFrameworkCore;
using RugbyEngine.Api.Data.Entities;

namespace RugbyEngine.Api.Data.Repositories
{
    public class PersonaRepository : GenericRepository<Persona>
    {
        public PersonaRepository(ApiDbContext context, ILogger<GenericRepository<Persona>> logger) : base(context, logger)
        {
        }

        public async Task<Persona?> GetByDocumentoAsync(string dni, bool borradoLogico = false, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(p => p.Documento == dni);
            if (!borradoLogico)
            {
                query = query.Where(p => p.BorradoLogico == false);
            }
            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> ExistsDocumentoAsync(string dni, bool borradoLogico = false, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(p => p.Documento == dni);
            if (!borradoLogico)
            {
                query = query.Where(p => p.BorradoLogico == false);
            }
            return await query.AnyAsync(cancellationToken);
        }

        public async Task<List<Persona>> SearchByNombreAsync(string nombre, bool borradoLogico = false, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(p => p.Nombres.Contains(nombre));
            if (!borradoLogico)
            {
                query = query.Where(p => p.BorradoLogico == false);
            }
            return await query.ToListAsync(cancellationToken);
        }
    }
}
