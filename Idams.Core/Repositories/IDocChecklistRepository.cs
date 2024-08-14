using Idams.Core.Model.Dtos;
using Idams.Core.Model.Entities;
using Idams.Core.Model.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Repositories
{
    public interface IDocChecklistRepository
    {
        Task<Paged<MpDocumentChecklistPagedDto>> GetPaged(PagedDto paged, DocumentChecklistFilter filter, string DocGroupParId, int DocGroupVersion);
        Task<bool> SaveDocumentChecklist(string docGroupParId, int docVersion, List<MpDocumentChecklist> data);
        Task<bool> DeleteDocumentChecklist(string docGroupParId, int docVersion, List<MpDocumentChecklist> data);
        Task<List<MpDocumentChecklist>> GetDocumentCheckListByDocGroup(string? docGroupParId, int? docVersion);
        
    }
}
