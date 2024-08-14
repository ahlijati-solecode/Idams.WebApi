using Idams.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace Idams.Infrastructure.EntityFramework.Repositories
{
    public class UnitOfWorks : IUnitOfWorks
    {
        private readonly IdamsDbContext _dbContext;
        public UnitOfWorks(IdamsDbContext context)
        {
            _dbContext = context as IdamsDbContext;

            if (_dbContext == null)
                throw new ArgumentNullException("DB Context is Null");
        }

        public Task SaveChangesAsync()
        {
            return _dbContext.SaveChangesAsync();
        }

        public IDbContextTransaction BeginTransaction()
        {
            return _dbContext.Database.BeginTransaction();
        }

        public IDbContextTransaction BeginTransaction(IsolationLevel level)
        {
            return _dbContext.Database.BeginTransaction(level);
        }
    }
}
