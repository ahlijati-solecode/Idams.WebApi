using AutoMapper;
using Idams.Core.Constants;
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
    [Route("project")]
    [ApiController]
    public class ProjectController : ApiController
    {
        private readonly IMapper _mapper;
        private readonly IProjectService _projectService;
        private readonly IUserService _userService;
        public const string Success = "Success";

        public ProjectController(IProjectService projectService, IMapper mapper, IUserService userService)
        {
            _projectService = projectService;
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet("list")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<object> GetPagedAsync([FromQuery] ProjectListPagedRequest request)
        {
            var userInfo = HttpContext.GetUserInfo();
            var res = await _projectService.GetPaged(_mapper.Map<PagedDto>(request), _mapper.Map<ProjectListFilter>(request), userInfo!);
            Message = Success;
            return Ok(res);
        }

        [HttpDelete("delete")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<object> DeleteProject(string projectId, int projectVersion)
        {
            var res = await _projectService.DeleteProject(projectId, projectVersion);
            Message = Success;
            return Ok(res);
        }

        [HttpPost("delete")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<object> DeleteMultipleProject([FromBody] MultipleProjectHeader param)
        {
            var res = await _projectService.DeleteProject(param.Projects);
            Message = Success;
            return Ok(res);
        }

        [HttpGet("banner")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<object> GetBannerDataCountAsync()
        {
            var userInfo = HttpContext.GetUserInfo();
            var res = await _projectService.GetBannerCount(userInfo);
            Message = Success;
            return Ok(res);
        }

        [HttpGet("dropdown")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<object> GetProjectFilterDropdownAsync()
        {
            var res = await _projectService.GetDropdownList();
            Message = Success;
            return Ok(res);
        }

        [HttpGet("tableSetting")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<object> GetTableSetting()
        {
            var userInfo = HttpContext.GetUserInfo();
            var res = await _projectService.GetProjectListTableConfig(userInfo.EmpAccount);
            Message = Success;
            return Ok(res);
        }

        [HttpPost("tableSetting")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<object> SaveTableSetting([FromBody] TableColumnSettingRequest tableSetting)
        {
            var userInfo = HttpContext.GetUserInfo();
            var res = await _projectService.SaveProjectListTableConfig(userInfo.EmpAccount, tableSetting.Setting);
            Message = Success;
            return Ok(res);
        }

        [HttpGet("availableHierlvl2")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<object> GetAvailableHierLvl2()
        {
            var userInfo = HttpContext.GetUserInfo();
            var res = await _projectService.GetAvailableHierLvl2(userInfo);
            Message = Success;
            return Ok(res);
        }

        [HttpPost("availableHierlvl2")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<object> GetAvailableHierLvl2s([FromBody] GetAvailableHierRequest request)
        {
            var userInfo = HttpContext.GetUserInfo();
            var res = await _projectService.GetAvailableHierLvl2(userInfo, request.HierLvl3s);
            Message = Success;
            return Ok(res);
        }

        [HttpGet("availableHierlvl3")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<object> GetAvailableHierLvl3(string hierlvl2)
        {
            var userInfo = HttpContext.GetUserInfo();
            var res = await _projectService.GetAvailableHierLvl3(userInfo, hierlvl2);
            Message = Success;
            return Ok(res);
        }

        [HttpPost("availableHierlvl3")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<object> GetAvailableHierLvl3s([FromBody] GetAvailableHierRequest request)
        {
            var userInfo = HttpContext.GetUserInfo();
            var res = await _projectService.GetAvailableHierLvl3(userInfo, request.HierLvl2s);
            Message = Success;
            return Ok(res);
        }

        [HttpGet("availableHierlvl4")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<object> GetAvailableHierLvl4(string hierlvl3)
        {
            var userInfo = HttpContext.GetUserInfo();
            var res = await _projectService.GetAvailableHierLvl4(hierlvl3);
            Message = Success;
            return Ok(res);
        }

        [HttpPost("availableWorkflowType")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<object> GetAvailableWorkflowType([FromBody] GetAvailableStageWorkflowTypeRequest request)
        {
            var res = await _projectService.GetAvailableWorkflowType(request.Stage);
            Message = Success;
            return Ok(res);
        }

        [HttpPost("availableStage")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<object> GetAvailableStage([FromBody] GetAvailableStageWorkflowTypeRequest request)
        {
            var res = await _projectService.GetAvaiableStage(request.WorkflowType);
            Message = Success;
            return Ok(res);
        }

        [HttpGet("determineTemplate")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<object> DetermineUsedTemplate([FromQuery] ProjectTemplateFilter projectTemplateFilter)
        {
            var res = await _projectService.DetermineUsedTemplate(projectTemplateFilter);
            Message = Success;
            return Ok(res);
        }

        [HttpPost("add")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<object> AddNewProject([FromBody] ProjectInformationRequest projectInformationRequest)
        {
            var tph = _mapper.Map<TxProjectHeader>(projectInformationRequest);
            var res = await _projectService.AddNewProject(tph, projectInformationRequest.EntityIds, projectInformationRequest.Section, projectInformationRequest.SaveLog);
            Message = Success;
            return Ok(new { res.ProjectId, res.ProjectVersion });
        }

        [HttpPost("updateProjectInformation")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<object> UpdateProjectInformation([FromBody] UpdateProjectInformationRequest request)
        {
            var res = await _projectService.UpdateProjectInformation(request);
            Message = "Success";
            return Ok(new { res.ProjectId, res.ProjectVersion });
        }

        [HttpPost("lockTemplate")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<object> LockProjectTemplate(string projectId, int projectVersion)
        {
            var res = await _projectService.PopulateMilestone(projectId, projectVersion);
            Message = "Success";
            return Ok(new { res.ProjectId, res.ProjectVersion });
        }

        [HttpPost("updateScopeOfWork")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<IActionResult> UpdateScopeOfWork([FromBody] UpdateScopeOfWorkRequest request)
        {
            var res = await _projectService.UpdateScopeOfWork(_mapper.Map<UpdateScopeOfWorkDto>(request));
            if (res == null) return NotFound("Project Not Found");
            Message = Success;
            return Ok(new { res.ProjectId, res.ProjectVersion });
        }

        [HttpPost("updateEconomicIndicator")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<IActionResult> UpdateEconomicIndicator([FromBody] UpdateEconomicIndicatorRequest request)
        {
            var res = await _projectService.UpdateEconomicIndicator(request);
            if (res == null) return NotFound("Project Not Found");
            Message = Success;
            return Ok(new { res.ProjectId, res.ProjectVersion });
        }

        [HttpPost("updateInitiationDocs")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<IActionResult> UpdateInitiationDocs([FromBody] UpdateInitiationDocsRequest request)
        {
            var res = await _projectService.UpdateInitiationDocuments(request);
            if (res == null) return NotFound("Project Not Found");
            Message = Success;
            return Ok(new { res.ProjectId, res.ProjectVersion });
        }

        [HttpGet("dropdownScopeOfWork")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<IActionResult> GetDropdownScopeOfWork()
        {
            var res = await _projectService.GetDropdownAsync();
            Message = Success;
            return Ok(_mapper.Map<GetScopeOfWorkDropdownResponse>(res));
        }

        [HttpGet("scopeOfWork")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<IActionResult> GetScopeOfWork(string projectId, int projectVersion)
        {
            var res = await _projectService.GetScopeOfWork(projectId, projectVersion);
            if (res == null) return NotFound("Project Not Found");
            Message = "Success";
            return Ok(_mapper.Map<GetScopeOfWorkResponse>(res));
        }

        [HttpPost("updateResources")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<IActionResult> UpdateProjectResources([FromBody] UpdateResourcesRequest request)
        {
            var res = await _projectService.UpdateResources(_mapper.Map<UpdateResourcesDto>(request));
            if (res == null) return NotFound("Project Not Found");
            Message = Success;
            return Ok(new {res.ProjectId, res.ProjectVersion});
        }

        [HttpGet("detail")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<object> GetProjectDetail(string projectId, int projectVersion)
        {
            var res = await _projectService.GetProjectDetail(projectId, projectVersion);
            Message = Success;
            return Ok(res);
        }

        [HttpGet("sequences")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<object> GetProjectSequence(string projectId, int projectVersion)
        {
            var res = await _projectService.GetProjectSequence(projectId, projectVersion);
            if (res == null) return NotFound("Project Not Found");
            Message = Success;
            return Ok(res);
        }

        [HttpGet("sequenceAction")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<object> GetProjectSequenceAction(string projectId, string workflowSequenceId)
        {
            var res = await _projectService.GetProjectSequenceAction(projectId, workflowSequenceId);
            Message = Success;
            return Ok(res);
        }

        [HttpGet("milestone")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<IActionResult> GetMilestone(string projectId, int projectVersion)
        {
            var res = await _projectService.GetMilestone(projectId, projectVersion);
            if (res == null) return NotFound("Project Not Found");
            Message = Success;
            return Ok(_mapper.Map<GetMilestoneResponse>(res));
        }

        [HttpPost("updateMilestone")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<IActionResult> AddMilestone([FromBody] UpdateMilestoneRequest request)
        {
            var res = await _projectService.UpdateMilestone(_mapper.Map<UpdateMilestoneDto>(request));
            if (res == null) return NotFound("Project Not Found");
            Message = Success;
            return Ok(new {res.ProjectId, res.ProjectVersion});
        }

        [HttpPost("initiate")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<IActionResult> InitiateProject(string projectId, int projectVersion)
        {
            var res = await _projectService.InitiateProject(projectId, projectVersion);
            return Ok(new {ProjectId = projectId, ProjectVersion = projectVersion});
        }

        [HttpGet("updateData")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<IActionResult> GetUpdateDataStatusDetail(string projectActionId)
        {
            var res = await _projectService.GetUpdateDataStatus(projectActionId);
            Message = Success;
            return Ok(res);
        }

        [HttpPost("updateData")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<IActionResult> PostUpdateDataStatusDetail(string projectActionId, bool complete)
        {
            var res = await _projectService.CompleteWorkflowUpdatedata(projectActionId, complete);
            Message = Success;
            return Ok(res);
        }

        [HttpGet("followUpDropdown")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<IActionResult> GetFollowUpDropdown()
        {
            var res = await _projectService.GetFollowUpDropdownList();
            Message = Success;
            return Ok(res);
        }

        [HttpGet("followup")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<IActionResult> GetFollowUpList(string projectActionId)
        {
            var res = await _projectService.GetFollowUpList(projectActionId);
            Message = Success;
            return Ok(res);
        }

        [HttpPost("followup")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<IActionResult> AddFollowUp([FromBody] List<FollowUpRequest> data)
        {
            var res = await _projectService.AddFollowUpList(data);
            Message = Success;
            return Ok(res);
        }

        [HttpPost("updatefollowup")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<IActionResult> UpdateFollowUp([FromBody] FollowUpRequest data)
        {
            var res = await _projectService.UpdateFollowUp(data);
            Message = Success;
            return Ok(res);
        }

        [HttpDelete("followup")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<IActionResult> DeleteFollowUp(int followUpId, string projectActionId)
        {
            var res = await _projectService.DeleteFollowUp(followUpId, projectActionId);
            Message = Success;
            return Ok(res);
        }

        [HttpGet("approval")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<IActionResult> GetApprovalDetail(string projectActionId)
        {
            var res = await _projectService.GetApprovalDetail(projectActionId);
            Message = Success;
            return Ok(res);
        }

        [HttpPost("approval")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<IActionResult> UpdateApproval([FromBody] ApprovalRequest approvalRequest)
        {
            var res = await _projectService.UpdateApprovalData(approvalRequest);
            Message = Success;
            return Ok(res);
        }

        [HttpGet("lastApproval")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<IActionResult> GetLastApproval(string projectId)
        {
            var res = await _projectService.GetLastApprovalData(projectId);
            Message = Success;
            return Ok(res);
        }

        [HttpGet("confirmation")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<IActionResult> GetConfirmationDetail(string projectActionId)
        {
            var res = await _projectService.GetConfirmationDetail(projectActionId);
            Message = Success;
            return Ok(res);
        }

        [HttpPost("confirmation")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<IActionResult> UpdateConfimation([FromBody] ConfirmationRequest confirmationRequest)
        {
            var res = await _projectService.UpdateConfirmationData(confirmationRequest);
            Message = Success;
            return Ok(res);
        }

        [HttpGet("history")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<IActionResult> GetProjectHistory([FromQuery] LogHistoryPagedRequest request)
        {
            List<RoleEnum> listRole = HttpContext.GetUserInfo().Roles.Select(r => r.Enum).ToList();
            var res = await _projectService.GetHistoryPaged(_mapper.Map<PagedDto>(request), _mapper.Map<LogHistoryFilter>(request), listRole);
            Message = Success;
            return Ok(_mapper.Map<Paged<LogHistoryResponse>>(res));
        }

        [HttpGet("meeting")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<IActionResult> GetMeetingList(string projectActionId)
        {
            var userInfo = HttpContext.GetUserInfo();
            var res = await _projectService.GetMeetingList(userInfo, projectActionId);
            Message = Success;
            return Ok(_mapper.Map<List<MeetingResponse>>(res));
        }

        [HttpGet("meetingDetail")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<IActionResult> GetMeetingDetail(string projectActionId, int meetingId)
        {
            var res = await _projectService.GetMeetingDetail(projectActionId, meetingId);
            Message = Success;
            return Ok(_mapper.Map<MeetingDetailResponse>(res));
        }
        
        [HttpPost("addMeeting")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<IActionResult> AddMeeting([FromBody] MeetingRequest request)
        {
            var res = await _projectService.AddMeeting(_mapper.Map<MeetingDetailDto>(request));
            Message = Success;
            return Ok(new {res.ProjectActionId, res.MeetingId});
        }

        [HttpPut("updateMeeting")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<IActionResult> UpdateMeeting([FromBody] MeetingRequest request)
        {
            var res = await _projectService.UpdateMeeting(_mapper.Map<MeetingDetailDto>(request));
            if (res == null) return NotFound("Meeting Not Found");
            Message = Success;
            return Ok(new { res.ProjectActionId, res.MeetingId });
        }

        [HttpDelete("deleteMeeting")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<IActionResult> DeleteMeeting(string projectActionId, int meetingId)
        {
            var res = await _projectService.DeleteMeeting(projectActionId, meetingId);
            if (!res) return BadRequest("Cannot Update Completed Meeting");
            Message = Success;
            return Ok(res);
        }

        [HttpGet("participant")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<IActionResult> GetParticipant(string email)
        {
            var res = await _userService.GetUserSuggestion(email);
            Message = Success;
            return Ok(_mapper.Map<List<MeetingParticipantResponse>>(res));
        }

        [HttpPost("completeAllMeeting")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<IActionResult> CompleteMeeting(string projectActionId)
        {
            var res = await _projectService.CompleteAllMeeting(projectActionId);
            Message = Success;
            return Ok(res);
        }

        [HttpGet("getCommentList")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<IActionResult> GetCommentList(string projectId)
        {
            var res = await _projectService.GetCommentList(projectId);
            Message = Success;
            return Ok(_mapper.Map<Paged<ProjectCommentResponse>>(res));
        }

        [HttpPost("addComment")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<IActionResult> AddComment(string projectId, [FromBody] ProjectCommentRequest request )
        {
            var res = await _projectService.AddComment(projectId, request!.Comment!);
            Message = Success;
            return Ok(new {res.ProjectId, res.ProjectCommentId});
        }

        [HttpDelete("deleteComment")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<IActionResult> DeleteComment(string projectId, int projectCommentId)
        {
            var userInfo = HttpContext.GetUserInfo();
            var res = await _projectService.DeleteComment(projectId, projectCommentId, userInfo);
            if (!res) return NotFound("Comment Not Found");
            Message = Success;
            return Ok(res);
        }

        [HttpGet("templateFollowUp")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<IActionResult> DownloadTemplateFollowUp()
        {
            string workingDirectory = System.IO.Directory.GetCurrentDirectory();
            var filePath = Path.Combine(workingDirectory, StaticFileNameConstant.TemplateFollowUpTemplateFileName);
            FileInfo fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists)
            {
                return NotFound("File Not Found");
            }
            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [HttpPost("uploadFollowUp")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<IActionResult> UploadTemplateFollowUp([FromForm] UploadTemplateFollowUpRequest request)
        {
            var res = await _projectService.UploadTemplateFollowUp(request);
            Message = Success;
            return Ok(res);
        }

        [HttpPost("completeUpload")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<IActionResult> CompleteUpload(string projectActionId)
        {
            var res = await _projectService.CompleteUploadDocument(projectActionId);
            Message = Success;
            return Ok(res);
        }

        [HttpGet("upcomingMeetings")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<IActionResult> GetUpcomingMeetings(string? projectId)
        {
            var res = await _projectService.GetUpcomingMeetings(projectId);
            Message = Success;
            return Ok(res);
        }

        [HttpPost("generateFid")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<IActionResult> GenerateFidcode([FromBody] GenerateFidRequest request)
        {
            var res = await _projectService.GenerateFidcode(
                request.SubholdingCode,
                request.ProjectCategory,
                request.ApprovedYear,
                request.Regional,
                request.ProjectId,
                request.ProjectVersion
            );
            Message = Success;
            return Ok(res);
        }

        [HttpPost("inputFid")]
        [ApiAuthorize(MenuNameConstants.ProjectList)]
        public async Task<IActionResult> InputFidcode([FromBody] GenerateFidRequest request)
        {
            var res = await _projectService.InputFidcode(
                request.Fidcode,
                request.ProjectId,
                request.ProjectVersion
            );
            Message = Success;
            return Ok(res);
        }

        [HttpGet("latestVersion")]
        [ApiAuthorize()]
        public async Task<IActionResult> GetLatestProjectVersion(string projectId)
        {
            var res = await _projectService.GetLatestProjectVersion(projectId);
            Message = Success;
            return Ok(res);
        }
    }
}
