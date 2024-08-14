using Idams.Core.Enums;
using Idams.Core.Model.Entities;
using Idams.Core.Repositories;
using Idams.Infrastructure.EntityFramework.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data.SqlClient;

namespace Idams.Infrastructure.EntityFramework.Repositories
{
    public class EFUserRepository : EFRepository<User, int, List<RoleEnum>>, IUserRepository
    {
        protected readonly IdamsDbContext _dbContext;
        public EFUserRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _dbContext = serviceProvider.GetService<IdamsDbContext>() ?? throw new ArgumentNullException(nameof(IdamsDbContext));
        }

        public Task<User> GetByIdAsync(int id)
        {
            return base.GetById(id);
        }

        //public Task<User> GetEmployee()
        //{
        //    string sql = "EXEC SalesLT.Product_UpdateListPrice @OrgUnitID, @ListPrice";

        //    List<SqlParameter> parms = new List<SqlParameter>
        //    { 
        //        new SqlParameter { ParameterName = "@OrgUnitID", Value = "10092067" }
        //    };
        //    int result = _dbContext.Database.ExecuteSqlRaw(sql, parms.ToArray());
        //    //return new User();
        //}
    }
}
