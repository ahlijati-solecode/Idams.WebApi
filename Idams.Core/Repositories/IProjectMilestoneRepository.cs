using Idams.Core.Model.Entities;

namespace Idams.Core.Repositories
{
    public interface IProjectMilestoneRepository
    {
        Task<bool> MilestoneExist(string projectId, int projectVersion);
        Task<List<TxProjectMilestone>> AddMilestoneAsync(List<TxProjectMilestone> milestones);
        Task<List<TxProjectMilestone>> UpdateMilestoneAsync(List<TxProjectMilestone> milestones);
        Task<List<TxProjectMilestone>> GetMilestonesAsync(string projectId, int projectVersion);
    }
}
