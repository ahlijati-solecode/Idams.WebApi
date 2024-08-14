using Idams.Core.Enums;
using Idams.Core.Model.Dtos;
using Idams.Core.Model.Entities;
using Idams.Core.Model.Entities.Custom;
using Idams.Core.Model.Filters;
using Idams.Core.Model.Requests;
using Idams.Core.Model.Responses;

namespace Idams.Core.Services
{
    public interface IProjectService
    {
        Task<Paged<ProjectListPaged>> GetPaged(PagedDto pagedDto, ProjectListFilter filter, UserDto user);
        Task<IEnumerable<ProjectBannerCount>> GetBannerCount(UserDto user);
        Task<int> DeleteProject(string projectId, int projectVersion);
        Task<int> DeleteProject(List<ProjectHeaderIdentifier> project);
        Task<ProjectListDropdown> GetDropdownList();
        Task<string> GetProjectListTableConfig(string userId);
        Task<int> SaveProjectListTableConfig(string userId, string config);
        Task<Dictionary<string, string>> GetAvailableHierLvl2(UserDto userInfo);
        Task<Dictionary<string, string>> GetAvailableHierLvl2(UserDto userInfo, List<string> hierlvl3s);
        Task<Dictionary<string, string>> GetAvailableHierLvl3(UserDto userInfo, string hierlvl2);
        Task<Dictionary<string, string>> GetAvailableHierLvl3(UserDto userinfo, List<string> hierlvl2s);
        Task<Dictionary<string, string>> GetAvailableHierLvl4(string hierlvl3);
        Task<RefTemplate?> DetermineUsedTemplate(ProjectTemplateFilter projectTemplateFilter);
        Task<TxProjectHeader> AddNewProject(TxProjectHeader tph, List<string> entityIds, string section, bool? saveLog);
        Task<TxProjectHeader> UpdateProjectInformation(UpdateProjectInformationRequest projectInformationRequest);
        Task<UpdateScopeOfWorkDto?> UpdateScopeOfWork(UpdateScopeOfWorkDto scopeOfWorkDto);
        Task<TxProjectHeader?> UpdateEconomicIndicator(UpdateEconomicIndicatorRequest request);
        Task<GetScopeOfWorkDropdownDto> GetDropdownAsync();
        Task<ProjectDetailResponse> GetProjectDetail(string projectId, int projectVersion);
        Task<TxProjectHeader> PopulateMilestone(string projectId, int projectVersion);
        Task<ProjectDocumentGroupResponse> GetDocumentGroupOfProject(string projectActionId);
        Task<UpdateResourcesDto?> UpdateResources(UpdateResourcesDto resourcesDto);
        Task<TxProjectHeader?> UpdateInitiationDocuments(UpdateInitiationDocsRequest request);
        Task<ProjectSequenceTimelineResponse?> GetProjectSequence(string projectId, int projectVersion);
        Task<ProjectSequenceActionsResponse> GetProjectSequenceAction(string projectId, string workflowSequenceId);
        Task<GetMilestoneDto?> GetMilestone(string projectId, int projectVersion);
        Task<UpdateMilestoneDto?> UpdateMilestone(UpdateMilestoneDto milestoneDto);
        Task<GetScopeOfWorkDto?> GetScopeOfWork(string projectId, int projectVersion);
        Task<bool?> InitiateProject(string projectId, int projectVersion);
        Task<Paged<LogHistoryDto>> GetHistoryPaged(PagedDto paged, LogHistoryFilter filter, List<RoleEnum> roles);
        Task<UpdateDataStatusResponse> GetUpdateDataStatus(string projectActionId);
        Task<UpdateDataStatusResponse> CompleteWorkflowUpdatedata(string projectActionId, bool complete);
        Task<FollowUpDropDownDto> GetFollowUpDropdownList();
        Task<List<FollowUpDetailResponse>> GetFollowUpList(string projectActionId);
        Task<List<TxFollowUp>> AddFollowUpList(List<FollowUpRequest> followUpList);
        Task<TxFollowUp> UpdateFollowUp(FollowUpRequest followUp);
        Task<bool> DeleteFollowUp(int followUpId, string projectActionId);
        Task<ApprovalDetailDto> GetApprovalDetail(string projectActionId);
        Task<TxApproval?> GetLastApprovalData(string projectId);
        Task<TxApproval> UpdateApprovalData(ApprovalRequest approvalRequst);
        Task<ConfirmationDetailDto> GetConfirmationDetail(string projectActionId);
        Task<ConfirmationDetailDto> UpdateConfirmationData(ConfirmationRequest confirmation);
        Task<List<MeetingDto>> GetMeetingList(UserDto user, string projectActionId);
        Task<MeetingDetailDto> GetMeetingDetail(string projectActionId, int meetingId);
        Task<MeetingDetailDto> AddMeeting(MeetingDetailDto meeting);
        Task<MeetingDetailDto?> UpdateMeeting(MeetingDetailDto meeting);
        Task<bool> DeleteMeeting(string projectActionId, int meetingId);
        Task<bool> CompleteAllMeeting(string projectActionId);
        Task<Paged<ProjectCommentDto>> GetCommentList(string projectId);
        Task<ProjectCommentDto> AddComment(string projectId, string comment);
        Task<bool> DeleteComment(string projectId, int projectCommentId, UserDto user);
        Task<FollowUpDtoWithSum> UploadTemplateFollowUp(UploadTemplateFollowUpRequest request);
        Task<bool> CompleteUploadDocument(string projectActionId);
        Task<List<UpcomingMeetingDto>> GetUpcomingMeetings(string? projectId);
        Task<CalendarEventDropdownDto> GetCalendarEventDropdown();
        Task<List<MeetingWithProjectAndStageDto>> GetCalendarEvent(DateTime startDate, DateTime endDate, string? projectName, string? threshold);
        Task<string> GenerateFidcode(string subholdingCode, string projectCategory, int approvedYear, string regional, string projectId, int projectVersion);
        Task<string> InputFidcode(string fidcode, string projectId, int projectVersion);
        Task<Dictionary<string, string>> GetAvailableWorkflowType(List<string> stage);
        Task<Dictionary<string, string>> GetAvaiableStage(List<string> workflowType);
        Task<int?> GetLatestProjectVersion(string projectVersion);
    }
}
