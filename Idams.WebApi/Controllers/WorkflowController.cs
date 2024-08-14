using AutoMapper;
using Idams.Core.Enums;
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

namespace Idams.WebApi.Controllers
{
    [Route("WorkflowSetting")]
    [ApiController]
    public class WorkflowController : ApiController
    {
        private readonly IWorkflowService _workflowService;
        private readonly IWorkflowSequenceService _workflowSequenceService;
        private readonly IMapper _mapper;

        const string BadRequestMessage = "Bad Request";
        const string NotFoundMessage = "Not Found";
        const string Success = "Success";
        public WorkflowController(IWorkflowService workflowService, IMapper mapper, IWorkflowSequenceService workflowSequenceService)
        {
            _mapper = mapper;
            _workflowService = workflowService;
            _workflowSequenceService = workflowSequenceService;
        }

        [HttpGet("workFlowSettingList")]
        [ApiAuthorize(MenuNameConstants.ProjectWorkflowSetting)]
        public async Task<object> GetPagedAsync([FromQuery] WorkflowSettingPagedRequest request)
        {
            List<RoleEnum> listRole = HttpContext.GetUserInfo().Roles.Select(r => r.Enum).ToList();
            
            var items = await _workflowService.GetPaged(_mapper.Map<PagedDto>(request), _mapper.Map<WorkflowSettingFilter>(request), listRole);
            var res = _mapper.Map<Paged<WorkflowSettingPagedResponse>>(items);
            Message = ApiConstant.SuccessGetRefTemplate;
            return Ok(res);

        }

        [HttpDelete]
        [ApiAuthorize(MenuNameConstants.ProjectWorkflowSetting)]
        public async Task<IActionResult> DeleteAsync([FromQuery] RefTemplateRequest request)
        {
            if (request.TemplateId == null || request.TemplateVersion == null)
            {
                Message = BadRequestMessage;
                return BadRequest(false);
            }
            var result = await _workflowService.DeleteWorkflowSettingAsync(request.TemplateId, request.TemplateVersion);
            if (result == false)
            {
                Message = NotFoundMessage;
                return NotFound(result);
            }
            Message = ApiConstant.SuccessDeleteRefTemplate;
            return Ok(result);
        }

        [HttpGet("DropdownList")]
        [ApiAuthorize(MenuNameConstants.ProjectWorkflowSetting, MenuNameConstants.ProjectList)]
        public async Task<IActionResult> GetDropdownListAsync()
        {
            var result = await _workflowService.GetDropdownListAsync();
            Message = ApiConstant.SuccessGetDropdown;
            return Ok(_mapper.Map<GetWorkflowSettingDropdownResponse>(result));
        }

        [HttpPost("save")]
        [ApiAuthorize(MenuNameConstants.ProjectWorkflowSetting)]
        public async Task<IActionResult> AddWorkflowSettingAsync([FromBody] AddWorkflowSettingRequest request)
        {
            var result = await _workflowService.AddWorkflowSettingAsync(_mapper.Map<RefTemplateDto>(request), _mapper.Map<List<RefTemplateWorkflowSequenceDto>>(request.WorkflowSequence));
            if (result == null) return BadRequest("Cannot Add Template");
            Message = Success;
            return Ok(_mapper.Map<AddWorkflowSettingResponse>(result));
        }

        [HttpGet("saveAllChanges")]
        [ApiAuthorize(MenuNameConstants.ProjectWorkflowSetting)]
        public async Task<IActionResult> SaveAllChanges(string templateId, int templateVersion)
        {
            var res = await _workflowService.SaveAllChanges(templateId, templateVersion);
            Message = Success;
            return Ok(new { templateId, templateVersion });
        }

        [HttpGet("generateShadowTemplate")]
        [ApiAuthorize(MenuNameConstants.ProjectWorkflowSetting)]
        public async Task<IActionResult> GenerateShadowTemplate([FromQuery] RefTemplateRequest request)
        {
            var res = await _workflowService.GenerateShadowTemplate(request.TemplateId!, request.TemplateVersion!.Value);
            Message = Success;
            return Ok(new {res.TemplateId, res.TemplateVersion});
        }

