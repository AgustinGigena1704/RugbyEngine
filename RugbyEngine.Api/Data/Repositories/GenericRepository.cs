using Microsoft.EntityFrameworkCore;
using RugbyEngine.Api.Data.Entities;
using System.Linq.Expressions;

namespace RugbyEngine.Api.Data.Repositories
{
    public class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : GenericEntity
    {
        protected readonly ApiDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;
        protected readonly ILogger _logger;
        public GenericRepository(ApiDbContext context, ILogger logger)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
            _logger = logger;
        }

        public virtual async Task<TEntity?> GetByIdAsync(int id, bool borradoLogico = false, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(e => e.Id == id).AsQueryable();
            if (!borradoLogico)
            {
                query = query.Where(e => e.BorradoLogico == borradoLogico);
            }
            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(bool borradoLogico = false, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsQueryable();
            if (!borradoLogico)
            {
                query = query.Where(e => e.BorradoLogico == borradoLogico);
            }
            return await query.ToListAsync(cancellationToken);
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity, Usuario? by = null, CancellationToken cancellationToken = default)
        {
            if (entity is IAudithory audithoryEntity)
            {
                if (by == null)
                {
                    _logger.LogWarning("AddAsync llamado sin usuario 'by' para entidad {Entity} con id {Id}", typeof(TEntity).Name, entity.Id);
                    throw new ArgumentNullException(nameof(by), "El usuario 'by' debe ser proporcionado para entidades con auditoría.");
                }
                audithoryEntity.CreatedBy = by;
                audithoryEntity.CreatedById = by.Id;
                audithoryEntity.CreatedAt = DateTime.UtcNow;
            }

            await _dbSet.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity entity, Usuario? by = null, CancellationToken cancellationToken = default)
        {
            if (entity is IAudithory audithoryEntity)
            {
                if (by == null)
                {
                    _logger.LogWarning("UpdateAsync llamado sin usuario 'by' para entidad {Entity} con id {Id}", typeof(TEntity).Name, entity.Id);
                    throw new ArgumentNullException(nameof(by), "El usuario 'by' debe ser proporcionado para entidades con auditoría.");
                }
                audithoryEntity.UpdatedBy = by;
                audithoryEntity.UpdatedById = by.Id;
                audithoryEntity.UpdatedAt = DateTime.UtcNow;
            }

            _dbSet.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public virtual async Task<bool> DeleteAsync(int id, Usuario? by = null, CancellationToken cancellationToken = default)
        {
            var entity = await GetByIdAsync(id, cancellationToken: cancellationToken);
            if (entity == null)
                return false;

            entity.BorradoLogico = true;
            if (entity is IAudithory audithoryEntity)
            {
                if (by == null)
                {
                    _logger.LogWarning("El usuario 'by' debe ser proporcionado para entidades con auditoría.");
                    throw new ArgumentNullException(nameof(by), "El usuario 'by' debe ser proporcionado para entidades con auditoría.");
                }
                audithoryEntity.DeletedBy = by;
                audithoryEntity.DeletedById = by.Id;
                audithoryEntity.DeletedAt = DateTime.UtcNow;
            }
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, bool borradoLogico = false, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(predicate).AsQueryable();
            if (!borradoLogico)
            {
                query = query.Where(e => e.BorradoLogico == borradoLogico);
            }
            return await query.ToListAsync(cancellationToken);
        }
    }
}
