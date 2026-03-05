using RugbyEngine.Api.Data.Entities;

namespace RugbyEngine.Api.Data.Repositories
{
    public interface IRepository<TEntity> where TEntity : GenericEntity
    {
        Task<TEntity?> GetByIdAsync(int id, bool borradoLogico = false, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> GetAllAsync(bool borradoLogico = false, CancellationToken cancellationToken = default);
        Task<TEntity> AddAsync(TEntity entity, Usuario? by = null, CancellationToken cancellationToken = default);
        Task<TEntity> UpdateAsync(TEntity entity, Usuario? by = null, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, Usuario? by = null, CancellationToken cancellationToken = default);
    }
}
