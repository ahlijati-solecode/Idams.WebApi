using Idams.Core.Model.Entities;
using Idams.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity.Migrations;

namespace Idams.Infrastructure.EntityFramework.Repositories
{
    public class EFTableColumnSettingRepository : ITableColumnSettingRepository
    {
        private readonly IdamsDbContext _dbContext;

        public EFTableColumnSettingRepository(IdamsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> GetProjectListTableConfig(string userId)
        {
            var ret = await _dbContext.TxProjectListColumnConfigs.AsQueryable().AsNoTracking().Where(n => n.UserId == userId).FirstOrDefaultAsync();
            return ret?.Config ?? "";
        }

        public async Task<int> SaveProjectListTableConfig(string userId, string config)
        {
            if(await _dbContext.TxProjectListColumnConfigs.AnyAsync( e => e.UserId == userId)){
                _dbContext.TxProjectListColumnConfigs.Update(new TxProjectListColumnConfig() { UserId = userId, Config = config });
            }
            else
            {
                _dbContext.TxProjectListColumnConfigs.Add(new TxProjectListColumnConfig() { UserId = userId, Config = config });
            }
            return await _dbContext.SaveChangesAsync();
        }
    }
}
