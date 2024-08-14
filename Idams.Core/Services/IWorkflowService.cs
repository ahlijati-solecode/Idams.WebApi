using Idams.Core.Enums;
using Idams.Core.Model.Dtos;
using Idams.Core.Model.Entities;
using Idams.Core.Model.Filters;
using Idams.Core.Model.Requests;
using Idams.Core.Model.Responses;

namespace Idams.Core.Services
{
     public interface IWorkflowService
    {
        Task<Paged<WorkflowSettingPagedDto>> GetPaged(PagedDto pagedDto, WorkflowSettingFilter filter, List<RoleEnum> roles);

        Task<bool> DeleteWorkflowSettingAsync(string templateId, int? templateVersion);
        Task<List<RefWorkflowDto>> GetWorkflowType(string CategoryParId);

        Task<GetWorkflowSettingDropdownDto> GetDropdownListAsync();

        Task<RefTemplateDto?> AddWorkflowSettingAsync(RefTemplateDto dto, List<RefTemplateWorkflowSequenceDto> sequenceDtos);

        Task<RefTemplateDto?> GetWorkflowSettingByTemplateAsync(string? templateId, int? templateVersion);
        Task<bool> SaveAllChanges(string templateId, int templateVersion);
        Task<RefTemplateDto> GenerateShadowTemplate(string templateId, int templateVersion);
    }
}
