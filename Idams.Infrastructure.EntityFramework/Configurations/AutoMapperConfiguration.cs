using AutoMapper;
using Idams.Core.Constants;
using Idams.Core.Model.Dtos;
using Idams.Core.Model.Entities;
using Idams.Core.Model.Entities.Custom;
using Idams.Core.Model.Filters;
using Idams.Core.Model.Requests;
using Idams.Core.Model.Responses;

namespace Idams.Infrastructure.EntityFramework.Configurations
{
    public class AutoMapperConfiguration : Profile
    {
        public AutoMapperConfiguration()
        {

            CreateMap<WorkflowSettingPaged, WorkflowSettingPagedDto>()
                .AfterMap((ent, dto) =>
                {
                    dto.Status = ent.Status == StatusConstant.Published ? "Active" : "Draft";
                });
            CreateMap<Paged<WorkflowSettingPaged>, Paged<WorkflowSettingPagedDto>>();
            CreateMap<WorkflowSettingPagedDto, WorkflowSettingPagedResponse>();
            CreateMap<Paged<WorkflowSettingPagedDto>, Paged<WorkflowSettingPagedResponse>>();
            CreateMap<RefWorkflowDto, RefWorkflowResponse>();

            CreateMap<WorkflowSettingPagedRequest, PagedDto>();
            CreateMap<WorkflowSettingPagedRequest, WorkflowSettingFilter>();

            CreateMap<ProjectListPagedRequest, PagedDto>();
            CreateMap<ProjectListPagedRequest, ProjectListFilter>();
            CreateMap<TxProjectHeader,ProjectDetailResponse>();
            CreateMap<ProjectInformationRequest, TxProjectHeader>()
                .AfterMap((dto, resp) =>
                {
                    resp.CapexUoM = "USD";
                    resp.DrillingCostUoM = "USD";
                    resp.FacilitiesCostUoM = "USD";
                });

            CreateMap<TxDocument, DocumentDto>();

            CreateMap<GetWorkflowSettingDropdown, GetWorkflowSettingDropdownDto>();
            CreateMap<GetWorkflowSettingDropdownDto, GetWorkflowSettingDropdownResponse>();

            CreateMap<AddWorkflowSettingRequest, RefTemplateDto>()
                .ForMember(n => n.ThresholdName, m => m.MapFrom(n => n.Threshold));

            CreateMap<RefTemplateWorkflowSequenceRequest, RefTemplateWorkflowSequenceDto>();

            CreateMap<RefTemplateDto, RefTemplate>()
                .ForMember(n => n.ThresholdNameParId, m => m.MapFrom(n => n.ThresholdName))
                .ForMember(n => n.ProjectCategoryParId, m => m.MapFrom(n => n.ProjectCategory))
                .ForMember(n => n.ProjectCriteriaParId, m => m.MapFrom(n => n.ProjectCriteria))
                .ForMember(n => n.ProjectSubCriteriaParId, m => m.MapFrom(n => n.ProjectSubCriteria))
                .ReverseMap();

            CreateMap<RefTemplateWorkflowSequenceDto, RefTemplateWorkflowSequence>().ReverseMap();
            CreateMap<RefTemplateWorkflowSequenceDto, RefTemplateWorkflowSequenceResponse>();
            CreateMap<RefTemplateDocumentDto, RefTemplateDocument>().ReverseMap();
            CreateMap<RefTemplateDto, AddWorkflowSettingResponse>();
            CreateMap<RefTemplateDto, GetWorkflowSettingResponse>()
                .ForMember(n => n.Threshold , m => m.MapFrom(n => n.ThresholdName))
                .AfterMap((ent, dto) =>
                {
                    dto.Status = ent.Status == StatusConstant.Published ? "Active" : "Draft";
                });
            CreateMap<RefTemplateWorkflowSequence, TemplateWorkflowSeqResponse>();
            CreateMap<TemplateWorkflowSeqDto, GetTemplateWorkflowSeqResponse>()
                .ForMember(n => n.documentGroup, m => m.MapFrom(s => s.documentGroup));
            CreateMap<WorkflowSeqDocGroupDto, WorkflowSeqDocGroupResponse>();
            CreateMap<DocumentPagedRequest, PagedDto>();
            CreateMap<DocumentPagedRequest, MdDocumentFilter>();
            CreateMap<Paged<MdDocumentPagedDto>, Paged<MdDocumentPagedResponse>>();
            CreateMap<MdDocumentPagedDto, MdDocumentPagedResponse>();
            CreateMap<DocChecklistPagedRequest, PagedDto>();
            CreateMap<DocChecklistPagedRequest, DocumentChecklistFilter>();
            CreateMap< Paged<MpDocumentChecklistPagedDto>, Paged<MpDocumentChecklistPagedResponse>>();
            CreateMap<MpDocumentChecklistPagedDto, MpDocumentChecklistPagedResponse>();

            CreateMap<UpdateScopeOfWorkRequest, UpdateScopeOfWorkDto>();
            CreateMap<ProjectPlatformRequest, ProjectPlatformDto>();
            CreateMap<ProjectPipelineRequest, ProjectPipelineDto>();
            CreateMap<ProjectCompressorRequest, ProjectCompressorDto>();
            CreateMap<ProjectEquipmentRequest, ProjectEquipmentDto>();
            CreateMap<ProjectPlatformDto, TxProjectPlatform>().ReverseMap();
            CreateMap<ProjectCompressorDto, TxProjectCompressor>()
                .ForMember(n => n.CompressorTypeParId, m => m.MapFrom(n => n.CompressorType))
                .ReverseMap();
            CreateMap<ProjectPipelineDto, TxProjectPipeline>()
                .ForMember(n => n.FieldServiceParId, m => m.MapFrom(n => n.FieldService))
                .ReverseMap();
            CreateMap<ProjectEquipmentDto, TxProjectEquipment>().ReverseMap();
            CreateMap<TxProjectHeader, UpdateScopeOfWorkDto>();

            CreateMap<GetScopeOfWorkDropdown, GetScopeOfWorkDropdownDto>();
            CreateMap<GetScopeOfWorkDropdownDto, GetScopeOfWorkDropdownResponse>();
            CreateMap<UpdateResourcesRequest, UpdateResourcesDto>();
            CreateMap<TxProjectHeader, UpdateResourcesDto>();
            CreateMap<MpDocumentChecklistDto, MpDocumentChecklistResponse>(); 
            CreateMap<GetMilestoneDto, GetMilestoneResponse>();
            CreateMap<MilestoneDto, MilestoneResponse>();
            CreateMap<MilestoneRequest, MilestoneDto>();
            CreateMap<UpdateMilestoneRequest, UpdateMilestoneDto>();
            CreateMap<MpDocumentChecklistDto, MpDocumentChecklistResponse>();

            CreateMap<FollowUpRequest, TxFollowUp>();
            CreateMap<GetScopeOfWorkDto, GetScopeOfWorkResponse>();
            CreateMap<ProjectPlatformDto, ProjectPlatformResponse>();
            CreateMap<ProjectPipelineDto, ProjectPipelineResponse>();
            CreateMap<ProjectCompressorDto, ProjectCompressorResponse>();
            CreateMap<ProjectEquipmentDto, ProjectEquipmentResponse>();
            CreateMap<LogHistoryPaged, LogHistoryDto>();
            CreateMap<Paged<LogHistoryPaged>, Paged<LogHistoryDto>>();
            CreateMap<LogHistoryDto, LogHistoryResponse>();
            CreateMap<Paged<LogHistoryDto>, Paged<LogHistoryResponse>>();
            CreateMap<LogHistoryPagedRequest, PagedDto>();
            CreateMap<LogHistoryPagedRequest, LogHistoryFilter>();
            CreateMap<MeetingDto, MeetingResponse>();
            CreateMap<TxMeeting, MeetingDto>()
                .ForMember(n => n.Status, m => m.MapFrom(n => n.MeetingStatusParId));
            CreateMap<MeetingDetailDto, MeetingDetailResponse>();
            CreateMap<MeetingParticipantDto, MeetingParticipantResponse>();
            CreateMap<TxMeetingParticipant, MeetingParticipantDto>().ReverseMap();
            CreateMap<TxMeeting, MeetingDetailDto>();
            CreateMap<MeetingRequest, MeetingDetailDto>();
            CreateMap<MeetingParticipantRequest, MeetingParticipantDto>();
            CreateMap<MeetingDetailDto, TxMeeting>()
                .ForMember(n => n.MeetingStatusParId, m => m.MapFrom(n => n.Status))
                .ReverseMap();
            CreateMap<TxProjectComment, ProjectCommentDto>();
            CreateMap<ProjectCommentDto, ProjectCommentResponse>();
            CreateMap<Paged<ProjectCommentDto>, Paged<ProjectCommentResponse>>();
            CreateMap<GetDocumentDropdownDto, GetDocumentDropdownResponse>();
            CreateMap<DocumentManagementPagedRequest, PagedDto>();
            CreateMap<DocumentManagementPagedRequest, TxDocumentFilter>();
            CreateMap<DocumentManagementPaged, DocumentManagementDto>();
            CreateMap<Paged<DocumentManagementPaged>, Paged<DocumentManagementDto>>();
            CreateMap<DocumentManagementDto, DocumentManagementResponse>();
            CreateMap<Paged<DocumentManagementDto>, Paged<DocumentManagementResponse>>();
            CreateMap<CalendarEventDropdownDto, CalendarEventDropdownResponse>();
            CreateMap<MeetingWithProjectAndStageDto, MeetingWithProjectAndStageResponse>();
            CreateMap<DocumentManagementPreviewDto, DocumentManagementPreviewResponse>();

            CreateMap<OutstandingTaskListPagedRequest, PagedDto>();
            CreateMap<OutstandingTaskListPagedRequest, OutstandingTaskListFilter>();
            CreateMap<EmployeeDto, MeetingParticipantResponse>();
        }
    }
}
