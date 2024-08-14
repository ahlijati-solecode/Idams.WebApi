namespace Idams.Infrastructure.EntityFramework.Core
{
    public interface IRepository<TEntity, TKey>
    {
        Task<TEntity> AddAsync(TEntity input);

        Task<TEntity> UpdateAsync(TEntity input);

        Task<TEntity> DeleteAsync(TEntity input, bool permanent = false);

        Task<TEntity> GetById(TKey id, bool isDeleted = false);
    }
}