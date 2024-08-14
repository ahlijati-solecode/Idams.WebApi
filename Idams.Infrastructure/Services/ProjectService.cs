using AutoMapper;
using ClosedXML.Excel;
using Idams.Core.Constants;
using Idams.Core.Enums;
using Idams.Core.Extenstions;
using Idams.Core.Model.Dtos;
using Idams.Core.Model.Entities;
using Idams.Core.Model.Entities.Custom;
using Idams.Core.Model.Filters;
using Idams.Core.Model.Requests;
using Idams.Core.Model.Responses;
using Idams.Core.Repositories;
using Idams.Core.Services;
using Idams.Infrastructure.Utils;
using shortid;
using shortid.Configuration;
using System.Data;

namespace Idams.Infrastructure.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IParameterListRepository _parameterRepository;
        private readonly IRefWorkflowRepository _refWorkflowRepository;
        private readonly IHierLvlVwRepository _hierLvlVwRepository;
        private readonly ITableColumnSettingRepository _tableSettingRepository;
        private readonly IWorkflowSettingRepository _workflowSettingRepository;
        private readonly IRefTemplateWorkflowSequenceRepository _workflowSequenceRepository;
        private readonly IProjectActionRepository _projectActionRepository;
        private readonly IProjectAuditTrailRepository _projectAuditTrailRepository;
        private readonly IProjectUpstreamEntityRepository _projectUpstreamEntityRepository;
        private readonly IProjectPlatformRepository _projectPlatformRepository;
        private readonly IProjectPipelineRepository _projectPipelineRepository;
        private readonly IProjectCompressorRepository _projectCompressorRepository;
        private readonly IProjectEquipmentRepository _projectEquipmentRepository;
        private readonly IUnitOfWorks _unitOfWorks;
        private readonly IMapper _mapper;
        private readonly IDocumentRepository _documentRepository;
        private readonly IProjectMilestoneRepository _projectMilestoneRepository;
        private readonly ILgProjectActivityAuditTrailRepository _lgProjectActivityAuditTrailRepository;
        private readonly IApprovalRepository _approvalRepository;
        private readonly IFollowUpRepository _followUpRepository;
        private readonly ITxMeetingRepositories _txMeetingRepositories;
        private readonly ITxMeetingParticipantRepository _txMeetingParticipantRepository;
        private readonly IProjectCommentRepository _projectCommentRepository;
        private readonly IUpcomingMeetingRepository _upcomingMeetingRepository;
        private readonly IFidcodeRepository _fidcodeRepository;
        private readonly IEmailService _emailService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IWorkflowIntegrationService _workflowIntegrationService;
        private readonly IUserService _userService;

        public ProjectService(IProjectRepository projectRepository,
            IParameterListRepository parameterListRepository,
            IRefWorkflowRepository refWorkflowRepository,
            IHierLvlVwRepository hierLvlVwRepository,
            ITableColumnSettingRepository tableSettingRepository,
            IWorkflowSettingRepository workflowSettingRepository,
            IRefTemplateWorkflowSequenceRepository workflowSequenceRepository,
            IProjectActionRepository projectActionRepository,
            IUnitOfWorks unitOfWorks,
            IProjectAuditTrailRepository projectAuditTrailRepository,
            IProjectUpstreamEntityRepository projectUpstreamEntityRepository,
            IProjectPlatformRepository platformRepository,
            IProjectPipelineRepository pipelineRepository,
            IProjectCompressorRepository compressorRepository,
            IProjectEquipmentRepository equipmentRepository,
            IMapper mapper,
            IDocumentRepository documentRepository,
            IProjectMilestoneRepository projectMilestoneRepository,
            ILgProjectActivityAuditTrailRepository lgProjectActivityAuditTrailRepository,
            IApprovalRepository approvalRepository,
            IFollowUpRepository followUpRepository,
            ITxMeetingRepositories txMeetingRepositories,
            ITxMeetingParticipantRepository txMeetingParticipantRepository,
            IProjectCommentRepository projectCommentRepository,
            IUpcomingMeetingRepository upcomingMeetingRepository,
            IFidcodeRepository fidcodeRepository,
            IEmailService emailService,
            ICurrentUserService currentUserService,
            IWorkflowIntegrationService workflowIntegrationService,
            IUserService userService)
        {
            _projectRepository = projectRepository;
            _parameterRepository = parameterListRepository;
            _refWorkflowRepository = refWorkflowRepository;
            _hierLvlVwRepository = hierLvlVwRepository;
            _tableSettingRepository = tableSettingRepository;
            _workflowSettingRepository = workflowSettingRepository;
            _workflowSequenceRepository = workflowSequenceRepository;
            _projectActionRepository = projectActionRepository;
            _unitOfWorks = unitOfWorks;
            _projectAuditTrailRepository = projectAuditTrailRepository;
            _projectUpstreamEntityRepository = projectUpstreamEntityRepository;
            _projectPlatformRepository = platformRepository;
            _projectPipelineRepository = pipelineRepository;
            _projectCompressorRepository = compressorRepository;
            _projectEquipmentRepository = equipmentRepository;
            _mapper = mapper;
            _documentRepository = documentRepository;
            _projectMilestoneRepository = projectMilestoneRepository;
            _lgProjectActivityAuditTrailRepository = lgProjectActivityAuditTrailRepository;
            _approvalRepository = approvalRepository;
            _followUpRepository = followUpRepository;
            _txMeetingRepositories = txMeetingRepositories;
            _txMeetingParticipantRepository = txMeetingParticipantRepository;
            _projectCommentRepository = projectCommentRepository;
            _upcomingMeetingRepository = upcomingMeetingRepository;
            _fidcodeRepository = fidcodeRepository;
            _emailService = emailService;
            _currentUserService = currentUserService;
            _workflowIntegrationService = workflowIntegrationService;
            _userService = userService;
        }

        private async Task GenerateDraftLog(string projectId, string? workflowSeqId, string? workflowActionId)
        {
            await GenerateLogActivityProject(projectId, workflowSeqId, ActionConstant.SaveDraft, StatusConstant.Draft, "Save Project As Draft for Project Initiation", workflowActionId);
        }

        private async Task GenerateLogActivityProject(string projectId, string? workflowSeqId, string? action , string? activityStatus, string? activityDescription, string? workflowActionId)
        {
            LgProjectActivityAuditTrail logActivity = new LgProjectActivityAuditTrail
            {
                ProjectId = projectId,
                WorkflowSequenceId = workflowSeqId,
                Action = action,
                ActivityStatusParId = activityStatus,
                ActivityDescription = activityDescription,
                WorkflowActionId = workflowActionId
            };
            await _lgProjectActivityAuditTrailRepository.AddLog(logActivity);
        }

        public async Task<Paged<ProjectListPaged>> GetPaged(PagedDto pagedDto, ProjectListFilter filter, UserDto user)
        {
            UserUtil.DetermineUserPrivelege(user, out string? hierlvl2, out string? hierlvl3);
            filter.HierLvl2 = hierlvl2;
            filter.HierLvl3 = hierlvl3;
            return await _projectRepository.GetPaged(filter, pagedDto.Page, pagedDto.Size, pagedDto.Sort);
        }

        private static void DetermineUserPrivelegeToAccessProject(UserDto user, out string? hierlvl2, out string? hierlvl3)
        {
            bool allData = false;
            bool lvl2 = false;
            hierlvl2 = null;
            hierlvl3 = null;
            foreach (var role in user.Roles)
            {
                switch (role.Enum)
                {
                    case RoleEnum.SUPER_ADMIN:
                    case RoleEnum.ADMIN_SHU:
                    case RoleEnum.PLANNING_SHU:
                    case RoleEnum.PIC_SHU:
                    case RoleEnum.REVIEWER_SHU:
                        allData = true;
                        break;
                    case RoleEnum.ADMIN_REGIONAL:
                    case RoleEnum.PLANNING_REGIONAL:
                    case RoleEnum.PIC_REGIONAL:
                    case RoleEnum.REVIEWER_REGIONAL:
                        lvl2 = true;
                        break;
                }
            }

            if (allData)
                return;
            hierlvl2 = string.IsNullOrWhiteSpace(user.HierLvl2.Value) ? null : user.HierLvl2?.Key;
            if (lvl2)
                return;
            hierlvl3 = string.IsNullOrWhiteSpace(user.HierLvl3.Value) ? null : user.HierLvl3?.Key;
        }

        public async Task<IEnumerable<ProjectBannerCount>> GetBannerCount(UserDto user)
        {
            UserUtil.DetermineUserPrivelege(user, out string? hierlvl2, out string? hierlvl3);
            return await _projectRepository.GetBannerCount(hierlvl2, hierlvl3);
        }

        public async Task<int> DeleteProject(string projectId, int projectVersion)
        {
            var projectHeader = await _projectRepository.GetProjectHeader(projectId, projectVersion);
            var additionalData = await _projectRepository.GetThresholdRegionalZonaByProjectId(projectId);
            List<SendMailRequest> sendMailReqList = new List<SendMailRequest>();
            SendMailRequest sendMailRequest = new SendMailRequest()
            {
                EmailCode = "IDAMS.ProjectDeleted",
                ActionBy = _currentUserService.CurrentUserInfo.Email,
                To = _currentUserService.CurrentUserInfo.Email,
                ListJson = new List<BaseKeyValueObject> {
                        new BaseKeyValueObject {Key = "User", Value = _currentUserService.CurrentUser.FullName},
                        new BaseKeyValueObject {Key = "Project_Name", Value = projectHeader!.ProjectName},
                        new BaseKeyValueObject {Key = "Project_Category", Value=_parameterRepository.GetParam("idams", "ProjectCategory", projectHeader.Template!.ProjectCategoryParId).Result!.ParamValue1Text},
                        new BaseKeyValueObject {Key = "Project_Criteria", Value = _parameterRepository.GetParam("idams", "ProjectCriteria", projectHeader.Template!.ProjectCriteriaParId).Result!.ParamValue1Text},
                        new BaseKeyValueObject {Key = "Project_SubCriteria", Value = projectHeader.Template.ProjectSubCriteriaParId == null ? "-" :  _parameterRepository.GetParam("idams", "SubProjectCriteria", projectHeader.Template!.ProjectSubCriteriaParId).Result!.ParamValue1Text ?? ""},
                        new BaseKeyValueObject {Key = "Project_Threshold", Value = _parameterRepository.GetParam("idams", "ThresholdName", projectHeader.Template!.ThresholdNameParId).Result!.ParamValue1Text},
                        new BaseKeyValueObject {Key = "Project_Regional", Value = additionalData!.HierLvl2Desc},
                        new BaseKeyValueObject {Key = "Project_Zona", Value = additionalData!.HierLvl3Desc}
                }
            };
            sendMailReqList.Add(sendMailRequest);
            await _emailService.SendMailAsync(sendMailReqList);
            return await _projectRepository.DeleteProject(projectId, projectVersion);
        }

        public async Task<int> DeleteProject(List<ProjectHeaderIdentifier> projects)
        {
            foreach(var project in projects)
            {
                var projectHeader = await _projectRepository.GetProjectHeader(project.ProjectId, project.ProjectVersion);
                var additionalData = await _projectRepository.GetThresholdRegionalZonaByProjectId(project.ProjectId);
                List<SendMailRequest> sendMailReqList = new List<SendMailRequest>();
                SendMailRequest sendMailRequest = new SendMailRequest()
                {
                    EmailCode = "IDAMS.ProjectDeleted",
                    ActionBy = _currentUserService.CurrentUserInfo.Email,
                    To = _currentUserService.CurrentUserInfo.Email,
                    ListJson = new List<BaseKeyValueObject> {
                        new BaseKeyValueObject {Key = "User", Value = _currentUserService.CurrentUserInfo.Name},
                        new BaseKeyValueObject {Key = "Project_Name", Value = projectHeader!.ProjectName},
                        new BaseKeyValueObject {Key = "Project_Category", Value=_parameterRepository.GetParam("idams", "ProjectCategory", projectHeader.Template!.ProjectCategoryParId).Result!.ParamValue1Text},
                        new BaseKeyValueObject {Key = "Project_Criteria", Value = _parameterRepository.GetParam("idams", "ProjectCriteria", projectHeader.Template!.ProjectCriteriaParId).Result!.ParamValue1Text},
                        new BaseKeyValueObject {Key = "Project_SubCriteria", Value = projectHeader.Template.ProjectSubCriteriaParId == null ? "-" :  _parameterRepository.GetParam("idams", "SubProjectCriteria", projectHeader.Template!.ProjectSubCriteriaParId).Result!.ParamValue1Text ?? ""},
                        new BaseKeyValueObject {Key = "Project_Threshold", Value = _parameterRepository.GetParam("idams", "ThresholdName", projectHeader.Template!.ThresholdNameParId).Result!.ParamValue1Text},
                        new BaseKeyValueObject {Key = "Project_Regional", Value = additionalData!.HierLvl2Desc},
                        new BaseKeyValueObject {Key = "Project_Zona", Value = additionalData!.HierLvl3Desc}
                    }
                };
                sendMailReqList.Add(sendMailRequest);
                await _emailService.SendMailAsync(sendMailReqList);
            }
            return await _projectRepository.DeleteProject(projects);
        }

        public async Task<ProjectListDropdown> GetDropdownList()
        {
            var threshold = await _parameterRepository.GetParams("idams", "ThresholdName");
            var stage = await _parameterRepository.GetParams("idams", "Stage");
            var wf = await _refWorkflowRepository.GetList();
            var hierlvl2 = await _hierLvlVwRepository.GetDistinctHierLvl2List();
            var hierlvl3 = await _hierLvlVwRepository.GetDistinctHierLvl3List();

            ProjectListDropdown ret = new ProjectListDropdown();
            ret.Threshold = threshold.ToDictionary(keySelector: t => t.ParamListId, elementSelector: t => t.ParamValue1Text!);
            ret.Stage = stage.ToDictionary(keySelector: t => t.ParamListId, elementSelector: t => t.ParamValue1Text!);
            ret.Workflow = wf.ToDictionary(keySelector: t => t.key, elementSelector: t => t.value!);
            ret.HierLvl2 = hierlvl2.ToDictionary(keySelector: t => t.Key, elementSelector: t => t.Value);
            ret.HierLvl3 = hierlvl3.ToDictionary(keySelector: t => t.Key, elementSelector: t => t.Value);
            return ret;
        }

        public async Task<string> GetProjectListTableConfig(string userId)
        {
            return await _tableSettingRepository.GetProjectListTableConfig(userId);
        }

        public async Task<int> SaveProjectListTableConfig(string userId, string config)
        {
            return await _tableSettingRepository.SaveProjectListTableConfig(userId, config);
        }

        public async Task<Dictionary<string, string>> GetAvailableHierLvl2(UserDto userInfo)
        {
            UserUtil.DetermineUserPrivelege(userInfo, out string? hierlvl2, out string? hierlvl3);

            if(hierlvl2 != null)
            {
                return new Dictionary<string, string>()
                {
                    {userInfo.HierLvl2.Key, userInfo.HierLvl2.Value }
                };
            }

            var hier = await _hierLvlVwRepository.GetDistinctHierLvl2List();
            return hier.ToDictionary(keySelector: t => t.Key, elementSelector: t => t.Value);
        }

        public async Task<Dictionary<string, string>> GetAvailableHierLvl2(UserDto userInfo, List<string> hierlvl3s)
        {
            UserUtil.DetermineUserPrivelege(userInfo, out string? hierlvl2, out string? hierlvl3);

            if(hierlvl2 != null)
            {
                return new Dictionary<string, string>()
                {
                    {userInfo.HierLvl2.Key, userInfo.HierLvl2.Value }
                };
            }
            var hier = await _hierLvlVwRepository.GetDistinctHierLvl2List(hierlvl3s);
            return hier.ToDictionary(keySelector: t => t.Key, elementSelector: t => t.Value);
        }

        public async Task<Dictionary<string,string>> GetAvailableHierLvl3(UserDto userInfo, string hierlvl2)
        {
            UserUtil.DetermineUserPrivelege(userInfo, out string? _, out string? hierlvl3);

            if(hierlvl3!= null)
            {
                return new Dictionary<string, string>()
                {
                    {userInfo.HierLvl3.Key, userInfo.HierLvl3.Value }
                };
            }

            var hier = await _hierLvlVwRepository.GetDistinctHierLvl3List(hierlvl2);
            return hier.ToDictionary(keySelector: t => t.Key, elementSelector: t => t.Value);
        }

        public async Task<Dictionary<string,string>> GetAvailableHierLvl3(UserDto userinfo, List<string> hierlvl2s)
        {
            UserUtil.DetermineUserPrivelege(userinfo, out string? _, out string? hierlvl3);
            if(hierlvl3 != null)
            {
                return new Dictionary<string, string>
                {
                    {userinfo.HierLvl3.Key, userinfo.HierLvl3.Value }
                };
            }
            HashSet<HierLvlDto> hierlvl3s = new HashSet<HierLvlDto>();
            foreach(var hierlvl2 in hierlvl2s)
            {
                var hier = await _hierLvlVwRepository.GetDistinctHierLvl3List(hierlvl2);

                foreach(var lvl3 in hier)
                {
                    hierlvl3s.Add(lvl3);
                }
            }
            return hierlvl3s.ToDictionary(keySelector: t => t.Key, elementSelector: t => t.Value);
        }

        public async Task<Dictionary<string,string>> GetAvailableHierLvl4(string hierlvl3)
        {
            var hier = await _hierLvlVwRepository.GetHierLvl4List(hierlvl3);
            return hier.ToDictionary(keySelector: t => t.Key, elementSelector: t => t.Value);
        }

        public async Task<RefTemplate?> DetermineUsedTemplate(ProjectTemplateFilter projectTemplateFilter)
        {
            return await _workflowSettingRepository.DetermineTemplateByMultipleCategory(projectTemplateFilter);
        }


        private async Task<TxProjectHeader> GenerateNewProjectVersion(string projectId , int projectVersion)
        {
            
            var project = await _projectRepository.GetProjectHeader(projectId, projectVersion);
            var newProject = new TxProjectHeader();
            project!.CopyProperties(newProject);
            newProject.Template = null;
            newProject.ProjectVersion = project!.ProjectVersion + 1;
            newProject.LastUpdateWorkflowSequence = project.CurrentWorkflowSequence;
            await _projectRepository.AddProject(newProject);
            await _unitOfWorks.SaveChangesAsync();


            //set inactive old project 
            project = await _projectRepository.GetProjectHeader(projectId, projectVersion);
            project!.IsActive = false;
            _projectRepository.Update(project);
            await _unitOfWorks.SaveChangesAsync();
            
            //add pipeline
            var pipelines = await _projectPipelineRepository.GetPipelineAsync(projectId, projectVersion);
            if (pipelines.Count > 0)
            {
                foreach (var pipeline in pipelines)
                {
                    pipeline.ProjectPipelineId = ShortId.Generate(new GenerationOptions(true, false, 20));
                    pipeline.ProjectVersion++;
                }
                await _projectPipelineRepository.AddPipelineAsync(pipelines);
            }
            //add platform
            var platforms = await _projectPlatformRepository.GetPlatformsAsync(projectId, projectVersion);
            if (platforms.Count > 0)
            {
                foreach (var platform in platforms)
                {
                    platform.ProjectPlateformId = ShortId.Generate(new GenerationOptions(true, false, 20));
                    platform.ProjectVersion = newProject.ProjectVersion;
                }
                await _projectPlatformRepository.AddPlatformAsync(platforms);

            }
            //add compressor
            var compressors = await _projectCompressorRepository.GetCompressorsAsync(projectId, projectVersion);
            if (compressors.Count > 0)
            {
                foreach (var compressor in compressors)
                {
                    compressor.ProjectCompressorId = ShortId.Generate(new GenerationOptions(true, false, 20));
                    compressor.ProjectVersion = newProject.ProjectVersion;
                }
                await _projectCompressorRepository.AddCompressorAsync(compressors);
            }

            //add equipment
            var equipments = await _projectEquipmentRepository.GetEquipmentsAsync(projectId, projectVersion);
            if (equipments.Count > 0)
            {
                foreach (var equipment in equipments)
                {
                    equipment.ProjectEquipmentId = ShortId.Generate(new GenerationOptions(true, false, 20));
                    equipment.ProjectVersion = newProject.ProjectVersion;
                }
                await _projectEquipmentRepository.AddEquipmentAsync(equipments);
            }

            //add milestone
            List<TxProjectMilestone> savedMilestone = new List<TxProjectMilestone>();
            var milestones = await _projectMilestoneRepository.GetMilestonesAsync(projectId, projectVersion);
            if (milestones.Count > 0)
            {
                foreach (var milestone in milestones)
                {
                    TxProjectMilestone txProjectMilestone = new TxProjectMilestone()
                    {
                        ProjectId = milestone.ProjectId,
                        ProjectVersion = milestone.ProjectVersion + 1,
                        WorkflowSequenceId = milestone.WorkflowSequenceId,
                        StartDate = milestone.StartDate,
                        EndDate = milestone.EndDate,
                    };
                    savedMilestone.Add(txProjectMilestone);
                }
                await _projectMilestoneRepository.AddMilestoneAsync(savedMilestone);
            }
            return newProject;
            
        }

        public async Task<TxProjectHeader> AddNewProject(TxProjectHeader tph, List<string> entityIds, string section, bool? saveLog)
        {
            using var transaction = _unitOfWorks.BeginTransaction();
            try
            {
                var template = await ValidateTemplateToBeUsedByProject(tph.TemplateId, tph.TemplateVersion);

                var workflowSequence = await _workflowSequenceRepository.GetListTemplateWorkflowSequence(template.TemplateId, template.TemplateVersion);

                var firstWorkflowSequence = workflowSequence[0];
                tph.Deleted = false;
                tph.Status = "Draft";
                tph.IsActive = true;
                tph = await _projectRepository.AddAsync(tph);
                tph.CurrentWorkflowSequence = firstWorkflowSequence.WorkflowSequenceId;
                tph.LastUpdateWorkflowSequence = firstWorkflowSequence.WorkflowSequenceId;


                ////do transaction -- disabled not used
                //var firstWorkflow = await _refWorkflowRepository.GetByWorkflowSequenceId(workflowSequence[0].WorkflowSequenceId);
                //PHEMadamAPIDto? resDoTransaction = await _workflowIntegrationService.DoTransaction("Save", "1", firstWorkflow!.WorkflowMadamPk!, _currentUserService.CurrentUserInfo.EmpAccount, _currentUserService.CurrentUserInfo.EmpAccount);

                var actions = await _refWorkflowRepository.GetListWorkflowAction(firstWorkflowSequence.WorkflowId!);
                TxProjectAction projectAction = new TxProjectAction();
                projectAction.ProjectId = tph.ProjectId;
                projectAction.Status = StatusConstant.Inprogress;
                projectAction.WorkflowSequenceId = firstWorkflowSequence.WorkflowSequenceId;
                projectAction.WorkflowActionId = actions[0].WorkflowActionId;
                // projectAction.AimanTransactionNumber = resDoTransaction?.Object?.Value!;
                projectAction = await _projectActionRepository.AddAsync(projectAction);

                tph.CurrentAction = projectAction.ProjectActionId;
                tph.InitiationAction = projectAction.ProjectActionId;

                if (saveLog.GetValueOrDefault())
                {
                    await GenerateDraftLog(tph.ProjectId, firstWorkflowSequence.WorkflowSequenceId, actions[0]?.WorkflowActionId);
                }

                await _projectAuditTrailRepository.SaveAuditTrailCreation(tph.ProjectId, tph.ProjectVersion, section);
                await _projectAuditTrailRepository.SaveAuditTrailUpdateProjectInformation(tph.ProjectId, tph.ProjectVersion);
                await _projectUpstreamEntityRepository.SaveProjectUpstreamEntity(tph.ProjectId, entityIds);
                await _unitOfWorks.SaveChangesAsync();
                await transaction.CommitAsync();
                return tph;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private async Task<RefTemplate> ValidateTemplateToBeUsedByProject(string? templateId, int? templateVersion)
        {
            var template = await _workflowSettingRepository.GetByTemplateIdAndTemplateVersion(templateId, templateVersion);

            if (template == null || template.Deleted == true)
                throw new InvalidDataException("Specified template not found");
            if (template.Status == StatusConstant.Draft)
                throw new InvalidOperationException("Specified template is not an active tempalte");

            return template;
        }

        public async Task<TxProjectHeader> PopulateMilestone(string projectId, int projectVersion)
        {
            using var transaction = _unitOfWorks.BeginTransaction();
            try
            {
                var project = await _projectRepository.GetProjectHeader(projectId, projectVersion);
                if (project == null)
                {
                    throw new InvalidOperationException("Specified Project not found");
                }

                if(await _projectMilestoneRepository.MilestoneExist(projectId, projectVersion))
                {
                    throw new InvalidOperationException("template used by this project already locked. Milestone populated.");
                }

                var workflowSequence = await _workflowSequenceRepository.GetListTemplateWorkflowSequence(project.TemplateId, project.TemplateVersion.Value);

                List<TxProjectMilestone> milestones = new List<TxProjectMilestone>();
                foreach (var sequence in workflowSequence)
                {
                    TxProjectMilestone milestone = new TxProjectMilestone()
                    {
                        ProjectId = project.ProjectId,
                        ProjectVersion = project.ProjectVersion,
                        WorkflowSequenceId = sequence.WorkflowSequenceId
                    };
                    milestones.Add(milestone);
                }
                await _projectMilestoneRepository.AddMilestoneAsync(milestones);
                await _unitOfWorks.SaveChangesAsync();
                await transaction.CommitAsync();
                return project;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<TxProjectHeader> UpdateProjectInformation(UpdateProjectInformationRequest projectInformationRequest)
        {
            using var transaction = _unitOfWorks.BeginTransaction();
            try
            {
                var project = await _projectRepository.GetProjectHeader(projectInformationRequest.ProjectId, projectInformationRequest.ProjectVersion);
                if (project == null)
                {
                    throw new InvalidOperationException("Specified Project not found");
                }
                if(project.IsActive == false)
                {
                    throw new InvalidOperationException("cannot update inactive template");
                }

                //validate template change
                bool changeTemplate = false;
                if (!string.IsNullOrEmpty(projectInformationRequest.TemplateId) && projectInformationRequest.TemplateVersion != null &&
                    (project.TemplateId != projectInformationRequest.TemplateId || project.TemplateVersion != projectInformationRequest.TemplateVersion))
                {
                    if (await _projectMilestoneRepository.MilestoneExist(projectInformationRequest.ProjectId, projectInformationRequest.ProjectVersion))
                    {
                        throw new InvalidOperationException($"Project template already locked. Cannot change project template to {projectInformationRequest.TemplateId}:{projectInformationRequest.TemplateVersion}");
                    }
                    changeTemplate = true;
                    var template = await ValidateTemplateToBeUsedByProject(projectInformationRequest.TemplateId, projectInformationRequest.TemplateVersion);
                    var projectAction = await _projectActionRepository.GetAction(project.CurrentAction);
                    var workflowSequence = await _workflowSequenceRepository.GetListTemplateWorkflowSequence(template.TemplateId, template.TemplateVersion);
                    var firstWorkflowSequence = workflowSequence[0];
                    project.CurrentWorkflowSequence = firstWorkflowSequence.WorkflowSequenceId;
                    var actions = await _refWorkflowRepository.GetListWorkflowAction(firstWorkflowSequence.WorkflowId!);
                    projectAction!.WorkflowSequenceId = firstWorkflowSequence.WorkflowSequenceId;
                    projectAction.WorkflowActionId = actions[0].WorkflowActionId;
                    await _projectActionRepository.UpdateAsync(projectAction);
                }

                if (!changeTemplate && project.IsActive == true && project.CurrentWorkflowSequence != project.LastUpdateWorkflowSequence)
                {
                    project = await GenerateNewProjectVersion(project.ProjectId, project.ProjectVersion);
                    projectInformationRequest.ProjectVersion = project.ProjectVersion;
                    projectInformationRequest.CopyProperties(project);
                    _projectRepository.Update(project);
                }
                else
                {
                    projectInformationRequest.CopyProperties(project);
                    _projectRepository.Update(project);
                }

                var action = await _projectActionRepository.GetAction(project.CurrentAction);
                var workfowSeq = await _workflowSequenceRepository.GetWorkflowSequence(action!.WorkflowSequenceId!);

                if (projectInformationRequest.SaveLog.GetValueOrDefault())
                {
                    await GenerateDraftLog(project.ProjectId, project.CurrentWorkflowSequence, action?.WorkflowActionId);
                }
                
                await _projectAuditTrailRepository.SaveAuditTrailCreation(project.ProjectId, project.ProjectVersion, projectInformationRequest.Section);
                await _projectAuditTrailRepository.SaveAuditTrailUpdateProjectInformation(project.ProjectId, project.ProjectVersion);
                await _projectUpstreamEntityRepository.SaveProjectUpstreamEntity(project.ProjectId, projectInformationRequest.EntityIds);
                await _unitOfWorks.SaveChangesAsync();
                await transaction.CommitAsync();
                return project;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<UpdateScopeOfWorkDto?> UpdateScopeOfWork(UpdateScopeOfWorkDto scopeOfWorkDto)
        {
            var project = await _projectRepository.GetProjectHeader(scopeOfWorkDto.ProjectId, scopeOfWorkDto.ProjectVersion);
            if(project == null)
            {
                return null;
            }
            using var transaction = _unitOfWorks.BeginTransaction();
            try
            {
                if (project.IsActive == false)
                {
                    throw new InvalidOperationException("cannot update inactive template");
                }

                if (project.IsActive == true && project.CurrentWorkflowSequence != project.LastUpdateWorkflowSequence)
                {
                    project = await GenerateNewProjectVersion(project.ProjectId, project.ProjectVersion);
                    scopeOfWorkDto.ProjectVersion = project.ProjectVersion;
                    scopeOfWorkDto.CopyProperties(project);
                    _projectRepository.Update(project);
                    await _unitOfWorks.SaveChangesAsync();
                }
                else
                {
                    scopeOfWorkDto.CopyProperties(project);
                    _projectRepository.Update(project);
                    await _unitOfWorks.SaveChangesAsync();
                }

                var action = await _projectActionRepository.GetAction(project.CurrentAction);
                var workflowSequence = await _workflowSequenceRepository.GetWorkflowSequence(action!.WorkflowSequenceId!);

                if (scopeOfWorkDto.SaveLog.GetValueOrDefault())
                {
                    await GenerateDraftLog(project.ProjectId, project.CurrentWorkflowSequence, action?.WorkflowActionId);
                }

                await _projectPlatformRepository.UpdateAsync(project.ProjectId, project.ProjectVersion, _mapper.Map<List<TxProjectPlatform>>(scopeOfWorkDto.Platform));
                await _projectPipelineRepository.UpdatedAsync(project.ProjectId, project.ProjectVersion, _mapper.Map<List<TxProjectPipeline>>(scopeOfWorkDto.Pipeline));
                await _projectCompressorRepository.UpdateAsync(project.ProjectId, project.ProjectVersion, _mapper.Map<List<TxProjectCompressor>>(scopeOfWorkDto.Compressor));
                await _projectEquipmentRepository.UpdateAsync(project.ProjectId, project.ProjectVersion, _mapper.Map<List<TxProjectEquipment>>(scopeOfWorkDto.Equipment));

                await _projectAuditTrailRepository.SaveAuditTrailCreation(project.ProjectId, project.ProjectVersion, scopeOfWorkDto.Section);
                await _projectAuditTrailRepository.SaveAuditTrailUpdateScopeOfWork(project.ProjectId, project.ProjectVersion);
                await transaction.CommitAsync();
                return _mapper.Map<UpdateScopeOfWorkDto>(project);
            }
            catch
            {
                transaction.Rollback();
                throw;
            }

        }

        public async Task<TxProjectHeader?> UpdateEconomicIndicator(UpdateEconomicIndicatorRequest request)
        {
            var project = await _projectRepository.GetProjectHeader(request.ProjectId, request.ProjectVersion);
            if (project == null)
                return null;

            using var transaction = _unitOfWorks.BeginTransaction();
            try
            {
                if (project.IsActive == false)
                {
                    throw new InvalidOperationException("cannot update inactive template");
                }

                if (project.IsActive == true && project.CurrentWorkflowSequence != project.LastUpdateWorkflowSequence)
                {
                    project = await GenerateNewProjectVersion(project.ProjectId, project.ProjectVersion);
                    request.ProjectVersion = project.ProjectVersion;
                    request.CopyProperties(project);
                    _projectRepository.Update(project);
                    await _unitOfWorks.SaveChangesAsync();
                }
                else
                {
                    request.CopyProperties(project);
                    _projectRepository.Update(project);
                    await _unitOfWorks.SaveChangesAsync();
                }

                var action = await _projectActionRepository.GetAction(project.CurrentAction);
                var workflowSequence = await _workflowSequenceRepository.GetWorkflowSequence(action!.WorkflowSequenceId!);

                if (request.SaveLog.GetValueOrDefault())
                {
                    await GenerateDraftLog(project.ProjectId, project.CurrentWorkflowSequence, action?.WorkflowActionId);
                }

                await _projectAuditTrailRepository.SaveAuditTrailCreation(project.ProjectId, project.ProjectVersion, request.Section);
                await _projectAuditTrailRepository.SaveAuditTrailUpdateEconomicIndicator(project.ProjectId, project.ProjectVersion);
                await _unitOfWorks.SaveChangesAsync();
                await transaction.CommitAsync();
                return project;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<TxProjectHeader?> UpdateInitiationDocuments(UpdateInitiationDocsRequest request)
        {
            var project = await _projectRepository.GetProjectHeader(request.ProjectId, request.ProjectVersion);
            if (project == null)
                return null;

            using var transaction = _unitOfWorks.BeginTransaction();
            try
            {
                if (project.IsActive == true && project.CurrentWorkflowSequence != project.LastUpdateWorkflowSequence)
                {
                    project = await GenerateNewProjectVersion(project.ProjectId, project.ProjectVersion);
                    _projectRepository.Update(project);
                }
                else
                {
                    _projectRepository.Update(project);
                }

                var action = await _projectActionRepository.GetAction(project.CurrentAction);
                var workflowSequence = await _workflowSequenceRepository.GetWorkflowSequence(action!.WorkflowSequenceId!);

                if (request.SaveLog.GetValueOrDefault())
                {
                    await GenerateDraftLog(project.ProjectId, project.CurrentWorkflowSequence, action?.WorkflowActionId);
                }

                await _projectAuditTrailRepository.SaveAuditTrailCreation(project.ProjectId, project.ProjectVersion, request.Section);
                await _projectAuditTrailRepository.SaveAuditTrailUpdateInitiationDocs(project.ProjectId, project.ProjectVersion);
                await _unitOfWorks.SaveChangesAsync();
                await transaction.CommitAsync();
                return project;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<ProjectSequenceTimelineResponse?> GetProjectSequence(string projectId, int projectVersion)
        {
            var projSequence = await _projectRepository.GetProjectSequence(projectId, projectVersion);
            return projSequence;
        }

        public async Task<ProjectDetailResponse> GetProjectDetail(string projectId, int projectVersion)
        {
            var tph = await _projectRepository.GetProjectHeader(projectId, projectVersion);
            if (tph == null)
                throw new InvalidOperationException("Specified Project not found");

            var action = await _projectActionRepository.GetProjectActionWithRefWorkflowAction(tph.CurrentAction);
            if (action == null)
                throw new Exception("Error determining current action");

            var ret = _mapper.Map<ProjectDetailResponse>(tph);
            ret.WorkflowActionId = action.WorkflowActionId;
            ret.OutstandingTask = action.WorkflowAction?.WorkflowActionName;
            ret.Section = await _projectAuditTrailRepository.GetSection(projectId, projectVersion);
            ret.LogSectionUpdatedDate = await _projectAuditTrailRepository.GetUpdateDate(projectId, projectVersion);

            var entities = await _projectUpstreamEntityRepository.GetProjectUpstreamEntities(ret.ProjectId);
            var entityDetail = await _hierLvlVwRepository.GetShuHier03(entities.Select(n => n.EntityId).ToList());
            ret.HierLvl2 = new HierLvlDto { Key = entityDetail[0].HierLvl2!, Value = entityDetail[0].HierLvl2Desc! };
            ret.HierLvl3 = new HierLvlDto { Key = entityDetail[0].HierLvl3!, Value = entityDetail[0].HierLvl3Desc! };
            ret.HierLvl4 = entityDetail.Select(n =>
            {
                return new HierLvlDto { Key = n.HierLvl4!, Value = HierLvlDto.FormatHierLvl4(n) };
            }).ToList();
            var template = await _workflowSettingRepository.GetTemplateWithThesholdName(tph.TemplateId, tph.TemplateVersion);
            ret.ProjectCategory = template?.ProjectCategory;
            ret.ProjectCriteria = template?.ProjectCriteria;
            ret.ProjectSubCriteria = template?.ProjectSubCriteria;
            ret.TemplateName = template?.TemplateName;
            ret.Threshold = template?.Threshold;
            ret.TemplateLocked = await _projectMilestoneRepository.MilestoneExist(projectId, projectVersion);
            return ret;
        }

        public async Task<GetScopeOfWorkDropdownDto> GetDropdownAsync()
        {
            var res = await _workflowSettingRepository.GetScopeOfWorkDropdownList();
            return _mapper.Map<GetScopeOfWorkDropdownDto>(res);
        }

        public async Task<ProjectDocumentGroupResponse> GetDocumentGroupOfProject(string projectActionId)
        {
            return await _documentRepository.GetDocumentGroupFromProjectAction(projectActionId);
        }

        public async Task<ProjectSequenceActionsResponse> GetProjectSequenceAction(string projectId, string workflowSequenceId)
        {
            var sequence = await _workflowSequenceRepository.GetByWorkflowSequenceId(workflowSequenceId);
            var workflowActions = await _refWorkflowRepository.GetListWorkflowActionWithCard(sequence.WorkflowId!, sequence.WorkflowIsOptional); 
            var performedActions = await _projectActionRepository.GetProjectAction(projectId, workflowSequenceId);
            var groupedActions = performedActions.GroupBy(n => n.WorkflowActionId);
            var notPerformedActions = new List<RefWorkflowAction>(workflowActions);

            var ret = new ProjectSequenceActionsResponse();
            foreach(var performedAction in groupedActions)
            {
                ProjectSequenceAction action = new ProjectSequenceAction();
                TxProjectAction lastAction = performedAction.Last();
                action.ProjectAction = lastAction;
                action.WorkflowAction = workflowActions.First(n => n.WorkflowActionId == lastAction.WorkflowActionId);
                
                foreach(var actor in workflowActions.First(n => n.WorkflowActionId == lastAction.WorkflowActionId).RefWorkflowActors)
                {
                    var paramRole = await _parameterRepository.GetParam("idams", "Roles", actor.Actor);
                    var actorName = paramRole!.ParamValue1Text;
                    action.ActorName.Add(actorName!);
                }

                var notPerformedAction = notPerformedActions.FirstOrDefault(n => n.WorkflowActionId == lastAction.WorkflowActionId);
                if (notPerformedAction != null)
                    notPerformedActions.Remove(notPerformedAction);
                ret.Actions.Add(action);
                if (lastAction.Status == StatusConstant.Skipped)
                    return ret;
            }
            foreach(var notPerformedAction in notPerformedActions)
            {
                ProjectSequenceAction action = new ProjectSequenceAction();
                action.WorkflowAction = notPerformedAction;

                foreach (var actor in workflowActions.First(n => n.WorkflowActionId == notPerformedAction.WorkflowActionId).RefWorkflowActors)
                {
                    var paramRole = await _parameterRepository.GetParam("idams", "Roles", actor.Actor);
                    var actorName = paramRole!.ParamValue1Text;
                    action.ActorName.Add(actorName!);
                }

                ret.Actions.Add(action);
            }
            return ret;
        }

        public async Task<UpdateResourcesDto?> UpdateResources(UpdateResourcesDto resourcesDto)
        {
            var project = await _projectRepository.GetProjectHeader(resourcesDto.ProjectId, resourcesDto.ProjectVersion);
            if(project == null)
            {
                return null;
            }
            using var transaction = _unitOfWorks.BeginTransaction();
            try
            {
                if (project.IsActive == false)
                {
                    throw new InvalidOperationException("cannot update inactive template");
                }

                if (project.IsActive == true && project.CurrentWorkflowSequence != project.LastUpdateWorkflowSequence)
                {
                    project = await GenerateNewProjectVersion(project.ProjectId, project.ProjectVersion);
                    project.Oil = resourcesDto.Oil;
                    project.OilUoM = "MMBO";
                    project.Gas = resourcesDto.Gas;
                    project.GasUoM = "BSCF";
                    project.OilEquivalent = resourcesDto.OilEquivalent;
                    project.OilEquivalentUoM = "MMBOE";
                    _projectRepository.Update(project);
                    await _unitOfWorks.SaveChangesAsync();
                }
                else
                {
                    project.Oil = resourcesDto.Oil;
                    project.OilUoM = "MMBO";
                    project.Gas = resourcesDto.Gas;
                    project.GasUoM = "BSCF";
                    project.OilEquivalent = resourcesDto.OilEquivalent;
                    project.OilEquivalentUoM = "MMBOE";
                    _projectRepository.Update(project);
                    await _unitOfWorks.SaveChangesAsync();
                }


                var action = await _projectActionRepository.GetAction(project.CurrentAction);
                var workflowSequence = await _workflowSequenceRepository.GetWorkflowSequence(action!.WorkflowSequenceId!);

                if (resourcesDto.SaveLog.GetValueOrDefault())
                {
                    await GenerateDraftLog(project.ProjectId, project.CurrentWorkflowSequence, action?.WorkflowActionId);
                }

                await _projectAuditTrailRepository.SaveAuditTrailCreation(project.ProjectId, project.ProjectVersion, resourcesDto.Section);
                await _projectAuditTrailRepository.SaveAuditTrailUpdateResources(project.ProjectId, project.ProjectVersion);

                await transaction.CommitAsync();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            return _mapper.Map<UpdateResourcesDto>(project);
        }

        public async Task<GetMilestoneDto?> GetMilestone(string projectId, int projectVersion)
        {
            GetMilestoneDto res = new GetMilestoneDto();
            List<MilestoneDto> milestones = new List<MilestoneDto>();
            var project = await _projectRepository.GetProjectHeader(projectId, projectVersion);
            if (project == null) return null;

            var template = await _workflowSettingRepository.GetByTemplateIdAndTemplateVersion(project.TemplateId, project.TemplateVersion);
            res.TemplateName = template.TemplateName;


            var item = await _projectRepository.GetMilestone(projectId, projectVersion);
            if (item == null) return null;
            
            foreach(var milestone in item)
            {
                var workflow = await _refWorkflowRepository.GetByWorkflowSequenceId(milestone.WorkflowSequenceId);
                var parameter = await _parameterRepository.GetParam("idams", "Stage", workflow?.WorkflowCategoryParId);
                var action = await _projectActionRepository.GetLastProjectAction(projectId, milestone.WorkflowSequenceId);
                MilestoneDto dto = new MilestoneDto()
                {
                    WorkflowSequenceId = milestone.WorkflowSequenceId,
                    WorkflowName = milestone.WorkflowSequence.WorkflowName ?? "",
                    WorkflowType = workflow?.WorkflowType,
                    WorkflowCategory = parameter?.ParamValue1Text,
                    Sla = milestone.WorkflowSequence.Sla,
                    StartDate = milestone.StartDate,
                    EndDate = milestone.EndDate,    
                    Done = action == null ? "" : action.Status == StatusConstant.Completed || action.Status == StatusConstant.Skipped ? "true" : "false"
                };
                milestones.Add(dto);
            }
            res.Milestone = milestones;
            return res;
        }

        public async Task<UpdateMilestoneDto?> UpdateMilestone(UpdateMilestoneDto milestoneDto)
        {
            List<TxProjectMilestone> milestones = new List<TxProjectMilestone>();
            var item = await _projectRepository.GetProjectHeader(milestoneDto.ProjectId, milestoneDto.ProjectVersion.GetValueOrDefault());
            if (item == null) return null;
            if (item.IsActive == false)
            {
                throw new InvalidOperationException("cannot update inactive template");
            }

            var template = await _workflowSettingRepository.GetByTemplateIdAndTemplateVersion(item.TemplateId, item.TemplateVersion);
            
            var allSequence = template.RefTemplateWorkflowSequences.ToList();

            foreach(var data in milestoneDto.Milestone)
            {
                var seq = allSequence.Where(n => n.WorkflowSequenceId == data.WorkflowSequenceId).SingleOrDefault();
                if(seq == null)
                {
                    throw new InvalidDataException("Workflow Sequence it's not in project");
                }

                if(data.StartDate > data.EndDate)
                {
                    throw new InvalidDataException("Start Date must be less than End Date");
                }

                TxProjectMilestone txProjectMilestone = new TxProjectMilestone()
                {
                    ProjectId = milestoneDto.ProjectId,
                    ProjectVersion = milestoneDto.ProjectVersion.GetValueOrDefault(),
                    WorkflowSequenceId = data.WorkflowSequenceId,
                    StartDate = data.StartDate,
                    EndDate = data.EndDate,
                };
                milestones.Add(txProjectMilestone);
            }

            var firstSeq = milestoneDto.Milestone.OrderBy(n => n.StartDate).First();
            var lastSeq = milestoneDto.Milestone.OrderBy(n => n.EndDate).Last();

            item.InitiationDate = firstSeq.StartDate;
            item.EndDate = lastSeq.EndDate;

            using var transaction = _unitOfWorks.BeginTransaction();
            try
            {
                if(item.IsActive == true && item.CurrentWorkflowSequence != item.LastUpdateWorkflowSequence)
                {
                    item = await GenerateNewProjectVersion(item.ProjectId, item.ProjectVersion);
                    item.InitiationDate = firstSeq.StartDate;
                    item.EndDate = lastSeq.EndDate;

                    var newMilestone = new List<TxProjectMilestone>();
                    var milestoneNoTrack = await _projectMilestoneRepository.GetMilestonesAsync(item.ProjectId, item.ProjectVersion);
                    foreach (var milestone in milestoneNoTrack)
                    {
                        var match = milestones.Where(n => n.WorkflowSequenceId == milestone.WorkflowSequenceId).SingleOrDefault();
                        milestone.StartDate = match!.StartDate;
                        milestone.EndDate = match!.EndDate;
                    }
                    await _projectMilestoneRepository.UpdateMilestoneAsync(milestoneNoTrack);
                    _projectRepository.Update(item);
                    await _unitOfWorks.SaveChangesAsync();
                }
                else
                {
                    await _projectMilestoneRepository.UpdateMilestoneAsync(milestones);
                    _projectRepository.Update(item);
                    await _unitOfWorks.SaveChangesAsync();
                }

                var action = await _projectActionRepository.GetAction(item.CurrentAction);
                var workflowSequence = await _workflowSequenceRepository.GetWorkflowSequence(action!.WorkflowSequenceId!);

                if (milestoneDto.SaveLog.GetValueOrDefault())
                {
                    await GenerateDraftLog(item.ProjectId, item.CurrentWorkflowSequence, action?.WorkflowActionId);
                }

                await _projectAuditTrailRepository.SaveAuditTrailCreation(item.ProjectId, item.ProjectVersion, milestoneDto?.Section);
                await _projectAuditTrailRepository.SaveAuditTrailUpdateMilestone(item.ProjectId, item.ProjectVersion);
                await transaction.CommitAsync();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            return new UpdateMilestoneDto
            {
                ProjectId = item.ProjectId,
                ProjectVersion = item.ProjectVersion
            };
        }
        
        public async Task<GetScopeOfWorkDto?> GetScopeOfWork(string projectId, int projectVersion)
        {
            var item = await _projectRepository.GetScopeOfWork(projectId, projectVersion);
            return item;
        }

        public async Task<bool?> InitiateProject(string projectId, int projectVersion)
        {
            var projectHeader = await _projectRepository.GetProjectHeader(projectId, projectVersion);
            var projectAction = await _projectActionRepository.GetAction(projectHeader.CurrentAction);
            using var transaction = _unitOfWorks.BeginTransaction(IsolationLevel.RepeatableRead);
            try
            {
                if (projectHeader.Status != StatusConstant.Draft &&
                    projectHeader.Status != StatusConstant.Rejected)
                {
                    throw new Exception($"Cannot initiate project that has status : {projectHeader.Status}");
                }
                projectHeader.Status = StatusConstant.Inprogress;
                //update last action to complete
                projectAction.Status = StatusConstant.Completed;
                await _projectActionRepository.UpdateAsync(projectAction);
                await _unitOfWorks.SaveChangesAsync();

                await UpdateProjectWorkflowAndMoveToNextAction(projectHeader, projectAction);
                await _unitOfWorks.SaveChangesAsync();

                LgProjectActivityAuditTrail logActivity = new LgProjectActivityAuditTrail()
                {
                    ProjectId = projectHeader.ProjectId,
                    WorkflowSequenceId = projectHeader.CurrentWorkflowSequence,
                    Action = ActionConstant.SubmitProject,
                    ActivityStatusParId = StatusConstant.Inprogress,
                    ActivityDescription = "Submitted Project Initiation",
                    WorkflowActionId = projectAction.WorkflowActionId,
                };
                await _lgProjectActivityAuditTrailRepository.AddLog(logActivity);

                var additionalData = await _projectRepository.GetThresholdRegionalZonaByProjectId(projectId);
                List<SendMailRequest> sendMailReqList = new List<SendMailRequest>();
                SendMailRequest sendMailRequest = new SendMailRequest()
                {
                    EmailCode = "IDAMS.ProjectInitiation",
                    ActionBy = _currentUserService.CurrentUserInfo.Email,
                    To = _currentUserService.CurrentUserInfo.Email,
                    ListJson = new List<BaseKeyValueObject> {
                        new BaseKeyValueObject {Key = "User", Value = _currentUserService.CurrentUserInfo.Name},
                        new BaseKeyValueObject {Key = "Project_Name", Value = projectHeader.ProjectName},
                        new BaseKeyValueObject {Key = "Project_Category", Value=_parameterRepository.GetParam("idams", "ProjectCategory", projectHeader.Template!.ProjectCategoryParId).Result!.ParamValue1Text},
                        new BaseKeyValueObject {Key = "Project_Criteria", Value = _parameterRepository.GetParam("idams", "ProjectCriteria", projectHeader.Template!.ProjectCriteriaParId).Result!.ParamValue1Text},
                        new BaseKeyValueObject {Key = "Project_SubCriteria", Value = projectHeader.Template.ProjectSubCriteriaParId == null ? "-" :  _parameterRepository.GetParam("idams", "SubProjectCriteria", projectHeader.Template!.ProjectSubCriteriaParId).Result!.ParamValue1Text ?? ""},
                        new BaseKeyValueObject {Key = "Project_Threshold", Value = _parameterRepository.GetParam("idams", "ThresholdName", projectHeader.Template!.ThresholdNameParId).Result!.ParamValue1Text},
                        new BaseKeyValueObject {Key = "Project_Regional", Value = additionalData!.HierLvl2Desc},
                        new BaseKeyValueObject {Key = "Project_Zona", Value = additionalData.HierLvl3Desc}
                    }
                };
                sendMailReqList.Add(sendMailRequest);
                await _emailService.SendMailAsync(sendMailReqList);
                await transaction.CommitAsync();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            return true;
        }

        private async Task<TxProjectAction> ValidateExistAndType(string projectActionId, string type)
        {
            var projectAction = await _projectActionRepository.GetProjectActionWithRefWorkflowAction(projectActionId);
            if (projectAction == null)
            {
                throw new Exception($"projectActionId {projectActionId} not found");
            }
            if (projectAction.WorkflowAction?.WorkflowActionTypeParId != type)
            {
                throw new Exception($"projectActionId {projectActionId} is not {type} type");
            }
            return projectAction;
        }

        public async Task<UpdateDataStatusResponse> GetUpdateDataStatus(string projectActionId)
        {
            var projectAction = await ValidateExistAndType(projectActionId, WorkflowActionTypeConstant.UpdateData);
            var approvalDetail = await _approvalRepository.GetApprovalDetail(projectActionId);
            
            UpdateDataStatusResponse ret = new()
            {
                CreatedDate = projectAction.CreatedDate,
                ApprovalDate = approvalDetail.Date,
                EmpName = approvalDetail.EmpName,
                Status = projectAction.Status,
            };

            return ret;
        }

        public async Task<UpdateDataStatusResponse> CompleteWorkflowUpdatedata(string projectActionId, bool complete)
        {
            using var transaction = _unitOfWorks.BeginTransaction(IsolationLevel.RepeatableRead);
            try
            {
                var projectAction = await ValidateExistAndType(projectActionId, WorkflowActionTypeConstant.UpdateData);
                var approval = await _approvalRepository.UpdateApprovalData(new ApprovalRequest { Approval = complete, ProjectActionId = projectActionId });
                var projectHeader = await _projectRepository.GetLatestVersionProjectHeader(projectAction!.ProjectId!);
                var workflowSequence = await _workflowSequenceRepository.GetWorkflowSequence(projectAction!.WorkflowSequenceId!);

                if (complete)
                {
                    projectAction.Status = StatusConstant.Completed;
                    await UpdateProjectWorkflowAndMoveToNextAction(projectHeader!, projectAction);

                    await GenerateLogActivityProject(projectHeader.ProjectId, projectHeader.CurrentWorkflowSequence, ActionConstant.CompleteUpdateProjectData,
                                                               projectHeader.Status, string.Format("Complete Update Project Data & Document Project for {0}", workflowSequence!.WorkflowName), projectAction?.WorkflowActionId);
                }
                else
                {
                    projectAction.Status = StatusConstant.Revised;
                    await UpdateProjectWorkflowAndMoveToPreviousActon(projectHeader!, projectAction);
                }
                await _projectActionRepository.UpdateAsync(projectAction);
                await _unitOfWorks.SaveChangesAsync();
                await transaction.CommitAsync();
                return new UpdateDataStatusResponse
                {
                    ApprovalDate = approval.ApprovalDate,
                    CreatedDate = projectAction.CreatedDate,
                    EmpName = approval.EmpName,
                    Status = approval.ApprovalStatusParId
                };
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<FollowUpDropDownDto> GetFollowUpDropdownList()
        {
            return await _followUpRepository.GetDropdownList();
        }

        public async Task<List<FollowUpDetailResponse>> GetFollowUpList(string projectActionId)
        {
            return await _followUpRepository.GetFollowUpList(projectActionId);
        }

        public async Task<List<TxFollowUp>> AddFollowUpList(List<FollowUpRequest> followUpList)
        {
            using var transaction = _unitOfWorks.BeginTransaction();
            try
            {
                var ret = await _followUpRepository.AddFollowUpList(followUpList);
                await transaction.CommitAsync();
                return ret;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<TxFollowUp> UpdateFollowUp(FollowUpRequest followUp)
        {
            return await _followUpRepository.UpdateFollowUp(followUp);
        }

        public async Task<bool> DeleteFollowUp(int followUpId, string projectActionId)
        {
            return await _followUpRepository.DeleteFollowUp(followUpId, projectActionId);
        }

        public async Task<ApprovalDetailDto> GetApprovalDetail(string projectActionId)
        {
            await ValidateExistAndType(projectActionId, WorkflowActionTypeConstant.Approval);
            return await _approvalRepository.GetApprovalDetail(projectActionId);
        }

        public async Task<TxApproval?> GetLastApprovalData(string projectId)
        {
            return await _approvalRepository.GetLastApprovalData(projectId);
        }

        public async Task<TxApproval> UpdateApprovalData(ApprovalRequest approvalRequst)
        {
            using var transaction = _unitOfWorks.BeginTransaction(IsolationLevel.RepeatableRead);
            try
            {
                var ret = await _approvalRepository.UpdateApprovalData(approvalRequst);
                var projectAction = await _projectActionRepository.GetAction(approvalRequst.ProjectActionId);
                var projectHeader = await _projectRepository.GetLatestVersionProjectHeader(projectAction!.ProjectId!);
                
                var projectActionWithWorkflowAction = await _projectActionRepository.GetProjectActionWithRefWorkflowAction(projectAction.ProjectActionId!);
                var workflowAction = projectActionWithWorkflowAction!.WorkflowAction;

                if (approvalRequst.Approval) // approved, move to next action
                {
                    projectAction.Status = StatusConstant.Completed;
                    await UpdateProjectWorkflowAndMoveToNextAction(projectHeader!, projectAction);
                }
                else // revised, move back to prev action
                {
                    projectAction.Status = StatusConstant.Revised;
                    await UpdateProjectWorkflowAndMoveToPreviousActon(projectHeader!, projectAction);
                }
                await _projectActionRepository.UpdateAsync(projectAction);
                await _unitOfWorks.SaveChangesAsync();

                var workflowSequence = await _workflowSequenceRepository.GetWorkflowSequence(projectAction!.WorkflowSequenceId!);

                bool isApprovalRequirement = workflowAction!.WorkflowActionName == WorkflowActionConstant.ApprovalRequirement ? true : false; 
                LgProjectActivityAuditTrail logHistory = new LgProjectActivityAuditTrail()
                {
                    ProjectId = projectHeader!.ProjectId,
                    WorkflowSequenceId = projectAction.WorkflowSequenceId,
                    ActivityDescription = approvalRequst.Approval ? String.Format("Approve {0}", workflowSequence?.WorkflowName) : 
                                           String.Format("Reject {0}", workflowSequence?.WorkflowName),
                    ActivityStatusParId = StatusConstant.Inprogress,
                    WorkflowActionId = projectAction.WorkflowActionId,
                };

                if (approvalRequst.Approval)
                {
                    logHistory.Action = isApprovalRequirement ? ActionConstant.ApproveReqChecklistDoc : ActionConstant.Approve;
                    logHistory.ActivityDescription = isApprovalRequirement ? string.Format("Approve Requirement Checklist Document for {0}", workflowSequence?.WorkflowName) : string.Format("Approve {0}", workflowSequence?.WorkflowName);   
                }
                else
                {
                    logHistory.Action = isApprovalRequirement ? ActionConstant.RejectReqChecklistDoc : ActionConstant.Reject;
                    logHistory.ActivityDescription = isApprovalRequirement ? string.Format("Reject Requirement Checklist Document for {0}", workflowSequence?.WorkflowName) : string.Format("Reject {0}", workflowSequence?.WorkflowName);
                    
                    //if action in approval project initiation , change status to rejected
                    if(workflowAction.WorkflowActionId == WorkflowIdConstant.ApprovalInitiationReg || workflowAction.WorkflowActionId == WorkflowIdConstant.ApprovalInitiationShu)
                    {
                        logHistory.ActivityStatusParId = StatusConstant.Rejected;
                    }
                }
                await _lgProjectActivityAuditTrailRepository.AddLog(logHistory);
                await transaction.CommitAsync();
                return ret;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<ConfirmationDetailDto> GetConfirmationDetail(string projectActionId)
        {
            return await _approvalRepository.GetConfirmationDetail(projectActionId);
        }

        public async Task<ConfirmationDetailDto> UpdateConfirmationData(ConfirmationRequest confirmation)
        {
            using var transaction = _unitOfWorks.BeginTransaction(IsolationLevel.RepeatableRead);
            try
            {
                var ret = await _approvalRepository.UpdateConfirmationData(confirmation);
                var projectAction = await _projectActionRepository.GetAction(confirmation.ProjectActionId);
                var projectHeader = await _projectRepository.GetLatestVersionProjectHeader(projectAction!.ProjectId!);
                if (confirmation.Approval) // approved, move to next action
                {
                    await UpdateProjectWorkflowAndMoveToNextAction(projectHeader!, projectAction);
                }
                else // rejected, means sequence is not executed move to next sequence
                {
                    await UpdateProjectWorkflowAndMoveToNextAction(projectHeader!, projectAction, true);
                }
                await _unitOfWorks.SaveChangesAsync();
                await transaction.CommitAsync();
                return ret;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private async Task<bool> UpdateProjectWorkflowAndMoveToPreviousActon(TxProjectHeader projectHeader, TxProjectAction projectAction)
        {
            //get all workflow sequences
            var workflowSequences = await _workflowSequenceRepository.GetListTemplateWorkflowSequence(projectHeader.TemplateId, projectHeader.TemplateVersion);
            var currentSequence = workflowSequences.First(n => n.WorkflowSequenceId == projectHeader.CurrentWorkflowSequence);

            //check current available actions in current sequence
            var currentAvailableActions = await _refWorkflowRepository.GetListWorkflowAction(currentSequence.WorkflowId!, currentSequence.WorkflowIsOptional);
            RefWorkflowAction? prevAction = null;
            for (int i = 1; i < currentAvailableActions.Count; i++)
            {
                if (currentAvailableActions[i].WorkflowActionId == projectAction.WorkflowActionId)
                {
                    prevAction = currentAvailableActions[i - 1];
                    break;
                }
            }

            if(prevAction == null)
            {
                throw new InvalidDataException("Not allowed to move to previous sequences");
            }

            var prevProjectAction = await _projectActionRepository.GetProjectAction(projectHeader.ProjectId, currentSequence.WorkflowSequenceId, prevAction.WorkflowActionId);

            if(prevProjectAction == null)
            {
                throw new InvalidDataException("Previous project action not found");
            }

            prevProjectAction.Status = StatusConstant.Inprogress;
            await _projectActionRepository.UpdateAsync(prevProjectAction);

            //add new approval entity if the prev action is updateData
            if (prevAction.WorkflowActionTypeParId == WorkflowActionTypeConstant.Approval ||
                prevAction.WorkflowActionTypeParId == WorkflowActionTypeConstant.UpdateData)
            {
                await _approvalRepository.GenerateNewApproval(prevProjectAction.ProjectActionId);
            }

            //update project header current action and current workflow sequence
            projectHeader.CurrentAction = prevProjectAction.ProjectActionId;

            //if back to draft project initiation, change status to rejected
            if (prevProjectAction.WorkflowActionId == WorkflowIdConstant.DraftInitiationShu ||
                prevProjectAction.WorkflowActionId == WorkflowIdConstant.DraftInitiationReg)
            {
                projectHeader.Status = StatusConstant.Rejected;
            }
            _projectRepository.Update(projectHeader);
            await _unitOfWorks.SaveChangesAsync();
            return true;
        }

        private string GetActionByActor(List<RefWorkflowActor> refWorkflowActors)
        {
            foreach (var wactor in refWorkflowActors)
            {
                foreach (var roles in _currentUserService.CurrentUserInfo.Roles)
                {
                    if (roles.Key == wactor.Actor)
                    {
                        return wactor.Action;
                    }
                }
            }
            return "Submit"; //if no action found, just return submit
        }

        private async Task<bool> UpdateProjectWorkflowAndMoveToNextAction(TxProjectHeader projectHeader, TxProjectAction projectAction, bool? skipCurrentSequence = null)
        {
            //get all workflow sequences
            var workflowSequences = await _workflowSequenceRepository.GetListTemplateWorkflowSequence(projectHeader.TemplateId, projectHeader.TemplateVersion);
            var currentSequence = workflowSequences.First(n => n.WorkflowSequenceId == projectHeader.CurrentWorkflowSequence);

            //check current available actions in current sequence
            var currentAvailableActions = await _refWorkflowRepository.GetListWorkflowAction(currentSequence.WorkflowId!, currentSequence.WorkflowIsOptional);
            RefWorkflowAction? nextAction = null;

            RefWorkflow? workflow = null;
            string actionByActor = "";
            string? aimanTransNo = null;

            await SendEmailFinalDocument(projectHeader, projectAction);

            if (skipCurrentSequence != true)
            {
                for (int i = 0; i < currentAvailableActions.Count - 1; i++)
                {
                    if (currentAvailableActions[i].WorkflowActionId == projectAction.WorkflowActionId)
                    {
                        nextAction = currentAvailableActions[i + 1];
                        break;
                    }
                }
            }

            //if there is no action in this sequence, move to next sequence
            if (nextAction == null)
            {
                RefTemplateWorkflowSequence? nextSequence = null;
                for (int i = 0; i < workflowSequences.Count - 1; i++)
                {
                    if (workflowSequences[i].WorkflowSequenceId == currentSequence.WorkflowSequenceId)
                    {
                        nextSequence = workflowSequences[i + 1];
                        break;
                    }
                }

                //no next sequence found, project completed
                if (nextSequence == null)
                {
                    projectHeader.Status = StatusConstant.Completed;
                    _projectRepository.Update(projectHeader);
                    await _unitOfWorks.SaveChangesAsync();
                    return true;
                }

                var nextAvailableAction = await _refWorkflowRepository.GetListWorkflowActionWithCard(nextSequence.WorkflowId!, nextSequence.WorkflowIsOptional);
                nextAction = nextAvailableAction.First();
                currentSequence = nextSequence;


                //add default aimantransno if sequence change
                aimanTransNo = "1";
            }

            TxProjectAction? nextProjectAction = await _projectActionRepository.GetProjectAction(projectHeader.ProjectId, currentSequence.WorkflowSequenceId, nextAction.WorkflowActionId);

            //// do transaction -- disabled not used
            //workflow = await _refWorkflowRepository.GetByWorkflowId(currentSequence?.WorkflowId!);
            //actionByActor = GetActionByActor(nextAction.RefWorkflowActors.ToList());
            //if (projectAction.AimanTransactionNumber == null || aimanTransNo == "1")
            //{
            //    aimanTransNo = "1";
            //}
            //else
            //{
            //    aimanTransNo = projectAction.AimanTransactionNumber;
            //}
            //PHEMadamAPIDto? resdoTransaction = await _workflowIntegrationService.DoTransaction(actionByActor, aimanTransNo, workflow!.WorkflowMadamPk!, _currentUserService.CurrentUserInfo.EmpAccount, 
            //                                    _currentUserService.CurrentUserInfo.EmpAccount);

            if (nextProjectAction == null)
            {
                //add new action with next workflow action if not already exist
                nextProjectAction = new TxProjectAction()
                {
                    ProjectId = projectHeader.ProjectId,
                    Status = StatusConstant.Inprogress,
                    WorkflowSequenceId = currentSequence.WorkflowSequenceId,
                    WorkflowActionId = nextAction.WorkflowActionId,
                    //AimanTransactionNumber = resdoTransaction?.Object?.Value
                };
                nextProjectAction = await _projectActionRepository.AddAsync(nextProjectAction);
            }
            else
            {
                // already exist, only update status
                nextProjectAction.Status = StatusConstant.Inprogress;
                //nextProjectAction.AimanTransactionNumber = resdoTransaction?.Object?.Value;
                await _projectActionRepository.UpdateAsync(nextProjectAction);
            }


            //update project header current action and current workflow sequence
            projectHeader.CurrentAction = nextProjectAction.ProjectActionId;
            projectHeader.CurrentWorkflowSequence = currentSequence.WorkflowSequenceId;
            _projectRepository.Update(projectHeader);
            await _unitOfWorks.SaveChangesAsync();

            //add new approval entity if the next action is approval or confirmation or updateData
            if (nextAction.WorkflowActionTypeParId == WorkflowActionTypeConstant.Approval ||
                nextAction.WorkflowActionTypeParId == WorkflowActionTypeConstant.Confirmation ||
                nextAction.WorkflowActionTypeParId == WorkflowActionTypeConstant.UpdateData)
            {
                await _approvalRepository.GenerateNewApproval(nextProjectAction.ProjectActionId);
            }

            return true;
        }

        private async Task SendEmailFinalDocument(TxProjectHeader projectHeader, TxProjectAction projectAction)
        {

            if (projectAction.WorkflowActionId != WorkflowIdConstant.UploadDokumenFinalInisiasi && projectAction.WorkflowActionId != WorkflowIdConstant.UploadDokumentFinalSeleksiRegional
                && projectAction.WorkflowActionId != WorkflowIdConstant.UploadDokumentFinalSeleksiSHUHolding && projectAction.WorkflowActionId != WorkflowIdConstant.UploadDokumentFinalKajianLanjutRegional &&
                projectAction.WorkflowActionId != WorkflowIdConstant.UploadDokumentFinalKajianLanjutSHU && projectAction.WorkflowActionId != WorkflowIdConstant.UploadDokumentFinalKajianLanjutHolding)
            {
                return;
            }
            var documents = await _documentRepository.GetDocumentGroupFromProjectAction(projectAction.ProjectActionId);

            List<SendMailRequest> sendMailReqList = new List<SendMailRequest>();
            var link = _parameterRepository.GetParam("idams", "url", "DocumentLink").Result!.ParamValue1Text! + "?projectId=" + projectHeader.ProjectId + "&projectVersion=" + projectHeader.ProjectVersion + "tab=projectDocument" + "<br>";
            SendMailRequest sendMailRequest = new SendMailRequest()
            {
                AppFk = _parameterRepository.GetParam("idams", "appFK", "appFK").Result!.ParamValue1Text!,
                EmailCode = "",
                TransNo = "transno",
                ActionBy = _currentUserService.CurrentUserInfo.Email,
                EmailSender = "phe.apps@pertamina.com",
                To = _currentUserService.CurrentUserInfo.Email,
                Bcc = _parameterRepository.GetParam("idams", "Bcc", "BccEmail").Result!.ParamValue1Text!,
                ListJson = new List<BaseKeyValueObject> {
                        new BaseKeyValueObject {Key = "User", Value = _currentUserService.CurrentUserInfo.Name},
                        new BaseKeyValueObject {Key = "Project_Name", Value = projectHeader.ProjectName},
                        new BaseKeyValueObject {Key = "Stage_Name", Value = ""},
                        new BaseKeyValueObject {Key = "Document_List", Value = ""},
                        new BaseKeyValueObject {Key = "Link", Value = link}
                    }
            };
            var kvStage = sendMailRequest.ListJson.Find(x => x.Key == "Stage_Name");
            var refWf = await _refWorkflowRepository.GetByWorkflowSequenceId(projectHeader.CurrentWorkflowSequence!);
            var stg = refWf!.WorkflowCategoryParId;
            var stgParamList = await _parameterRepository.GetParam("idams", "stage", stg);
            kvStage!.Value = stgParamList!.ParamValue1Text;

            var kvDocumentList = sendMailRequest.ListJson.Find(x => x.Key == "Document_List");
            foreach(var doc in documents.RequiredDocs)
            {
                kvDocumentList!.Value += doc.RequiredName;
                kvDocumentList.Value += "<br>";
            }
            foreach(var doc in documents.SupportingDocs)
            {
                kvDocumentList!.Value += doc.RequiredName;
                kvDocumentList.Value += "<br>";
            }

            //send email for action upload dokumen final in every stage 
            if (projectAction.WorkflowActionId == WorkflowIdConstant.UploadDokumenFinalInisiasi)
            {
                sendMailRequest.EmailCode = "IDAMS.FinalDocumentInisiasiUploaded";
                sendMailReqList.Add(sendMailRequest);
            }
            else if (projectAction.WorkflowActionId == WorkflowIdConstant.UploadDokumentFinalSeleksiRegional || projectAction.WorkflowActionId == WorkflowIdConstant.UploadDokumentFinalSeleksiSHUHolding)
            {
                sendMailRequest.EmailCode = "IDAMS.FinalDocumentSeleksiUploaded";
                sendMailReqList.Add(sendMailRequest);
            }
            else if (projectAction.WorkflowActionId == WorkflowIdConstant.UploadDokumentFinalKajianLanjutRegional || projectAction.WorkflowActionId == WorkflowIdConstant.UploadDokumentFinalKajianLanjutSHU ||
                projectAction.WorkflowActionId == WorkflowIdConstant.UploadDokumentFinalKajianLanjutHolding) 
            {
                sendMailRequest.EmailCode = "IDAMS.FinalDocumentKajianLanjutUploaded";
                sendMailReqList.Add(sendMailRequest);

                //get user that initiate project
                List<LgProjectActivityAuditTrail> logs = await _lgProjectActivityAuditTrailRepository.GetLogByActivityDescription(projectHeader.ProjectId, "Submitted Project Initiation");
                var logLast = logs.Last();
                var userEmailInitiate = logLast.EmpAccount;
                var userNameInitiate = logLast.EmpName;

                //get user email that create project
                var masterEmployeeUrl = _parameterRepository.GetParam("idams", "Url", "GetAllMasterEmployee").Result!.ParamValue1Text;
                await _userService.GetBaseUrlListAsync();
                var employee = await _userService.GetEmployee(projectHeader.CreatedBy!, masterEmployeeUrl!);
                var userEmailCreation = employee == null ? "" : employee.EmpEmail;

                //construct new send mail request for project initiator
                var sendMailReq = new SendMailRequest();
                sendMailRequest.CopyProperties(sendMailReq);
                var listJson = new List<BaseKeyValueObject>();
                sendMailRequest.ListJson.ForEach(n =>
                {
                    var bkv = new BaseKeyValueObject();
                    n.CopyProperties(bkv);
                    listJson.Add(bkv);
                });
                sendMailReq.ListJson = listJson;

                sendMailReq.To = userEmailInitiate;
                var kvUser = sendMailReq.ListJson.Find(n => n.Key == "User");
                kvUser!.Value = userNameInitiate;

                sendMailReqList.Add(sendMailReq);

                //construct new send mail request for project creator 
                var sendMailRequestCreate = new SendMailRequest();
                sendMailRequest.CopyProperties(sendMailRequestCreate);
                var listJsonCreate = new List<BaseKeyValueObject>();
                sendMailRequest.ListJson.ForEach(n =>
                {
                    var bkv = new BaseKeyValueObject();
                    n.CopyProperties(bkv);
                    listJsonCreate.Add(bkv);
                });
                sendMailRequestCreate.ListJson = listJsonCreate;

                sendMailRequestCreate.To = userEmailCreation;
                var kvUserCreate = sendMailRequestCreate.ListJson.Find(n => n.Key == "User");
                kvUser!.Value = projectHeader.CreatedBy;

                sendMailReqList.Add(sendMailRequestCreate);
            }
            await _emailService.SendMailAsync(sendMailReqList);
        }

        public async Task<Paged<LogHistoryDto>> GetHistoryPaged(PagedDto paged, LogHistoryFilter filter, List<RoleEnum> roles)
        {
            var item = await _lgProjectActivityAuditTrailRepository.GetPaged(filter, roles, paged.Page, paged.Size, paged.Sort);
            return _mapper.Map<Paged<LogHistoryDto>>(item);
        }

        public async Task<List<MeetingDto>> GetMeetingList(UserDto user, string projectActionId)
        {
            List<TxMeeting> item;
            var allData = false;
            var action = await _projectActionRepository.GetAction(projectActionId);
            var actor = await _projectActionRepository.GetActorByAction(action!.WorkflowActionId!);
            foreach(var role in user.Roles)
            {
                var roleEnum = role.Enum.ToString();
                if(actor.Any(n => n.Actor == roleEnum))
                {
                    allData = true;
                    break;
                }
            }
            if (allData)
            {
                item = await _txMeetingRepositories.GetListAsync(projectActionId, false);
            }
            else
            {
                item = await _txMeetingRepositories.GetListWithoutDraftAsync(projectActionId);
            }
            return _mapper.Map<List<MeetingDto>>(item);
        }

        public async Task<MeetingDetailDto> GetMeetingDetail(string projectActionId, int meetingId)
        {
            var item = await _txMeetingRepositories.GetMeetingWithParticipant(projectActionId, meetingId);
            var res = _mapper.Map<MeetingDetailDto>(item);
            var additionalData = await _projectRepository.GetThresholdRegionalZonaByProjectId(item.ProjectAction.ProjectId!);
            var param = await _parameterRepository.GetParam("idams", "ThresholdName", additionalData!.ThresholdNameParId);

            res.Threshold = param!.ParamValue1Text;
            res.Regional = additionalData.HierLvl2Desc;
            res.Zona = additionalData.HierLvl3Desc;

            var participant = _mapper.Map<List<MeetingParticipantDto>>(item.TxMeetingParticipants);
            res.Participants = participant;
            return res;
        }

        public async Task<MeetingDetailDto> AddMeeting(MeetingDetailDto meeting)
        {
            var meetingEntity = _mapper.Map<TxMeeting>(meeting);
            using var transaction  = _unitOfWorks.BeginTransaction();
            try
            {
                var meetingId = 1;
                var meetings = await _txMeetingRepositories.GetListAsync(meeting.ProjectActionId, true);
                if(meetings.Count > 0)
                {
                    meetingId = meetings.OrderBy(n => n.MeetingId).Last().MeetingId + 1;
                    meetingEntity.MeetingId = meetingId;
                }
                else
                {
                    meetingEntity.MeetingId = meetingId;
                }
                var res = await _txMeetingRepositories.AddMeetingAsync(meetingEntity);

                List<TxMeetingParticipant> participants = new List<TxMeetingParticipant>();
                foreach(var participant in meeting.Participants)
                {
                    var participantEntity = _mapper.Map<TxMeetingParticipant>(participant);
                    participantEntity.MeetingId = meetingId;
                    participantEntity.ProjectActionId = meeting.ProjectActionId;
                    participants.Add(participantEntity);                    
                }
                await _txMeetingParticipantRepository.AddAsync(participants);

                var action = await _projectActionRepository.GetAction(meeting.ProjectActionId);
                var projectHeader = await _projectRepository.GetLatestVersionProjectHeader(action!.ProjectId!);
                var workflowSequence = await _workflowSequenceRepository.GetWorkflowSequence(action.WorkflowSequenceId!);
                LgProjectActivityAuditTrail logHistory = new LgProjectActivityAuditTrail()
                {
                    ProjectId = action.ProjectId,
                    WorkflowSequenceId = action.WorkflowSequenceId,
                    ActivityStatusParId = StatusConstant.Inprogress,
                    WorkflowActionId = action.WorkflowActionId,
                };

                if (meeting.Status == StatusConstant.Draft)
                {
                    logHistory.Action = ActionConstant.SaveDraft;
                    logHistory.ActivityDescription = "Save Meeting as Draft";
                }
                else
                {
                    logHistory.Action = ActionConstant.CreateMeeting;
                    logHistory.ActivityDescription = String.Format("Schedule Meeting for {0}", workflowSequence.WorkflowName);
                }

                await _lgProjectActivityAuditTrailRepository.AddLog(logHistory);

                if(meeting.Status == StatusConstant.Scheduled)
                {
                    List<SendMailRequest> sendMailReqList = new List<SendMailRequest>();
                    SendMailRequest sendMailRequest = new SendMailRequest()
                    {
                        EmailCode = "IDAMS.MeetingCreated",
                        ActionBy = _currentUserService.CurrentUserInfo.Email,
                        To = _currentUserService.CurrentUserInfo.Email,
                        ListJson = new List<BaseKeyValueObject> {
                            new BaseKeyValueObject {Key = "User", Value = _currentUserService.CurrentUserInfo.Name},
                            new BaseKeyValueObject {Key = "Meeting_Name", Value = meetingEntity.Title},
                            new BaseKeyValueObject {Key = "Workflow_Name", Value = action.WorkflowSequence!.WorkflowName},
                            new BaseKeyValueObject {Key = "Project_Name", Value = projectHeader!.ProjectName},
                            new BaseKeyValueObject {Key = "Meeting_Date", Value = meetingEntity.Date.GetValueOrDefault().ToString("dd/MM/yyyy")},
                            new BaseKeyValueObject {Key = "Meeting_Start_Hour", Value = meetingEntity.Start.ToString()},
                            new BaseKeyValueObject {Key = "Meeting_End_Hour", Value = meetingEntity.End.ToString()},
                            new BaseKeyValueObject {Key = "Meeting_Location", Value = meetingEntity.Location},
                            new BaseKeyValueObject {Key = "Meeting_Participant", Value = ""}
                        }
                    };
                    var kvMeetingParticipant = sendMailRequest.ListJson.Find(x => x.Key == "Meeting_Participant");
                    foreach (var participant in meeting.Participants)
                    {
                        kvMeetingParticipant!.Value += participant.EmpEmail;
                        kvMeetingParticipant!.Value += "<br>";
                    }
                    sendMailReqList.Add(sendMailRequest);


                    foreach (var participant in meeting.Participants)
                    {
                        //for meeting invitation
                        var sendMailReq = new SendMailRequest();
                        sendMailRequest.CopyProperties(sendMailReq);
                        var listJson = new List<BaseKeyValueObject>();
                        sendMailRequest.ListJson.ForEach(n =>
                        {
                            var bkv = new BaseKeyValueObject();
                            n.CopyProperties(bkv);
                            listJson.Add(bkv);
                        });
                        sendMailReq.ListJson = listJson;

                        sendMailReq.EmailCode = "IDAMS.MeetingInvitation";
                        sendMailReq.To = participant.EmpEmail!;
                        sendMailReq.ListJson.Find(n => n.Key == "User")!.Value = (!string.IsNullOrEmpty(participant.EmpName) ? participant.EmpName : participant.EmpEmail);
                        sendMailReqList.Add(sendMailReq);
                    }
                    await _emailService.SendMailAsync(sendMailReqList);
                }
                await transaction.CommitAsync();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            return _mapper.Map<MeetingDetailDto>(meetingEntity);
        }

        public async Task<MeetingDetailDto?> UpdateMeeting(MeetingDetailDto meeting)
        {
            var meetingEntity = _mapper.Map<TxMeeting>(meeting);
            using var transaction = _unitOfWorks.BeginTransaction();
            try
            {
                var meetingExisting = await _txMeetingRepositories.GetMeeting(meeting.ProjectActionId, meeting.MeetingId.GetValueOrDefault());
                if(meetingExisting == null) return null;
                meetingEntity.CopyProperties(meetingExisting);
                var res = await _txMeetingRepositories.UpdateMeetingAsync(meetingExisting);

                List<TxMeetingParticipant> participants = new List<TxMeetingParticipant>();
                foreach (var participant in meeting.Participants)
                {
                    var participantEntity = _mapper.Map<TxMeetingParticipant>(participant);
                    participantEntity.MeetingId = meeting.MeetingId.GetValueOrDefault();
                    participantEntity.ProjectActionId = meetingEntity.ProjectActionId;
                    participants.Add(participantEntity);
                }
                List<TxMeetingParticipant> txMeetingParticipants = await _txMeetingParticipantRepository.GetListAsync(meeting.ProjectActionId,
                    meeting.MeetingId.GetValueOrDefault());
                await _txMeetingParticipantRepository.DeleteAsync(txMeetingParticipants);
                await _txMeetingParticipantRepository.AddAsync(participants);

                var action = await _projectActionRepository.GetAction(meeting.ProjectActionId);
                var projectHeader = await _projectRepository.GetLatestVersionProjectHeader(action!.ProjectId!);
                var workflowSequence = await _workflowSequenceRepository.GetWorkflowSequence(action!.WorkflowSequenceId!);
                LgProjectActivityAuditTrail logHistory = new LgProjectActivityAuditTrail()
                {
                    ProjectId = action.ProjectId,
                    WorkflowSequenceId = action.WorkflowSequenceId,
                    ActivityStatusParId = StatusConstant.Inprogress,
                    WorkflowActionId = action.WorkflowActionId,
                };

                List<SendMailRequest> sendMailReqList = new List<SendMailRequest>();
                SendMailRequest sendMailRequest = new SendMailRequest()
                {
                    EmailCode = "",
                    ActionBy = _currentUserService.CurrentUserInfo.Email,
                    To = "",
                    ListJson = new List<BaseKeyValueObject> {
                        new BaseKeyValueObject {Key = "User", Value = ""},
                        new BaseKeyValueObject {Key = "Meeting_Name", Value = meetingEntity.Title},
                        new BaseKeyValueObject {Key = "Workflow_Name", Value = action.WorkflowSequence!.WorkflowName},
                        new BaseKeyValueObject {Key = "Project_Name", Value = projectHeader!.ProjectName},
                        new BaseKeyValueObject {Key = "Meeting_Date", Value = meetingEntity.Date.GetValueOrDefault().ToString("dd/MM/yyyy")},
                        new BaseKeyValueObject {Key = "Meeting_Start_Hour", Value = meetingEntity.Start.ToString()},
                        new BaseKeyValueObject {Key = "Meeting_End_Hour", Value = meetingEntity.End.ToString()},
                        new BaseKeyValueObject {Key = "Meeting_Location", Value = meetingEntity.Location},
                        new BaseKeyValueObject {Key = "Meeting_Notes", Value = meetingEntity.Notes},
                        new BaseKeyValueObject {Key = "Meeting_Participant", Value = ""}
                    }
                };
                var kvMeetingParticipant = sendMailRequest.ListJson.Find(x => x.Key == "Meeting_Participant");
                foreach (var participant in meeting.Participants)
                {
                    kvMeetingParticipant!.Value += participant.EmpEmail;
                    kvMeetingParticipant!.Value += "<br>";
                }
                if (meeting.Status == StatusConstant.Completed)
                {
                    logHistory.Action = ActionConstant.CompleteMeeting;
                    logHistory.ActivityDescription = String.Format("Complete a Meeting for {0}", workflowSequence!.WorkflowName);

                    foreach (var participant in meeting.Participants)
                    {
                        var sendMailReq = new SendMailRequest();
                        sendMailRequest.CopyProperties(sendMailReq);
                        var listJson = new List<BaseKeyValueObject>();
                        sendMailRequest.ListJson.ForEach(n =>
                        {
                            var bkv = new BaseKeyValueObject();
                            n.CopyProperties(bkv);
                            listJson.Add(bkv);
                        });
                        sendMailReq.ListJson = listJson;

                        sendMailReq.EmailCode = "IDAMS.OfflineMeetingCompleted";
                        sendMailReq.To = participant.EmpEmail!;
                        sendMailReq.ListJson.Find(n => n.Key == "User")!.Value = (!string.IsNullOrEmpty(participant.EmpName) ? participant.EmpName : participant.EmpEmail);
                        sendMailReqList.Add(sendMailReq);
                    }
                    await _emailService.SendMailAsync(sendMailReqList);
                }
                else if(meeting.Status == StatusConstant.Canceled)
                {
                    logHistory.Action = ActionConstant.CancelMeeting;
                    logHistory.ActivityDescription = String.Format("Cancel a Meeting for {0}", workflowSequence!.WorkflowName);

                    foreach(var participant in meeting.Participants)
                    {
                        var sendMailReq = new SendMailRequest();
                        sendMailRequest.CopyProperties(sendMailReq);
                        var listJson = new List<BaseKeyValueObject>();
                        sendMailRequest.ListJson.ForEach(n =>
                        {
                            var bkv = new BaseKeyValueObject();
                            n.CopyProperties(bkv);
                            listJson.Add(bkv);
                        });
                        sendMailReq.ListJson = listJson;

                        sendMailReq.EmailCode = "IDAMS.OfflineMeetingCanceled";
                        sendMailReq.To = participant.EmpEmail!;
                        sendMailReq.ListJson.Find(n => n.Key == "User")!.Value = (!string.IsNullOrEmpty(participant.EmpName) ? participant.EmpName : participant.EmpEmail);
                        sendMailReqList.Add(sendMailReq);
                    }
                    await _emailService.SendMailAsync(sendMailReqList);
                }
                else if(meeting.Status == StatusConstant.Scheduled)
                {
                    logHistory.Action = ActionConstant.CreateMeeting;
                    logHistory.ActivityDescription = String.Format("Schedule Meeting for {0}", workflowSequence!.WorkflowName);
                }
                else
                {
                    logHistory.Action = ActionConstant.SaveDraft;
                    logHistory.ActivityDescription = "Save Meeting as Draft";
                }
                await _lgProjectActivityAuditTrailRepository.AddLog(logHistory);
                await transaction.CommitAsync();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            return _mapper.Map<MeetingDetailDto>(meetingEntity);
        }

        public async Task<bool> DeleteMeeting(string projectActionId, int meetingId)
        {
            var item = await _txMeetingRepositories.GetMeeting(projectActionId, meetingId);
            if(item?.MeetingStatusParId == StatusConstant.Completed)
            {
                return false;
            }
            var action = await _projectActionRepository.GetAction(projectActionId);
            var projectHeader = await _projectRepository.GetLatestVersionProjectHeader(action.ProjectId);
            var workflowSequence = await _workflowSequenceRepository.GetWorkflowSequence(action.WorkflowSequenceId);

            using var transaction = _unitOfWorks.BeginTransaction();
            try
            {
                LgProjectActivityAuditTrail logHistory = new LgProjectActivityAuditTrail()
                {
                    ProjectId = action.ProjectId,
                    WorkflowSequenceId = action.WorkflowSequenceId,
                    Action = ActionConstant.DeleteMeeting,
                    ActivityDescription = String.Format("Delete Meeting for {0}", workflowSequence.WorkflowName),
                    ActivityStatusParId = StatusConstant.Inprogress,
                    WorkflowActionId = action.WorkflowActionId,
                };
                await _lgProjectActivityAuditTrailRepository.AddLog(logHistory);
                item.Deleted = true;
                await _txMeetingRepositories.UpdateMeetingAsync(item);

                List<SendMailRequest> sendMailReqList = new List<SendMailRequest>();
                SendMailRequest sendMailRequest = new SendMailRequest()
                {
                    EmailCode = "IDAMS.OfflineMeetingDeleted",
                    ActionBy = _currentUserService.CurrentUserInfo.Email,
                    To = "",
                    ListJson = new List<BaseKeyValueObject> {
                    new BaseKeyValueObject {Key = "User", Value = ""},
                    new BaseKeyValueObject {Key = "Meeting_Name", Value = item.Title},
                    new BaseKeyValueObject {Key = "Workflow_Name", Value = action.WorkflowSequence!.WorkflowName},
                    new BaseKeyValueObject {Key = "Project_Name", Value = projectHeader!.ProjectName},
                    new BaseKeyValueObject {Key = "Meeting_Date", Value = item.Date.GetValueOrDefault().ToString("dd/MM/yyyy")},
                    new BaseKeyValueObject {Key = "Meeting_Start_Hour", Value = item.Start.ToString()},
                    new BaseKeyValueObject {Key = "Meeting_End_Hour", Value = item.End.ToString()},
                    new BaseKeyValueObject {Key = "Meeting_Location", Value = item.Location},
                    new BaseKeyValueObject {Key = "Meeting_Participant", Value = ""}
                }
                };
                var kvUser = sendMailRequest.ListJson.Find(n => n.Key == "User");
                var kvMeetingParticipant = sendMailRequest.ListJson.Find(x => x.Key == "Meeting_Participant");
                foreach (var participant in item.TxMeetingParticipants)
                {
                    kvMeetingParticipant!.Value += participant.EmpEmail;
                    kvMeetingParticipant!.Value += "<br>";
                }
                foreach (var participant in item.TxMeetingParticipants)
                {
                    var sendMailReq = new SendMailRequest();
                    sendMailRequest.CopyProperties(sendMailReq);
                    var listJson = new List<BaseKeyValueObject>();
                    sendMailRequest.ListJson.ForEach(n =>
                    {
                        var bkv = new BaseKeyValueObject();
                        n.CopyProperties(bkv);
                        listJson.Add(bkv);
                    });
                    sendMailReq.ListJson = listJson;

                    sendMailReq.To = participant.EmpEmail!;
                    sendMailReq.ListJson.Find(n => n.Key == "User")!.Value = (!string.IsNullOrEmpty(participant.EmpName) ? participant.EmpName : participant.EmpEmail);
                    sendMailReqList.Add(sendMailReq);
                }
                await _emailService.SendMailAsync(sendMailReqList);
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<bool> CompleteAllMeeting(string projectActionId)
        {
            using var transaction = _unitOfWorks.BeginTransaction(IsolationLevel.RepeatableRead);

            try
            {
                var projectAction = await _projectActionRepository.GetAction(projectActionId);
                if(projectAction.Status != StatusConstant.Inprogress)
                {
                    throw new Exception($"{projectActionId} already {projectAction.Status}");
                }
                var projectHeader = await _projectRepository.GetLatestVersionProjectHeader(projectAction!.ProjectId!);
                projectAction.Status = StatusConstant.Completed;    
                await _projectActionRepository.UpdateAsync(projectAction);
                await UpdateProjectWorkflowAndMoveToNextAction(projectHeader!, projectAction);


                var workflowSequence = await _workflowSequenceRepository.GetWorkflowSequence(projectAction!.WorkflowSequenceId!);
                LgProjectActivityAuditTrail logHistory = new LgProjectActivityAuditTrail()
                {
                    ProjectId = projectHeader!.ProjectId,
                    WorkflowSequenceId = projectAction.WorkflowSequenceId,
                    Action = ActionConstant.CompleteAllMeeting,
                    ActivityDescription = String.Format("Complete all Meeting for {0}", workflowSequence!.WorkflowName),
                    ActivityStatusParId = StatusConstant.Inprogress,
                    WorkflowActionId = projectAction.WorkflowActionId,
                };
                await _lgProjectActivityAuditTrailRepository.AddLog(logHistory);

                await transaction.CommitAsync();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            return true;
        }

        public async Task<Paged<ProjectCommentDto>> GetCommentList(string projectId)
        {
            var listDto = new List<ProjectCommentDto>();

            var res = await _projectCommentRepository.GetCommentListAsync(projectId);
            foreach(var data in res.Items)
            {
                var dto =  _mapper.Map<ProjectCommentDto>(data);
                var action = await _projectActionRepository.GetAction(data.ProjectActionId);
                var workflowSequence = await _workflowSequenceRepository.GetByWorkflowSequenceId(action!.WorkflowSequenceId!);
                var workflow = await _refWorkflowRepository.GetByWorkflowId(workflowSequence!.WorkflowId!);
                var param = await _parameterRepository.GetParam("idams", "Stage", workflow.WorkflowCategoryParId);

                dto.Workflow = workflowSequence?.WorkflowName;
                dto.Stage = param!.ParamValue1Text;
                listDto.Add(dto);
            }
            return new Paged<ProjectCommentDto>
            {
                TotalItems = res.TotalItems,
                Items = listDto
            };
        }

        public async Task<ProjectCommentDto> AddComment(string projectId, string comment)
        {
            using var transaction = _unitOfWorks.BeginTransaction();
            TxProjectComment commentEntity = new TxProjectComment();
            try
            {
                var project = await _projectRepository.GetLatestVersionProjectHeader(projectId);
                commentEntity.ProjectId = projectId;
                commentEntity.ProjectActionId = project!.CurrentAction;

                var commentId = await _projectCommentRepository.GetLatestProjectCommentId(projectId);
                if (commentId == null) commentEntity.ProjectCommentId = 1;
                else commentEntity.ProjectCommentId = commentId.Value + 1;
                commentEntity.Comment = comment;
                await _projectCommentRepository.AddCommentAsync(commentEntity);
                await transaction.CommitAsync();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            return _mapper.Map<ProjectCommentDto>(commentEntity);
        }

        public async Task<bool> DeleteComment(string projectId, int projectCommentId, UserDto user)
        {
            var username = user.Name;
            using var transaction = _unitOfWorks.BeginTransaction();
            try
            {
                var comment = await _projectCommentRepository.GetCommentAsync(projectId, projectCommentId);
                if (comment == null) return false;

                if (username != comment.EmpName) throw new UnauthorizedAccessException("Cannot Delete comment, Authorization failed");

                await _projectCommentRepository.DeleteCommentAsync(comment);
                await transaction.CommitAsync();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            return true;
        }

        public Task<FollowUpDtoWithSum> UploadTemplateFollowUp(UploadTemplateFollowUpRequest request)
        {
            var res = new FollowUpDtoWithSum();
            var workbook = new XLWorkbook(request.File.OpenReadStream());
            var ws = workbook.Worksheet(1);
            var range = ws.RangeUsed();
            for(int i= 2; i <= range.RowCount(); i++)
            {
                var dto = new FollowUpDto()
                {
                    ReviewerName = ws.Cell(i, 1).Value.ToString(),
                    PositionFunction = ws.Cell(i, 2).Value.ToString(),
                    FollowUpAspectParId = ws.Cell(i, 3).Value.ToString(),
                    ReviewResult = ws.Cell(i, 4).Value.ToString(),
                    RiskDescription = ws.Cell(i, 5).Value.ToString(),
                    Recommendation = ws.Cell(i, 6).Value.ToString(),
                    RiskLevelParId = ws.Cell(i, 7).Value.ToString(),
                    Notes = ws.Cell(i, 8).Value.ToString(),

                };
                res.Dtos.Add(dto);
            }
            res.TotalData = res.Dtos.Count;
            res.TotalAspect = res.Dtos.DistinctBy(x => x.FollowUpAspectParId).Count();
            
            return Task.FromResult(res);
        }

        public async Task<bool> CompleteUploadDocument(string projectActionId)
        {
            using var transaction = _unitOfWorks.BeginTransaction(IsolationLevel.RepeatableRead);
            try
            {
                var projectAction = await _projectActionRepository.GetAction(projectActionId);

                var projectActionWithWorkflowAction = await _projectActionRepository.GetProjectActionWithRefWorkflowAction(projectAction.ProjectActionId!);
                var workflowAction = projectActionWithWorkflowAction!.WorkflowAction;

                if (projectAction.Status != StatusConstant.Inprogress)
                {
                    throw new Exception($"{projectActionId} already {projectAction.Status}");
                }
                var projectHeader = await _projectRepository.GetLatestVersionProjectHeader(projectAction!.ProjectId!);
                projectAction.Status = StatusConstant.Completed;
                await _projectActionRepository.UpdateAsync(projectAction);
                await UpdateProjectWorkflowAndMoveToNextAction(projectHeader!, projectAction);

                var workflowSequence = await _workflowSequenceRepository.GetWorkflowSequence(projectAction!.WorkflowSequenceId!);
                bool isRequirementChecklistDocAction = workflowAction!.WorkflowActionName == WorkflowActionConstant.RequirementChecklistDocument ? true : false;
                bool isUploadFidDoc = workflowAction!.WorkflowActionTypeParId == WorkflowActionTypeConstant.UploadFID ? true : false;
                LgProjectActivityAuditTrail logHistory = new LgProjectActivityAuditTrail()
                {
                    ProjectId = projectHeader!.ProjectId,
                    WorkflowSequenceId = projectAction.WorkflowSequenceId,
                    ActivityStatusParId = StatusConstant.Inprogress,
                    WorkflowActionId = projectAction.WorkflowActionId,
                };
                if (isRequirementChecklistDocAction)
                {
                    logHistory.Action = ActionConstant.UploadReqChecklistDoc;
                    logHistory.ActivityDescription = string.Format("Complete Upload Requirement Checklist Document for {0}", workflowSequence!.WorkflowName);
                }
                else if (isUploadFidDoc)
                {
                    logHistory.ActivityStatusParId = StatusConstant.Completed;
                    logHistory.Action = ActionConstant.UploadApprovalDocument;
                    logHistory.ActivityDescription = string.Format("Complete Upload FID Document for {0}", workflowSequence!.WorkflowName);
                }
                else if(workflowAction.WorkflowActionId == "022.6") // Upload FID Supporting Document
                {
                    logHistory.Action = ActionConstant.UploadFidSupportingDocument;
                    logHistory.ActivityDescription = string.Format("Complete Upload FID Supporting Document for {0}", workflowSequence!.WorkflowName);
                } 
                else
                {
                    logHistory.Action = ActionConstant.UploadDocument;
                    logHistory.ActivityDescription = string.Format("Complete Upload Document for {0}", workflowSequence!.WorkflowName);
                }
                await _lgProjectActivityAuditTrailRepository.AddLog(logHistory);

                await transaction.CommitAsync();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            return true;
        }

        public async Task<List<UpcomingMeetingDto>> GetUpcomingMeetings(string? projectId)
        {
            return await _upcomingMeetingRepository.GetList(projectId);
        }

        public async Task<CalendarEventDropdownDto> GetCalendarEventDropdown()
        {
            CalendarEventDropdownDto calendarEventDropdownDto = new CalendarEventDropdownDto();
            var project = await _projectRepository.GetAllExceptDraftProject();
            var threshold = await _parameterRepository.GetParams("idams", "ThresholdName");
            calendarEventDropdownDto.Project = project.ToDictionary(keySelector: t => t.ProjectId, elementSelector: t => t.ProjectName!);
            calendarEventDropdownDto.Threshold = threshold.ToDictionary(keySelector: t => t.ParamListId, elementSelector: t => t.ParamValue1Text!);
            return calendarEventDropdownDto;
        }

        public async Task<List<MeetingWithProjectAndStageDto>> GetCalendarEvent(DateTime startDate, DateTime endDate, string? projectId, string? threshold)
        {
            return await _txMeetingParticipantRepository.GetCalendarEvent(startDate, endDate, projectId, threshold);
        }

        public async Task<string> GenerateFidcode(string subholdingCode, string projectCategory, int approvedYear, string regional, string projectId, int projectVersion)
        {
            TxProjectHeader? projectHeader = await _projectRepository.GetProjectHeader(projectId, projectVersion);

            if (projectHeader == null)
            {
                throw new InvalidDataException("project not found");
            }

            string fidcode = await _fidcodeRepository.Add(subholdingCode, projectCategory, approvedYear, regional);

            projectHeader.Fidcode = fidcode;

            using var transaction = _unitOfWorks.BeginTransaction();

            try
            {
                _projectRepository.Update(projectHeader);
                await _unitOfWorks.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }

            return fidcode;
        }

        public async Task<string> InputFidcode(string fidcode, string projectId, int projectVersion)
        {
            string newFidCode = "R" + fidcode;

            Paged<ProjectListPaged>? project = await _projectRepository.GetPaged(new ProjectListFilter { Fidcode = newFidCode }, 1, 1, "ProjectId asc");

            if (project == null || project.TotalItems == 0)
            {
                TxProjectHeader? projectHeader = await _projectRepository.GetProjectHeader(projectId, projectVersion);

                if (projectHeader == null)
                {
                    throw new InvalidDataException("project not found");
                }

                projectHeader.Fidcode = newFidCode;

                using var transaction = _unitOfWorks.BeginTransaction();

                try
                {
                    _projectRepository.Update(projectHeader);
                    await _unitOfWorks.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }

                return newFidCode;
            }

            throw new InvalidDataException("This FID Number is already registered by project " + project.Items.First().ProjectName);
        }

        public async Task<Dictionary<string, string>> GetAvailableWorkflowType(List<string> stage)
        {
            List<RefWorkflow> listWorkflow = new List<RefWorkflow>();
            foreach(var st in stage)
            {
                List<RefWorkflow> lrw = await _refWorkflowRepository.GetWorkflowByStage(st);
                listWorkflow.AddRange(lrw);
            }
            return listWorkflow.ToDictionary(keySelector: t => t.WorkflowId!, elementSelector: t => t.WorkflowType!);
        }

        public async Task<Dictionary<string, string>> GetAvaiableStage(List<string> workflowType)
        {
            List<RefWorkflow> workflows = new List<RefWorkflow>();
            foreach(var x in workflowType)
            {
                RefWorkflow wf = await _refWorkflowRepository.GetByWorkflowId(x);
                workflows.Add(wf);
            }
            HashSet<string> wt = new HashSet<string>();
            foreach(var wf in workflows)
            {
                wt.Add(wf.WorkflowCategoryParId!);
            }
            return wt.ToDictionary(keySelector: t => t, elementSelector: t => _parameterRepository.GetParam("idams", "stage", t).Result!.ParamValue1Text!);

        }

        public async Task<int?> GetLatestProjectVersion(string projectId)
        {
            var project = await _projectRepository.GetLatestVersionProjectHeader(projectId);
            return project?.ProjectVersion;
        }
    }
}
