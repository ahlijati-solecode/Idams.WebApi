using AutoMapper;
using Idams.Core.Model.Dtos;
using Idams.Core.Model.Entities;
using Idams.Core.Model.Filters;
using Idams.Core.Model.Requests;
using Idams.Core.Model.Responses;
using Idams.Core.Services;
using Idams.WebApi.Constants;
using Idams.WebApi.Utils;
using Idams.WebApi.Utils.Attributes;
using Idams.WebApi.Utils.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.IO.Compression;

namespace Idams.WebApi.Controllers
{
    [Route("Document")]
    [ApiController]
    public class DocumentController : ApiController
    {
        private readonly IDocumentService _documentService;
        private readonly IMapper _mapper;
        private readonly ILogger<DocumentController> _logger;

        const string Success = "Success";

        public DocumentController(IDocumentService documentService, IMapper mapper, ILogger<DocumentController> logger)
        {
            _documentService = documentService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet("projectAction")]
        [ApiAuthorize()]
        public async Task<object> GetDocumentGroupOfProjectAction(string projectActionId)
        {
            var res = await _documentService.GetDocumentGroupOfProjectAction(projectActionId);
            Message = Success;
            return Ok(res);
        }

        [HttpPost("UploadRequiredDoc")]
        [ApiAuthorize()]
        public async Task<object> UploadRequiredDocument([FromForm]UploadRequiredDocRequest request)
        {
            var res = await _documentService.UploadRequiredDocument(request);
            Message = Success;
            return Ok(_mapper.Map<DocumentDto>(res));
        }

        [HttpPost("UploadSupportingDoc")]
        [ApiAuthorize()]
        public async Task<object> UploadSupportingDocument([FromForm] UploadRequest request)
        {
            var res = await _documentService.UploadSupportingDocument(request);
            Message = Success;
            return Ok(_mapper.Map<DocumentDto>(res));
        }

        [HttpDelete("delete")]
        [ApiAuthorize()]
        public async Task<object> DeleteDocument(string transactionDocId)
        {
            var res = await _documentService.DeleteDocument(transactionDocId);
            Message = Success;
            return Ok(res);
        }

        [HttpPut("rename")]
        [ApiAuthorize()]
        public async Task<object> RenameDocument(string transactionDocId, string newName)
        {
            var res = await _documentService.RenameDocument(transactionDocId, newName);
            if (res == null)
                return NotFound();
            Message = Success;
            return Ok(_mapper.Map<DocumentDto>(res));
        }

        [HttpGet("download")]
        [ApiAuthorize()]
        public async Task<object> DownloadDocument(string transactionDocId)
        {
            var res = await _documentService.GetDocumentDownloadDto(transactionDocId);
            if (res == null)
                return NotFound();

            FileInfo fileInfo = new FileInfo(res.FilePath);
            if (fileInfo.Exists)
            {
                string nani = "nani";
            }
            var bytes = await System.IO.File.ReadAllBytesAsync(res.FilePath);
            return File(bytes, GetContentType(res.FileName), res.FileName);
        }

        private string GetContentType(string fileName)
        {
            _ = new FileExtensionContentTypeProvider().TryGetContentType(fileName, out string contentType);
            return contentType ?? "application/octet-stream";
        }

        [HttpGet("dropdown")]
        [ApiAuthorize(MenuNameConstants.DocumentManagement)]
        public async Task<IActionResult> GetDocumentDropdown()
        {
            var res = await _documentService.GetDocumentDropdown();
            Message = Success;
            return Ok(_mapper.Map<GetDocumentDropdownResponse>(res));
        }
        [HttpPost("list")]
        [ApiAuthorize(MenuNameConstants.DocumentManagement)]
        public async Task<IActionResult> GetDocumentList([FromBody] DocumentManagementPagedRequest request)
        {
            var userInfo = HttpContext.GetUserInfo();
            var res = await _documentService.GetDocumentManagementPaged(_mapper.Map<PagedDto>(request), _mapper.Map<TxDocumentFilter>(request), userInfo);
            Message = Success;
            return Ok(_mapper.Map<Paged<DocumentManagementResponse>>(res));
        }

        [HttpPost("download")]
        [ApiAuthorize(MenuNameConstants.DocumentManagement)]
        public async Task<object> DownloadDocuments([FromBody] List<string> transactionDocId)
        {
            if(transactionDocId.Count == 1)
            {
                var res = await _documentService.GetDocumentDownloadDto(transactionDocId[0]);
                if (res == null)
                    return NotFound();

                FileInfo fileInfo = new FileInfo(res.FilePath);
                if (fileInfo.Exists)
                {
                    string nani = "nani";
                }
                _logger.LogInformation("FILENAME = {0}", res.FileName);
                var bytes = await System.IO.File.ReadAllBytesAsync(res.FilePath);
                return File(bytes, GetContentType(res.FileName), res.FileName);
            }
            else
            {
                byte[] compressedBytes;
                using (var ms = new MemoryStream())
                {
                    using(ZipArchive zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
                    {
                        foreach (var item in transactionDocId)
                        {
                            var res = await _documentService.GetDocumentDownloadDto(item);
                            FileInfo fileInfo = new FileInfo(res!.FilePath);
                            if (!fileInfo.Exists)
                            {
                                return NotFound("File not Found");
                            }
                            //rename doc name to avoid file become directory
                            if (res.FileName.Contains('/'))
                            {
                                res.FileName = res.FileName.Replace('/', '-');
                            }
                            var fileInArchive = zip.CreateEntry(res.FileName, CompressionLevel.Optimal);
                            var bytes = await System.IO.File.ReadAllBytesAsync(res.FilePath);

                            using var entryStream = fileInArchive.Open();
                            using (var fileToCompressStream = new MemoryStream(bytes)){
                                fileToCompressStream.CopyTo(entryStream);
                            }
                        }
                    }
                    compressedBytes = ms.ToArray();
                }
                return File(compressedBytes, "application/zip", "documents.zip");
            }

        }

        [HttpGet("history")]
        [ApiAuthorize(MenuNameConstants.DocumentManagement)]
        public async Task<IActionResult> GetHistoryDocument(string transactionDocId)
        {
            var res = await _documentService.GetDocumentHistory(transactionDocId);
            Message = Success;
            return Ok(_mapper.Map<List<DocumentManagementPreviewResponse>>(res));
        }

        [HttpPost("listbyproject")]
        [ApiAuthorize(MenuNameConstants.DocumentManagement)]
        public async Task<IActionResult> GetDocumentList(string projectId, [FromBody] DocumentManagementPagedRequest request)
        {
            var res = await _documentService.GetDocumentManagementPagedByProject(_mapper.Map<PagedDto>(request), _mapper.Map<TxDocumentFilter>(request), projectId);
            Message = Success;
            return Ok(_mapper.Map<Paged<DocumentManagementResponse>>(res));
        }

    }
}
