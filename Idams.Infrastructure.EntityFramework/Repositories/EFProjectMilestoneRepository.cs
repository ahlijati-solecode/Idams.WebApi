using Idams.Core.Model.Entities;
using Idams.Core.Repositories;
using Idams.Core.Services;
using Idams.Infrastructure.EntityFramework.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Idams.Infrastructure.EntityFramework.Repositories
{
    public class EFProjectMilestoneRepository : BaseRepository, IProjectMilestoneRepository
    {
        public EFProjectMilestoneRepository(IConfiguration configuration, ICurrentUserService currentService, IdamsDbContext dbContext) : 
            base(configuration, currentService, dbContext)
        {
        }

        public async Task<List<TxProjectMilestone>> AddMilestoneAsync(List<TxProjectMilestone> milestones)
        {
            await _dbContext.TxProjectMilestones.AddRangeAsync(milestones);
            await _dbContext.SaveChangesAsync();
            return milestones;
        }

        public async Task<List<TxProjectMilestone>> UpdateMilestoneAsync(List<TxProjectMilestone> milestones)
        {
            _dbContext.TxProjectMilestones.UpdateRange(milestones);
            await _dbContext.SaveChangesAsync();
            return milestones;
        }

        public async Task<bool> MilestoneExist(string projectId, int projectVersion)
        {
            var exist = await _dbContext.TxProjectMilestones.AsNoTracking().Where(n => n.ProjectId == projectId && n.ProjectVersion == projectVersion).FirstOrDefaultAsync();
            return exist != null;
        }

        public async Task<List<TxProjectMilestone>> GetMilestonesAsync(string projectId, int projectVersion)
        {
            return await _dbContext.TxProjectMilestones.Where(n => n.ProjectId == projectId && n.ProjectVersion == projectVersion).ToListAsync();
        }
    }
}
