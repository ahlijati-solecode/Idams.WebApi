using Idams.Core.Enums;
using Idams.Core.Model.Dtos;
using Idams.Core.Model.Entities;
using Idams.Core.Model.Entities.Custom;
using Idams.Core.Model.Filters;
using Idams.Core.Model.Requests;

namespace Idams.Core.Repositories
{
    public interface IWorkflowSettingRepository
    {
        Task<Paged<WorkflowSettingPaged>> GetPaged(WorkflowSettingFilter filter, List<RoleEnum> roles,  int page = 1, int size = 10, string sort = "id asc");

        Task<RefTemplate> GetByTemplateIdAndTemplateVersion(string? templateId, int? templateVersion);

        Task<RefTemplate?> GetByTemplateIdAndTemplateVersionWithTemplateDocument(string? templateId, int? templateVersion);

        Task<RefTemplate> UpdateRefTemplateAsync(RefTemplate template, RefTemplate model);

        Task<int> GetLatestTemplateVersion();

        Task<string> GetLastTemplateId();

        Task<int> GetThresholdVersion(string threshold);

        Task<GetWorkflowSettingDropdown> GetDropdownList();

        Task<GetScopeOfWorkDropdown> GetScopeOfWorkDropdownList();

        Task<RefTemplate> AddTemplateAsync(RefTemplate template);

        Task<RefTemplate?> DetermineTemplateByMultipleCategory(ProjectTemplateFilter projectTemplateFilter);

        Task<TemplateWithThresholdNameDto?> GetTemplateWithThesholdName(string? templateId, int? templateVersion);
        Task<bool> DeleteRefTemplate (RefTemplate template);
        Task<bool> CheckThresholdCategoryCriteriaSubCriteria(string? threshold, string? category, string? criteria, string? subcriteria);

    }
}
