using Idams.Core.Model;
using Idams.Core.Model.Entities;
using Idams.Core.Repositories;
using Idams.Core.Services;
using Idams.Infrastructure.EntityFramework.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Idams.Infrastructure.EntityFramework.Repositories
{
    public class EFProjectActionRepository : BaseRepository, IProjectActionRepository
    {
        public EFProjectActionRepository(IConfiguration configuration,
            ICurrentUserService currentUserService,
            IdamsDbContext dbContext)
            :base(configuration, currentUserService, dbContext)
        {
        }

        public async Task<TxProjectAction> AddAsync(TxProjectAction txProjectAction)
        {
            var latest = await _dbContext.TxProjectActions.AsNoTracking().OrderByDescending(n => n.ProjectActionId).FirstOrDefaultAsync();
            txProjectAction.ProjectActionId = IdGenerationUtil.GenerateNextId(latest?.ProjectActionId, "PA");
            txProjectAction.CreatedDate = DateTime.UtcNow;
            txProjectAction.CreatedBy = GetCurrentUser;
            await _dbContext.TxProjectActions.AddAsync(txProjectAction);
            return txProjectAction;
        }

        public async Task<TxProjectAction> UpdateAsync(TxProjectAction txProjectAction)
        {
            txProjectAction.UpdatedDate = DateTime.UtcNow;
            txProjectAction.UpdatedBy = GetCurrentUser;
            _dbContext.TxProjectActions.Update(txProjectAction);
            return txProjectAction;
        }

        public async Task<TxProjectAction?> GetAction(string? projectActionId)
        {
            return await _dbContext.TxProjectActions.AsNoTracking().Include(n => n.WorkflowSequence).Where(n => n.ProjectActionId == projectActionId).FirstOrDefaultAsync();
        }

        public async Task<TxProjectAction?> GetProjectActionWithRefWorkflowAction(string? projectActionId)
        {
            return await _dbContext.TxProjectActions.AsNoTracking().Where(n => n.ProjectActionId == projectActionId).Include(n => n.WorkflowAction).FirstOrDefaultAsync();
        }

        public async Task<List<TxProjectAction>> GetProjectAction(string projectId, string sequenceId)
        {
            return await (from pa in _dbContext.TxProjectActions
                       join wa in _dbContext.RefWorkflowActions on pa.WorkflowActionId equals wa.WorkflowActionId
                       where pa.ProjectId == projectId && wa.WorkflowActionTypeParId != null && pa.WorkflowSequenceId == sequenceId
                       select pa).AsNoTracking().ToListAsync();
        }

        public async Task<TxProjectAction?> GetProjectAction(string? projectId, string? workflowSequenceId, string? workflowActionId)
        {
            return await _dbContext.TxProjectActions.AsNoTracking()
                .Where(n => n.ProjectId == projectId && n.WorkflowSequenceId == workflowSequenceId && n.WorkflowActionId == workflowActionId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<RefWorkflowActor>> GetActorByAction(string workflowActionId)
        {
            return await _dbContext.RefWorkflowActors.Where(n => n.WorkflowActionId == workflowActionId).ToListAsync();
        }

        public async Task<TxProjectAction?> GetLastProjectAction(string projectId, string workfloSequeceId)
        {
            return await _dbContext.TxProjectActions.AsNoTracking().Where(n => n.ProjectId == projectId && n.WorkflowSequenceId == workfloSequeceId)
                .OrderByDescending(n => n.WorkflowActionId).FirstOrDefaultAsync();
        }
    }
}