        [HttpGet("workflowSetting")]
        [ApiAuthorize(MenuNameConstants.ProjectWorkflowSetting)]
        public async Task<IActionResult> GetWorkflowSettingByTemplate([FromQuery] RefTemplateRequest request)
        {
            var result = await _workflowService.GetWorkflowSettingByTemplateAsync(request.TemplateId, request.TemplateVersion);
            if(result == null)
            {
                return NotFound("Cannot Find Template");
            }
            var item = _mapper.Map<GetWorkflowSettingResponse>(result);
            Message = ApiConstant.SuccessGetRefTemplate;
            return Ok(item);
        }


        [HttpGet("workflowtype")]
        [ApiAuthorize(MenuNameConstants.ProjectWorkflowSetting)]
        public async Task<IActionResult> GetWorkFlowTypeAsync([FromQuery] string? stage = null)
        {
            var workFlowType = await _workflowService.GetWorkflowType(stage);
            var res = workFlowType.Select(x => _mapper.Map<RefWorkflowResponse>(x));
            Message = Success;
            return Ok(res);
        }

        [HttpPost("saveWorkflowSequence")]
        [ApiAuthorize(MenuNameConstants.ProjectWorkflowSetting)]
        public async Task<IActionResult> SaveWorkflowSequence([FromBody] TemplateWorkflowSeqRequest param)
        {
            var workFlowSeq = await _workflowSequenceService.AddorUpdateWorkflowSequece(param);
            Message = Success;
            return Ok(_mapper.Map<TemplateWorkflowSeqResponse>(workFlowSeq));
        }

        [HttpDelete("workflowSequence")]
        [ApiAuthorize(MenuNameConstants.ProjectWorkflowSetting)]
        public async Task<IActionResult> DeleteWorflowSequence([FromQuery] string workflowSequenceId)
        {
            if (workflowSequenceId == null)
            {
                Message = BadRequestMessage;
                return BadRequest(false);
            }
            var result = await _workflowSequenceService.DeleteWorkflowSequece(workflowSequenceId);
            if (result == false)
            {
                Message = NotFoundMessage;
                return NotFound(result);
            }
            Message = ApiConstant.SuccessDeleteWorkflowSequence;
            return Ok(result);
        }

        [HttpGet("workflowSequence")]
        [ApiAuthorize(MenuNameConstants.ProjectWorkflowSetting)]
        public async Task<IActionResult> GetWorkflowSequence([FromQuery] string workflowSequenceId)
        {

            var workFlowSeq = await _workflowSequenceService.GetWorkflowSequece(workflowSequenceId);
            Message = Success;
            return Ok(_mapper.Map<GetTemplateWorkflowSeqResponse>(workFlowSeq));

        }

        [HttpPost("saveDocumentChecklist")]
        [ApiAuthorize(MenuNameConstants.ProjectWorkflowSetting)]
        public async Task<IActionResult> SaveDocumentChecklist([FromBody] DocumentChecklistRequest param)
        {

            var docChecklist = await _workflowSequenceService.SaveDocumentChecklist(param);
            Message = Success;
            return Ok(docChecklist);

        }

        [HttpPost("deleteDocumentChecklist")]
        [ApiAuthorize(MenuNameConstants.ProjectWorkflowSetting)]
        public async Task<IActionResult> DeleteDocumentChecklist([FromBody] DocumentChecklistRequest param)
        {
            var docChecklist = await _workflowSequenceService.DeleteDocumentChecklist(param);
            Message = Success;
            return Ok(docChecklist);
        }

        [HttpGet("sequenceDocumentChecklist")]
        [ApiAuthorize(MenuNameConstants.ProjectWorkflowSetting)]
        public async Task<IActionResult> GetDocumentChecklist([FromQuery] DocChecklistPagedRequest request, 
            [FromQuery] string DocGroupParId, [FromQuery] int DocGroupVersion)
        {
            var workFlowSeq = await _workflowSequenceService.GetDocumentChecklistPaged(_mapper.Map<PagedDto>(request), _mapper.Map<DocumentChecklistFilter>(request), DocGroupParId, DocGroupVersion);
            Message = Success;
            return Ok(_mapper.Map<Paged<MpDocumentChecklistPagedResponse>>(workFlowSeq));

        }
    }
}
