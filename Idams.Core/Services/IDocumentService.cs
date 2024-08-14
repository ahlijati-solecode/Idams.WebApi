using Idams.Core.Model.Dtos;
using Idams.Core.Model.Entities;
using Idams.Core.Model.Filters;
using Idams.Core.Model.Requests;
using Idams.Core.Model.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Services
{
    public interface IDocumentService
    {
        Task<Paged<MdDocumentPagedDto>> GetPaged(PagedDto pagedDto, MdDocumentFilter filter);
        Task<ProjectDocumentGroupResponse> GetDocumentGroupOfProjectAction(string projectActionId);
        Task<TxDocument> UploadRequiredDocument(UploadRequiredDocRequest uploadRequest);
        Task<TxDocument> UploadSupportingDocument(UploadRequest uploadRequest);
        Task<DownloadDocumentDto?> GetDocumentDownloadDto(string transactionDocId);
        Task<bool> DeleteDocument(string transactionDocId);
        Task<TxDocument?> RenameDocument(string transactionDocId, string newName);
        Task<GetDocumentDropdownDto> GetDocumentDropdown();
        Task<Paged<DocumentManagementDto>> GetDocumentManagementPaged(PagedDto dto, TxDocumentFilter filter, UserDto user);
        Task<List<DocumentManagementPreviewDto>> GetDocumentHistory(string transactionDocId);
        Task<Paged<DocumentManagementDto>> GetDocumentManagementPagedByProject(PagedDto dto, TxDocumentFilter filter, string projectId);

    }
}
