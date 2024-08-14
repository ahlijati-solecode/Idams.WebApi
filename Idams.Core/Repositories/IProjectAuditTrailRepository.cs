using Idams.Core.Model.Entities;

namespace Idams.Core.Repositories
{
    public interface IProjectAuditTrailRepository
    {
        Task<LgProjectHeaderAuditTrail> SaveAuditTrailCreation(string projectId, int projectVersion, string section);
        Task<LgProjectHeaderAuditTrail> SaveAuditTrailUpdateProjectInformation(string projectId, int projectVersion);
        Task<LgProjectHeaderAuditTrail> SaveAuditTrailUpdateScopeOfWork(string projectId, int projectVersion);
        Task<LgProjectHeaderAuditTrail> SaveAuditTrailUpdateEconomicIndicator(string projectId, int projectVersion);
        Task<LgProjectHeaderAuditTrail> SaveAuditTrailUpdateInitiationDocs(string projectId, int projectVersion);
        Task<LgProjectHeaderAuditTrail> SaveAuditTrailUpdateMilestone(string projectId, int projectVersion);
        Task<string?> GetSection(string projectId, int projectVersion);
        Task<Dictionary<string, DateTime?>> GetUpdateDate(string projectId, int projectVersion);
        Task<LgProjectHeaderAuditTrail> SaveAuditTrailUpdateResources(string projectId, int projectVersion);
    }
}
