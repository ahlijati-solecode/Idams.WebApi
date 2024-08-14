using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace Idams.Core.Repositories
{
    public interface IUnitOfWorks
    {
        Task SaveChangesAsync();
        IDbContextTransaction BeginTransaction();
        IDbContextTransaction BeginTransaction(IsolationLevel level);
    }
}
