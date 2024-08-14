using Idams.Core.Model.Dtos;
using Idams.Core.Model.Entities;
using Idams.Core.Model.Filters;
using Idams.Core.Model.Responses;

namespace Idams.Core.Repositories
{
    public interface IDocumentRepository
    {
        Task<Paged<MdDocumentPagedDto>> GetPagedAsync(MdDocumentFilter filter, int page = 1, int size = 10, string sort = "id asc");
        Task<ProjectDocumentGroupResponse> GetDocumentGroupFromProjectAction(string projectActionId);
        Task<TxDocument> SaveAsync(TxDocument document);
        TxDocument SaveAsOldVersion(TxDocument document);
        Task<TxDocument?> GetByProjectActionAndDocId(string projectActionId, string docDescriptionId);
        Task<TxDocument?> GetByTransactionId(string transactionId);
        Task<TxDocument?> RenameDocument(string transactionId, string newName);
        Task<bool> DeleteAsync(TxDocument doc);
        Task<MdDocumentDescription?> GetDocumentDescription(string docDescriptionId);
        Task<TxDocument?> GetDocumentByTransactionDocId(string transactionDocId);
        Task<List<DocumentManagementPreviewDto>> GetDocumentByProjectAndDocDescription(string projectId, string docTypeParId);
    }
}
