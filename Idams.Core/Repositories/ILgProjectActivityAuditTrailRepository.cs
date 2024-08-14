
using Idams.Core.Enums;
using Idams.Core.Model.Entities;
using Idams.Core.Model.Entities.Custom;
using Idams.Core.Model.Filters;

namespace Idams.Core.Repositories
{
    public interface ILgProjectActivityAuditTrailRepository
    {
        Task<Paged<LogHistoryPaged>> GetPaged(LogHistoryFilter filter, List<RoleEnum> roles, int page = 1, int size = 10, string sort = "dateModified desc");

        Task<LgProjectActivityAuditTrail> AddLog(LgProjectActivityAuditTrail entity);
        Task<List<LgProjectActivityAuditTrail>> GetLogByActivityDescription(string projectId, string activityDescription);
    }
}
