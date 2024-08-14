using Idams.Core.Model.Dtos;
using Idams.Core.Model.Entities;

namespace Idams.Core.Repositories
{
    public interface IRefTemplateWorkflowSequenceRepository
    {
        Task<RefTemplateWorkflowSequence> UpdateRefTemplateWorkflowSequenceAsync(RefTemplateWorkflowSequence templateSequence, RefTemplateWorkflowSequence model);
        Task<RefTemplateWorkflowSequence> AddNewWorkflowSequence(RefTemplateWorkflowSequence data);

        Task<bool> DeleteWorkflowSequenceAsync(string workflowSequenceId);
        Task<RefTemplateWorkflowSequence> GetByWorkflowSequenceId(string workflowSequenceId);
        Task<string?> GetLastWorkflowSequnceId();
        Task<TemplateWorkflowSeqDto?> GetWorkflowSequence(string workflowSequenceId);
        Task<List<RefTemplateWorkflowSequence>> GetListTemplateWorkflowSequence(string? templateId, int? templateVersion);
        Task<bool> DeleteWorkflowSequence(List<RefTemplateWorkflowSequence> refTemplateWorkflowSequences);
    }
}
