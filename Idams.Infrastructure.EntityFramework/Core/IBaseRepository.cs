namespace Idams.Infrastructure.EntityFramework.Core
{
    public interface IBaseRepository<TEntity> : IRepository<TEntity, int>
    {

    }
    public interface IBaseRepository<TEntity, TKey> : IRepository<TEntity, TKey>
    {

    }
}