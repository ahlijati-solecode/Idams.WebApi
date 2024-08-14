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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Idams.WebApi.Controllers
{
    [Route("DocumentSetting")]
    [ApiController]
    public class DocumentSettingController : ApiController
    {
        private readonly IDocumentService _documentService;
        private readonly IMapper _mapper;

        const string BadRequestMessage = "Bad Request";
        const string NotFoundMessage = "Not Found";
        const string Success = "Success";

        public DocumentSettingController(IDocumentService documentService, IMapper mapper)
        {
            _mapper = mapper;
            _documentService = documentService;
        }

        [HttpGet("documents")]
        [ApiAuthorize(MenuNameConstants.ProjectWorkflowSetting)]
        public async Task<object> GetPagedAsync([FromQuery] DocumentPagedRequest request)
        {
            try
            {
                var items = await _documentService.GetPaged(_mapper.Map<PagedDto>(request), _mapper.Map<MdDocumentFilter>(request));
                var res = _mapper.Map<Paged<MdDocumentPagedResponse>>(items);
                Message = Success;
                return Ok(res);
            }
            catch (Exception ex)
            {
                Message = BadRequestMessage;
                return Ok(false);
            }

        }

    }
}
