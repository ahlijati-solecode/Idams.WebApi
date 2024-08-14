using AutoMapper;
using Idams.Core.Constants;
using Idams.Core.Enums;
using Idams.Core.Model.Dtos;
using Idams.Core.Model.Entities;
using Idams.Core.Model.Filters;
using Idams.Core.Model.Requests;
using Idams.Core.Model.Responses;
using Idams.Core.Repositories;
using Idams.Core.Services;
using Idams.Infrastructure.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Idams.Infrastructure.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IConfiguration _configuration;
        private readonly IProjectActionRepository _projectActionRepository;
        private readonly ITemplateDocumentRepository _templateDocumentRepository;
        private readonly IRefWorkflowRepository _refWorkflowRepository;
        private readonly IParameterListRepository _parameterListRepository;
        private readonly IDocumentManagementRepository _documentManagementRepository;
        private readonly IUnitOfWorks _unitOfWorks;
        private readonly IMapper _mapper;
        private readonly IProjectRepository _projectRepository;

        public DocumentService(IDocumentRepository documentRepository,
            IConfiguration configuration,
            ITemplateDocumentRepository templateDocumentRepository,
            IProjectActionRepository projectActionRepository,
            IUnitOfWorks unitOfWorks,
            IRefWorkflowRepository refWorkflowRepository,
            IParameterListRepository parameterListRepository,
            IDocumentManagementRepository documentManagementRepository,
            IMapper mapper,
            IProjectRepository projectRepository)
        {
            _documentRepository = documentRepository;
            _configuration = configuration;
            _templateDocumentRepository = templateDocumentRepository;
            _projectActionRepository = projectActionRepository;
            _unitOfWorks = unitOfWorks;
            _refWorkflowRepository = refWorkflowRepository;
            _parameterListRepository = parameterListRepository;
            _documentManagementRepository = documentManagementRepository;
            _mapper = mapper;
            _projectRepository = projectRepository;
        }

        public async Task<Paged<MdDocumentPagedDto>> GetPaged(PagedDto pagedDto, MdDocumentFilter filter)
        {
            var result = await _documentRepository.GetPagedAsync(filter, pagedDto.Page, pagedDto.Size, pagedDto.Sort);
            return result;
        }

        public async Task<ProjectDocumentGroupResponse> GetDocumentGroupOfProjectAction(string projectActionId)
        {
            return await _documentRepository.GetDocumentGroupFromProjectAction(projectActionId);
        }

        public async Task<TxDocument> UploadRequiredDocument(UploadRequiredDocRequest uploadRequest)
        {
            var projectAction = await _projectActionRepository.GetAction(uploadRequest.ProjectActionId);
            if (projectAction == null)
                throw new Exception($"ProjectActionId {uploadRequest.ProjectActionId} not found");

            var projectHeader = await _projectRepository.GetLatestVersionProjectHeader(projectAction.ProjectId!);
            if (projectHeader == null)
                throw new Exception("project header is null");

            var refTemplateDoc = await _templateDocumentRepository.GetTemplateDocument(projectAction.WorkflowSequenceId!, projectAction.WorkflowActionId!);
            if (refTemplateDoc == null)
                throw new InvalidDataException($"Error refTempalteDocument of {uploadRequest.ProjectActionId} not found");

            using var transaction = _unitOfWorks.BeginTransaction();
            try
            {
                var doc = await SaveTxDocument(projectAction, refTemplateDoc, uploadRequest, projectHeader.CurrentWorkflowSequence);
                string dirPath = GenerateDirPath(projectAction.ProjectActionId);
                Directory.CreateDirectory(dirPath);
                string filePath = Path.Combine(dirPath, doc.TransactionDocId);
                if (File.Exists(filePath))
                    File.Delete(filePath);
                using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadRequest.File.CopyToAsync(fileStream);
                }
                await _unitOfWorks.SaveChangesAsync();
                await transaction.CommitAsync();
                return doc;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<TxDocument?> RenameDocument(string transactionDocId, string newName)
        {
            return await _documentRepository.RenameDocument(transactionDocId, RemoveIllegalCharFileName(newName));
        }

        public async Task<bool> DeleteDocument(string transactionDocId)
        {
            var doc = await _documentRepository.GetByTransactionId(transactionDocId);
            if (doc == null)
                return false;

            string filePath = GenerateFilePath(doc);
            File.Delete(filePath);
            return await _documentRepository.DeleteAsync(doc);
        }

        public async Task<DownloadDocumentDto?> GetDocumentDownloadDto(string transactionDocId)
        {
            var doc = await _documentRepository.GetByTransactionId(transactionDocId);
            if (doc == null)
                return null;

            DownloadDocumentDto ret = new();
            ret.FilePath = GenerateFilePath(doc);
            if (string.IsNullOrWhiteSpace(doc.DocName))
            {
                ret.FileName = RemoveIllegalCharFileName(doc?.DocDescription?.DocDescription ?? "Documents") +  " - " + doc!.ProjectAction!.WorkflowSequence!.WorkflowName + doc!.FileExtension;
            }
            else
            {
                ret.FileName = doc.DocName + " - " +doc!.ProjectAction!.WorkflowSequence!.WorkflowName+ doc!.FileExtension;
            }
            return ret;
        }

        public async Task<TxDocument> UploadSupportingDocument(UploadRequest uploadRequest)
        {
            var projectAction = await _projectActionRepository.GetAction(uploadRequest.ProjectActionId);
            if (projectAction == null)
                throw new Exception($"ProjectActionId {uploadRequest.ProjectActionId} not found");

            var projectHeader = await _projectRepository.GetLatestVersionProjectHeader(projectAction.ProjectId!);
            if (projectHeader == null)
                throw new Exception("project header is null");

            var refTemplateDoc = await _templateDocumentRepository.GetTemplateDocument(projectAction.WorkflowSequenceId!, projectAction.WorkflowActionId!);
            if (refTemplateDoc == null)
                throw new InvalidDataException($"Error refTempalteDocument of {uploadRequest.ProjectActionId} not found");

            string docDescriptionId = await GetDocDescriptionId(refTemplateDoc.WorkflowSequenceId);

            using var transaction = _unitOfWorks.BeginTransaction();
            try
            {
                var doc = ConstructNewDocument(projectAction, refTemplateDoc, docDescriptionId, projectHeader.CurrentWorkflowSequence);
                doc.DocName = RemoveIllegalCharFileName(Path.GetFileNameWithoutExtension(uploadRequest.File.FileName));
                doc = await SaveTxDocument(doc, uploadRequest);

                string dirPath = GenerateDirPath(projectAction.ProjectActionId);
                Directory.CreateDirectory(dirPath);
                string filePath = GenerateFilePath(doc);
                if (File.Exists(filePath))
                    File.Delete(filePath);
                using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadRequest.File.CopyToAsync(fileStream);
                }

                await _unitOfWorks.SaveChangesAsync();
                await transaction.CommitAsync();
                return doc;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private async Task<string> GetDocDescriptionId(string workflowSequenceId)
        {
            var workflow = await _refWorkflowRepository.GetByWorkflowSequenceId(workflowSequenceId);
            string docId;
            
            if (workflow?.WorkflowCategoryParId == ParIdConstant.Inisiasi)
            {
                docId = ParIdConstant.DokumenPendukungInisiasi;
            }
            else if (workflow?.WorkflowCategoryParId == ParIdConstant.Seleksi)
            {
                docId = ParIdConstant.DokumenPendukungSeleksi;
            }
            else
            {
                docId = ParIdConstant.DokumenPendukungKajianLanjut;
            }

            return docId;
        }

        private static string RemoveIllegalCharFileName(string input)
        {
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            input = r.Replace(input, "");
            return input;
        }

        private string GenerateFilePath(TxDocument document)
        {
            return Path.Combine(GenerateDirPath(document.ProjectActionId!), document.TransactionDocId);
        }

        private string GenerateDirPath(string projectActionId)
        {
            string basePath = _configuration.GetSection("FileServerMountPoint").Value;
            return Path.Combine(basePath, projectActionId);
        }

        private async Task<TxDocument> SaveTxDocument(TxProjectAction projectAction, RefTemplateDocument refTemplateDoc, UploadRequiredDocRequest uploadRequest, string? currentWorkflowSequence)
        {
            var doc = await _documentRepository.GetByProjectActionAndDocId(projectAction.ProjectActionId, uploadRequest.DocDescriptionId);
            if (doc == null)
            {
                var docDescription = await _documentRepository.GetDocumentDescription(uploadRequest.DocDescriptionId);
                doc = ConstructNewDocument(projectAction, refTemplateDoc, uploadRequest.DocDescriptionId, currentWorkflowSequence);
                doc.DocName = docDescription!.DocDescription;
            }
            else if (doc.LastUpdateWorkflowSequence != currentWorkflowSequence)
            {
                _documentRepository.SaveAsOldVersion(doc);
                var docDescription = await _documentRepository.GetDocumentDescription(uploadRequest.DocDescriptionId);
                var newVersionDoc = ConstructNewDocument(projectAction, refTemplateDoc, uploadRequest.DocDescriptionId, currentWorkflowSequence);
                newVersionDoc.DocName = docDescription!.DocDescription;
                doc = newVersionDoc;
            }
            
            return await SaveTxDocument(doc, uploadRequest);
        }

        public async Task<TxDocument> SaveTxDocument(TxDocument doc, UploadRequest uploadRequest)
        {
            doc.FileExtension = Path.GetExtension(uploadRequest.File.FileName);
            doc.FileSize = (int)uploadRequest.File.Length;

            return await _documentRepository.SaveAsync(doc);
        }

        private static TxDocument ConstructNewDocument(TxProjectAction projectAction, RefTemplateDocument refTemplateDoc, string docDescriptionId, string? currentWorkflowSequence)
        {
            TxDocument txDoc = new TxDocument();
            txDoc.ProjectActionId = projectAction.ProjectActionId;
            txDoc.TemplateId = refTemplateDoc.TemplateId;
            txDoc.TemplateVersion = refTemplateDoc.TemplateVersion;
            txDoc.ThresholdNameParId = refTemplateDoc.ThresholdNameParId;
            txDoc.ThresholdVersion = refTemplateDoc.ThresholdVersion;
            txDoc.WorkflowSequenceId = refTemplateDoc.WorkflowSequenceId;
            txDoc.WorkflowActionId = projectAction.WorkflowActionId;
            txDoc.DocDescriptionId = docDescriptionId;
            txDoc.LastUpdateWorkflowSequence = currentWorkflowSequence;
            txDoc.IsActive = true;
            txDoc.FileSizeUoM = "byte";
            return txDoc;
        }

        public async Task<GetDocumentDropdownDto> GetDocumentDropdown()
        {
            var document = await _parameterListRepository.GetParams("idams", "DocType");
            GetDocumentDropdownDto docDto = new GetDocumentDropdownDto();
            docDto.Documents = document.ToDictionary(keySelector: t => t.ParamListId, elementSelector: t => t.ParamValue1Text!);
            return docDto;

        }


        public async Task<Paged<DocumentManagementDto>> GetDocumentManagementPaged(PagedDto paged, TxDocumentFilter filter, UserDto user)
        {
            UserUtil.DetermineUserPrivelege(user, out string? hierLvl2, out string? hierLvl3);
            filter.HierLvl2 = hierLvl2;
            filter.HierLvl3 = hierLvl3;
            var res = await _documentManagementRepository.GetPaged(filter, paged.Page, paged.Size, paged.Sort);
            return _mapper.Map<Paged<DocumentManagementDto>>(res);
        }

        public async Task<List<DocumentManagementPreviewDto>> GetDocumentHistory(string transactionDocId)
        {
            var document = await _documentRepository.GetDocumentByTransactionDocId(transactionDocId);
            var res = await _documentRepository.GetDocumentByProjectAndDocDescription(document!.ProjectAction!.ProjectId!, document.DocDescription!.DocTypeParId!);
            return res;
        }

        public async Task<Paged<DocumentManagementDto>> GetDocumentManagementPagedByProject(PagedDto paged, TxDocumentFilter filter, string projectId)
        {
            var res = await _documentManagementRepository.GetPagedByProject(filter, paged.Page, paged.Size, paged.Sort, projectId);
            return _mapper.Map<Paged<DocumentManagementDto>>(res);
        }

    }
}
