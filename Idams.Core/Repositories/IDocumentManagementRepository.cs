using Idams.Core.Model.Entities;
using Idams.Core.Model.Entities.Custom;
using Idams.Core.Model.Filters;

namespace Idams.Core.Repositories
{
    public interface IDocumentManagementRepository
    {
        Task<Paged<DocumentManagementPaged>> GetPaged(TxDocumentFilter filter, int page, int size, string sort);  
        Task<Paged<DocumentManagementPaged>> GetPagedByProject(TxDocumentFilter filter, int page, int size, string sort, string projectId);  
    }
}
