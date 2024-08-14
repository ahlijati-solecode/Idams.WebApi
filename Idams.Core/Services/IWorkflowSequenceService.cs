using Idams.Core.Model.Dtos;
using Idams.Core.Model.Entities;
using Idams.Core.Model.Filters;
using Idams.Core.Model.Requests;

namespace Idams.Core.Services
{
    public interface IWorkflowSequenceService
    {
        Task<TemplateWorkflowSeqDto?> GetWorkflowSequece(string workflowSequenceId);
        Task<bool> DeleteWorkflowSequece(string workflowSequenceId);
        Task<bool> SaveDocumentChecklist(DocumentChecklistRequest param);
        Task<bool> DeleteDocumentChecklist(DocumentChecklistRequest param);
        Task<Paged<MpDocumentChecklistPagedDto>> GetDocumentChecklistPaged(PagedDto pagedDto, DocumentChecklistFilter filter, string DocGroupParId, int DocGroupVersion);
        Task<RefTemplateWorkflowSequence> AddorUpdateWorkflowSequece(TemplateWorkflowSeqRequest param);
    }
}
