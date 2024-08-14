using Idams.Core.Model.Dtos;
using Idams.Core.Model.Entities;
using Idams.Core.Model.Entities.Custom;
using Idams.Core.Model.Filters;
using Idams.Core.Model.Requests;
using Idams.Core.Model.Responses;

namespace Idams.Core.Repositories
{
    public interface IProjectRepository
    {
        Task<TxProjectHeader?> GetProjectHeader(string projectId, int projectVersion);
        Task<TxProjectHeader?> GetLatestVersionProjectHeader(string projectId);
        Task<Paged<ProjectListPaged>> GetPaged(ProjectListFilter filter, int page, int size, string sort);
        Task<IEnumerable<ProjectBannerCount>> GetBannerCount(string? hierlvl2, string? hierlvl3);
        Task<int> DeleteProject(string projectId, int projectVersion);
        Task<int> DeleteProject(List<ProjectHeaderIdentifier> projects);
        Task<TxProjectHeader> AddAsync(TxProjectHeader tph);
        Task<TxProjectHeader> AddProject(TxProjectHeader tph);
        TxProjectHeader Update(TxProjectHeader tph);
        Task<ProjectSequenceTimelineResponse?> GetProjectSequence(string projectId, int projectVersion);
        Task<List<TxProjectMilestone>> GetMilestone(string projectId, int projectVersion);
        Task<GetScopeOfWorkDto?> GetScopeOfWork(string projectId, int projectVersion);
        Task<List<TxProjectHeader>> GetAllExceptDraftProject();
        Task<RegionalZonaThresholdData?> GetThresholdRegionalZonaByProjectId(string projectId);        
    }
}
