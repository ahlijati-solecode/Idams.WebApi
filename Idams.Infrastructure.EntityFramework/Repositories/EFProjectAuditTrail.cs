using Idams.Core.Constants;
using Idams.Core.Model.Entities;
using Idams.Core.Repositories;
using Idams.Core.Services;
using Idams.Infrastructure.EntityFramework.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Idams.Infrastructure.EntityFramework.Repositories
{
    public class EFProjectAuditTrail : BaseRepository, IProjectAuditTrailRepository
    {
        public EFProjectAuditTrail(IdamsDbContext dbContext, IConfiguration configuration, ICurrentUserService currentUserService)
            : base(configuration, currentUserService, dbContext)
        {
        }

        public async Task<LgProjectHeaderAuditTrail> SaveAuditTrailCreation(string projectId, int projectVersion, string section)
        {
            return await SaveAuditTrail(projectId, projectVersion, ProjectAuditConstant.CreationAuditTrail, section);
        }

        public async Task<LgProjectHeaderAuditTrail> SaveAuditTrailUpdateProjectInformation(string projectId, int projectVersion)
        {
            return await SaveAuditTrail(projectId, projectVersion, ProjectAuditConstant.UpdateProjectInformation);
        }

        public async Task<LgProjectHeaderAuditTrail> SaveAuditTrailUpdateScopeOfWork(string projectId, int projectVersion)
        {
            return await SaveAuditTrail(projectId, projectVersion, ProjectAuditConstant.UpdateScopeOfWork);
        }

        public async Task<LgProjectHeaderAuditTrail> SaveAuditTrailUpdateEconomicIndicator(string projectId, int projectVersion)
        {
            return await SaveAuditTrail(projectId, projectVersion, ProjectAuditConstant.UpdateEconomicIndicator);
        }

        public async Task<LgProjectHeaderAuditTrail> SaveAuditTrailUpdateInitiationDocs(string projectId, int projectVersion)
        {
            return await SaveAuditTrail(projectId, projectVersion, ProjectAuditConstant.UpdateProjectInitDoc);
        }

        public async Task<LgProjectHeaderAuditTrail> SaveAuditTrailUpdateMilestone(string projectId, int projectVersion)
        {
            return await SaveAuditTrail(projectId, projectVersion, ProjectAuditConstant.UpdateMilestone);
        }

        public async Task<string?> GetSection(string projectId, int projectVersion)
        {
            var audit = await _dbContext.LgProjectHeaderAuditTrails.Where(n => n.ProjectId == projectId &&
                            n.ProjectVersion == projectVersion &&
                            n.Action == ProjectAuditConstant.CreationAuditTrail)
                            .FirstOrDefaultAsync();
            return audit?.Section;
        }

        public async Task<Dictionary<string, DateTime?>> GetUpdateDate(string projectId, int projectVersion)
        {
            Dictionary<string, DateTime?> ret = new();
            var audit = await _dbContext.LgProjectHeaderAuditTrails.Where(n => n.ProjectId == projectId && n.ProjectVersion == projectVersion).ToListAsync();
            ret.Add(ProjectAuditConstant.UpdateProjectInformation, audit.FirstOrDefault(n => n.Action == ProjectAuditConstant.UpdateProjectInformation)?.UpdatedDate);
            ret.Add(ProjectAuditConstant.UpdateScopeOfWork, audit.FirstOrDefault(n => n.Action == ProjectAuditConstant.UpdateScopeOfWork)?.UpdatedDate);
            ret.Add(ProjectAuditConstant.UpdateResources, audit.FirstOrDefault(n => n.Action == ProjectAuditConstant.UpdateResources)?.UpdatedDate);
            ret.Add(ProjectAuditConstant.UpdateEconomicIndicator, audit.FirstOrDefault(n => n.Action == ProjectAuditConstant.UpdateEconomicIndicator)?.UpdatedDate);
            ret.Add(ProjectAuditConstant.UpdateProjectInitDoc, audit.FirstOrDefault(n => n.Action == ProjectAuditConstant.UpdateProjectInitDoc)?.UpdatedDate);
            ret.Add(ProjectAuditConstant.UpdateMilestone, audit.FirstOrDefault(n => n.Action == ProjectAuditConstant.UpdateMilestone)?.UpdatedDate);
            return ret;
        }

        private async Task<LgProjectHeaderAuditTrail> SaveAuditTrail(string projectId, int projectVersion, string action, string? section = null)
        {
            var audit = await _dbContext.LgProjectHeaderAuditTrails.Where(n => n.ProjectId == projectId &&
                            n.ProjectVersion == projectVersion &&
                            n.Action == action)
                            .FirstOrDefaultAsync();

            if (audit == null)
            {
                audit = new LgProjectHeaderAuditTrail
                {
                    ProjectId = projectId,
                    ProjectVersion = projectVersion,
                    Action = action,
                    Section = section,
                    UpdatedDate = DateTime.UtcNow,
                    UpdatedBy = GetCurrentUser
                };
                await _dbContext.LgProjectHeaderAuditTrails.AddAsync(audit);
            }
            else
            {
                audit.Section = section;
                audit.UpdatedDate = DateTime.UtcNow;
                audit.UpdatedBy = GetCurrentUser;
            }
            await _dbContext.SaveChangesAsync();
            return audit;
        }

        public async Task<LgProjectHeaderAuditTrail> SaveAuditTrailUpdateResources(string projectId, int projectVersion)
        {
            return await SaveAuditTrail(projectId, projectVersion, ProjectAuditConstant.UpdateResources);
        }
    }
}
