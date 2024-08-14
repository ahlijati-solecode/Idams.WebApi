using Idams.Core.Model.Entities;

namespace Idams.Core.Repositories
{
    public interface ITemplateDocumentRepository
    {
        Task<RefTemplateDocument?> GetTemplateDocument(string workflowSequenceId, string workflowActionId);
        Task<List<RefTemplateDocument>> GetTemplateDocumentByTemplateIdAndTemplateVersion(string templateId, int templateVersion);
        Task<bool> DeleteDocGroup(string docGroupParId, int docGroupVersion);
        Task<bool> DeleteRefTemplateDocument(RefTemplateDocument refTemplateDocument);
    }
}
