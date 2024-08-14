using AutoMapper;
using Idams.Core.Constants;
using Idams.Core.Enums;
using Idams.Core.Model;
using Idams.Core.Model.Dtos;
using Idams.Core.Model.Entities;
using Idams.Core.Model.Entities.Custom;
using Idams.Core.Model.Filters;
using Idams.Core.Model.Requests;
using Idams.Core.Model.Responses;
using Idams.Core.Repositories;
using Idams.Core.Services;
using Idams.Infrastructure.EntityFramework.Core;
using Idams.Infrastructure.EntityFramework.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Idams.Infrastructure.EntityFramework.Repositories
{
    public class EFProjectRepository : BaseRepository, IProjectRepository
    {
        private readonly IMapper _mapper;
        public EFProjectRepository(IdamsDbContext dbContext, IConfiguration configuration, ICurrentUserService currentUserService, IMapper mapper)
            : base(configuration, currentUserService, dbContext)
        {
            _mapper = mapper;
        }

        public async Task<Paged<ProjectListPaged>> GetPaged(ProjectListFilter filter, int page = 1, int size = 10, string sort = "projectId asc")
        {
            return await GetPaged<ProjectListPaged>(ProjectQuery.SelectPagedQuery, ProjectQuery.CountQuery,
                BuildProjectListQuery(filter), page, size, sort);
        }

        public async Task<TxProjectHeader?> GetProjectHeader(string projectId, int projectVersion)
        {
            return await _dbContext.TxProjectHeaders.AsNoTracking().Include(n => n.Template).Where(n => n.ProjectId == projectId && n.ProjectVersion == projectVersion).FirstOrDefaultAsync();
        }

        public async Task<TxProjectHeader?> GetLatestVersionProjectHeader(string projectId)
        {
            return await _dbContext.TxProjectHeaders.AsNoTracking().Where(n => n.ProjectId == projectId)
                .OrderByDescending(n => n.ProjectVersion).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ProjectBannerCount>> GetBannerCount(string? hierlvl2, string? hierlvl3)
        {
            List<List<FilterModel>> filters = new List<List<FilterModel>>();
            filters.Add(new List<FilterModel> { new FilterModel("tph.Deleted", FilterBuilderEnum.EQUALS, "0") });
            filters.Add(new List<FilterModel> { new FilterModel("tph.IsActive", FilterBuilderEnum.EQUALS, "1") });
            if (!string.IsNullOrEmpty(hierlvl2))
            {
                filters.Add(new List<FilterModel> { new FilterModel("HierLvl2", FilterBuilderEnum.EQUALS, hierlvl2) });
            }
            if (!string.IsNullOrEmpty(hierlvl3))
            {
                filters.Add(new List<FilterModel> { new FilterModel("HierLvl3", FilterBuilderEnum.EQUALS, hierlvl3) });
            }

            string filter = FilterModel.BuildFilter(filters);
            var metaData = new Dictionary<string, object>
            {
                { "filter", filter }
            };
            string sql = BuildQuery(ProjectQuery.ProjectBannerData, metaData);
            return await ExecuteQueryAsync<ProjectBannerCount>(sql);
        }

        public async Task<int> DeleteProject(string projectId, int projectVersion)
        {
            var proj = await _dbContext.TxProjectHeaders.Where(n => n.ProjectId == projectId && n.ProjectVersion == projectVersion).FirstOrDefaultAsync();
            if (proj == null)
            {
                return await Task.FromResult(0);
            }

            proj.Deleted = true;
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteProject(List<ProjectHeaderIdentifier> projects)
        {
            int ret = 0;
            foreach (var project in projects)
            {
                var res = await DeleteProject(project.ProjectId, project.ProjectVersion);
                ret += res;
            }
            return ret;
        }

        public async Task<TxProjectHeader> AddAsync(TxProjectHeader tph)
        {
            var latestProject = await _dbContext.TxProjectHeaders.AsNoTracking().OrderByDescending(n => n.ProjectId).FirstOrDefaultAsync();
            tph.ProjectId = IdGenerationUtil.GenerateNextId(latestProject?.ProjectId, "P");
            tph.ProjectVersion = 1;
            tph.CreatedDate = DateTime.UtcNow;
            tph.CreatedBy = GetCurrentUserInfo.EmpAccount;
            tph.UpdatedDate = DateTime.UtcNow;
            tph.UpdatedBy = GetCurrentUser;
            await _dbContext.TxProjectHeaders.AddAsync(tph);
            return tph;
        }
        public async Task<TxProjectHeader> AddProject(TxProjectHeader tph)
        {
            tph.CreatedDate = DateTime.UtcNow;
            tph.CreatedBy = GetCurrentUser;
            tph.UpdatedDate = DateTime.UtcNow;
            tph.UpdatedBy = GetCurrentUser;
            await _dbContext.TxProjectHeaders.AddAsync(tph);
            return tph;
        }

        public TxProjectHeader Update(TxProjectHeader tph)
        {
            tph.UpdatedDate = DateTime.UtcNow;
            tph.UpdatedBy = GetCurrentUser;
            _dbContext.TxProjectHeaders.Update(tph);
            return tph;
        }

        public async Task<ProjectSequenceTimelineResponse?> GetProjectSequence(string projectId, int projectVersion)
        {
            var totalComplete = 0;
            double percentage;
            var project = await _dbContext.TxProjectHeaders.AsNoTracking().Where(n => n.ProjectId == projectId && n.ProjectVersion == projectVersion).FirstOrDefaultAsync();
            if (project == null)
                return null;
            bool isProjectCompleted = project.Status == StatusConstant.Completed;
            var paramList = (from paramIdList in _dbContext.MdParamaterLists
                            where paramIdList.Schema == "idams" && paramIdList.ParamId == "Stage"
                            select paramIdList).AsNoTracking();

            var workflowProjects = await (from milestone in _dbContext.TxProjectMilestones
                         join workflowSeq in _dbContext.RefTemplateWorkflowSequences on milestone.WorkflowSequenceId equals workflowSeq.WorkflowSequenceId
                         join workflow in _dbContext.RefWorkflows on workflowSeq.WorkflowId equals workflow.WorkflowId
                         join pl in paramList on workflow.WorkflowCategoryParId equals pl.ParamListId
                         join wa in _dbContext.RefWorkflowActions on workflow.WorkflowId equals wa.WorkflowId
                         join pa in _dbContext.TxProjectActions on new { project.ProjectId, workflowSeq.WorkflowSequenceId, wa.WorkflowActionId } equals new {pa.ProjectId, pa.WorkflowSequenceId, pa.WorkflowActionId} into map from pa in map.DefaultIfEmpty()
                         where milestone.ProjectId == project.ProjectId && milestone.ProjectVersion == projectVersion && wa.WorkflowActionTypeParId != null && wa.WorkflowActionTypeParId != WorkflowActionTypeConstant.Confirmation
                         orderby workflowSeq.Order, wa.WorkflowActionId ascending
                         select new WorkflowProjectAction()
                         {
                             WorkflowSequenceId = workflowSeq.WorkflowSequenceId,
                             WorkflowName = workflowSeq.WorkflowName!,
                             Order = workflowSeq.Order,
                             WorkflowType = workflow.WorkflowType!,
                             WorkflowCategory = pl.ParamValue1Text!,
                             Start = milestone.StartDate,
                             End = milestone.EndDate,
                             IsOptional = workflowSeq.WorkflowIsOptional,
                             WorkflowActionId = wa.WorkflowActionId,
                             WorkflowActionName = wa.WorkflowActionName,
                             ActionStart = pa.CreatedDate,
                             ActionEnd = pa.UpdatedDate,
                             ActionStatus = pa.Status,
                         }).AsNoTracking().ToListAsync();

            var currentSeq = workflowProjects.First(n => n.WorkflowSequenceId == project.CurrentWorkflowSequence);
            var currentAction = await _dbContext.TxProjectActions.AsNoTracking().Where(n => n.ProjectActionId == project.CurrentAction).FirstOrDefaultAsync();
            if (currentAction == null)
                throw new InvalidDataException("current action not found");

            foreach(var workflow in workflowProjects)
            {
                if(workflow.Order < currentSeq.Order || isProjectCompleted)
                {
                    workflow.Status = StatusConstant.Completed;
                }
                else if(workflow.Order == currentSeq.Order)
                {
                    workflow.Status = StatusConstant.Inprogress;
                }
                else
                {
                    workflow.Status = StatusConstant.NotStarted;
                }
            }

            List<WorkflowProject> ret = new();
            var grouped = workflowProjects.GroupBy(n => n.WorkflowSequenceId);
            foreach(var group in grouped)
            {
                var first = group.First();
                var last = group.Last();
                if (first.Status == StatusConstant.Completed) totalComplete++;
                WorkflowProject wf = new()
                {
                    WorkflowSequenceId = first.WorkflowSequenceId,
                    WorkflowName = first.WorkflowName,
                    Order = first.Order,
                    WorkflowType = first.WorkflowType!,
                    WorkflowCategory = first.WorkflowCategory!,
                    Start = first.Start,
                    End = first.End,
                    Status = first.Status,
                    ActionStart = first.ActionStart,
                    ActionEnd = last.ActionStatus == StatusConstant.Completed ? last.ActionEnd : null
                };

                foreach (var action in group)
                {
                    string status = wf.Status;
                    if (wf.Status == StatusConstant.Inprogress)
                    {
                        bool less = double.Parse(action.WorkflowActionId) < double.Parse(currentAction.WorkflowActionId!);
                        status = less || isProjectCompleted ? StatusConstant.Completed : StatusConstant.NotStarted;
                    }
                    WorkflowActionDetail workflowActionDetail = new()
                    {
                        WorkflowActionId = action.WorkflowActionId,
                        WorkflowActionName = action.WorkflowActionName!,
                        Status = status
                    };
                    wf.WorkflowActions.Add(workflowActionDetail);
                }

                // update optional workflow status
                if(first.IsOptional == true && wf.Status != StatusConstant.Inprogress)
                {
                    var optionalAction = await _dbContext.TxProjectActions.AsNoTracking().Where(n => n.ProjectId == projectId &&
                            n.WorkflowSequenceId == wf.WorkflowSequenceId).FirstOrDefaultAsync();
                    if(optionalAction != null)
                    {
                        wf.Status = optionalAction.Status!;
                    }
                }
                ret.Add(wf);
            }
            if (isProjectCompleted)
            {
                percentage = 100.0;
            }
            else percentage = (double)totalComplete/ grouped.Count() * 100;
            return new ProjectSequenceTimelineResponse()
            {
                PercentageCompleted = Convert.ToInt32(percentage),
                Workflows = ret
            };
        }

        private List<List<FilterModel>> BuildProjectListQuery(ProjectListFilter filter)
        {
            List<List<FilterModel>> ret = new List<List<FilterModel>>();

            BuildNotDeleted(ret);
            BuildIsActive(ret);
            BuildProjectNameFilter(ret, filter);
            BuildHierLvl2Filter(ret, filter);
            BuildHierLvl2DropdownFilter(ret, filter);
            BuildHierLvl3Filter(ret, filter);
            BuildHierLvl4Filter(ret, filter);
            BuildThresholdFitler(ret, filter);
            BuildThresholdDropdownFitler(ret, filter);
            BuildDrillingCostFitler(ret, filter);
            BuildFacilityCostFitler(ret, filter);
            BuildCapexFitler(ret, filter);
            BuildEstFidFitler(ret, filter);
            BuildRKAPFitler(ret, filter);
            BuildRevisionFitler(ret, filter);
            BuildStageFitler(ret, filter);
            BuildStageDropdownFilter(ret, filter);
            BuildWorkflowNameFitler(ret, filter);
            BuildProjectOnStreamFitler(ret, filter);
            BuildNetPresentFitler(ret, filter);
            BuildInternalRateFitler(ret, filter);
            BuildProfitabilityIndexFitler(ret, filter);
            BuildOilFitler(ret, filter);
            BuildGasFitler(ret, filter);
            BuildOilEquivalentFitler(ret, filter);
            BuildInitiationDateFitler(ret, filter);
            BuildEndDateFitler(ret, filter);
            BuildWorkflowActionNameFitler(ret, filter);
            BuildStatusFitler(ret, filter);
            BuildSubCriteriaFilter(ret, filter);
            BuildUpdatedDateFitler(ret, filter);
            BuildWorkflowTypeFitler(ret, filter);
            BuildUserHierLvl2Filter(ret, filter);
            BuildUserHierLvl3Filter(ret, filter);
            BuildFidcodeFilter(ret, filter);
            BuildFidcodeLikeFilter(ret, filter);

            return ret;
        }

        private static void BuildNotDeleted(List<List<FilterModel>> ret)
        {
            ret.Add(new List<FilterModel> { new FilterModel("Deleted", FilterBuilderEnum.EQUALS, "0") });
        }

        private static void BuildIsActive(List<List<FilterModel>> ret)
        {
            ret.Add(new List<FilterModel> { new FilterModel("IsActive", FilterBuilderEnum.EQUALS, "1") });
        }

        private static void BuildWorkflowTypeFitler(List<List<FilterModel>> ret, ProjectListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.WorkflowType))
            {
                ret.Add(new List<FilterModel> { new FilterModel("WorkflowType", FilterBuilderEnum.LIKE, filter.WorkflowType) });
            }
        }

        private static void BuildUpdatedDateFitler(List<List<FilterModel>> ret, ProjectListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.UpdatedDate))
            {
                ret.Add(new List<FilterModel> { new FilterModel("convert(varchar(25), UpdatedDate, 113)", FilterBuilderEnum.LIKE, filter.UpdatedDate) });
            }
        }
        private static void BuildSubCriteriaFilter(List<List<FilterModel>> ret, ProjectListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.SubCriteria))
            {
                ret.Add(new List<FilterModel> { new FilterModel("SubCriteria", FilterBuilderEnum.LIKE, filter.SubCriteria) });
            }
        }
        private static void BuildStatusFitler(List<List<FilterModel>> ret, ProjectListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.Status))
            {
                ret.Add(new List<FilterModel> { new FilterModel("Status", FilterBuilderEnum.LIKE, filter.Status) });
            }
        }

        private static void BuildWorkflowActionNameFitler(List<List<FilterModel>> ret, ProjectListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.WorkflowActionName))
            {
                ret.Add(new List<FilterModel> { new FilterModel("WorkflowActionName", FilterBuilderEnum.LIKE, filter.WorkflowActionName) });
            }
        }

        private static void BuildEndDateFitler(List<List<FilterModel>> ret, ProjectListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.EndDate))
            {
                ret.Add(new List<FilterModel> { new FilterModel("convert(varchar(25), EndDate, 113)", FilterBuilderEnum.LIKE, filter.EndDate) });
            }
        }

        private static void BuildInitiationDateFitler(List<List<FilterModel>> ret, ProjectListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.InitiationDate))
            {
                ret.Add(new List<FilterModel> { new FilterModel("convert(varchar(25), InitiationDate, 113)", FilterBuilderEnum.LIKE, filter.InitiationDate) });
            }
        }

        private static void BuildOilEquivalentFitler(List<List<FilterModel>> ret, ProjectListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.OilEquivalent))
            {
                ret.Add(new List<FilterModel> { new FilterModel("cast(OilEquivalent as varchar(15))", FilterBuilderEnum.LIKE, filter.OilEquivalent) });
            }
        }

        private static void BuildGasFitler(List<List<FilterModel>> ret, ProjectListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.Gas))
            {
                ret.Add(new List<FilterModel> { new FilterModel("cast(Gas as varchar(15))", FilterBuilderEnum.LIKE, filter.Gas) });
            }
        }

        private static void BuildOilFitler(List<List<FilterModel>> ret, ProjectListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.Oil))
            {
                ret.Add(new List<FilterModel> { new FilterModel("cast(Gas as varchar(15))", FilterBuilderEnum.LIKE, filter.Oil) });
            }
        }

        private static void BuildProfitabilityIndexFitler(List<List<FilterModel>> ret, ProjectListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.ProfitabilityIndex))
            {
                ret.Add(new List<FilterModel> { new FilterModel("cast(ProfitabilityIndex as varchar(15))", FilterBuilderEnum.LIKE, filter.ProfitabilityIndex) });
            }
        }

        private static void BuildInternalRateFitler(List<List<FilterModel>> ret, ProjectListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.InternalRateOfReturn))
            {
                ret.Add(new List<FilterModel> { new FilterModel("cast(InternalRateOfReturn as varchar(15))", FilterBuilderEnum.LIKE, filter.InternalRateOfReturn) });
            }
        }

        private static void BuildNetPresentFitler(List<List<FilterModel>> ret, ProjectListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.NetPresentValue))
            {
                ret.Add(new List<FilterModel> { new FilterModel("cast(NetPresentValue as varchar(15))", FilterBuilderEnum.LIKE, filter.NetPresentValue) });
            }
        }

        private static void BuildProjectOnStreamFitler(List<List<FilterModel>> ret, ProjectListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.ProjectOnStream))
            {
                ret.Add(new List<FilterModel> { new FilterModel("convert(varchar(25), ProjectOnStream, 113)", FilterBuilderEnum.LIKE, filter.ProjectOnStream) });
            }
        }

        private static void BuildWorkflowNameFitler(List<List<FilterModel>> ret, ProjectListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.WorkflowName))
            {
                ret.Add(new List<FilterModel> { new FilterModel("WorkflowName", FilterBuilderEnum.LIKE, filter.WorkflowName) });
            }
        }

        private static void BuildStageFitler(List<List<FilterModel>> ret, ProjectListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.Stage))
            {
                ret.Add(new List<FilterModel> { new FilterModel("Stage", FilterBuilderEnum.LIKE, filter.Stage) });
            }
        }

        private static void BuildStageDropdownFilter(List<List<FilterModel>> ret, ProjectListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.StageDropdown))
            {
                ret.Add(new List<FilterModel> { new FilterModel("Stage", FilterBuilderEnum.LIKE, filter.StageDropdown) });
            }
        }

        private static void BuildRKAPFitler(List<List<FilterModel>> ret, ProjectListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.RKAP))
            {
                ret.Add(new List<FilterModel> { new FilterModel("cast(RKAP as varchar(15))", FilterBuilderEnum.LIKE, filter.RKAP) });
            }
        }

        private static void BuildRevisionFitler(List<List<FilterModel>> ret, ProjectListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.Revision))
            {
                ret.Add(new List<FilterModel> { new FilterModel("Revision", FilterBuilderEnum.EQUALS, filter.Revision) });
            }
        }

        private static void BuildEstFidFitler(List<List<FilterModel>> ret, ProjectListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.EstFIDApproved))
            {
                ret.Add(new List<FilterModel> { new FilterModel("convert(varchar(25), EstFIDApproved, 113)", FilterBuilderEnum.LIKE, filter.EstFIDApproved) });
            }
        }

        private static void BuildCapexFitler(List<List<FilterModel>> ret, ProjectListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.Capex))
            {
                ret.Add(new List<FilterModel> { new FilterModel("cast(Capex as varchar(15))", FilterBuilderEnum.LIKE, filter.Capex) });
            }
        }

        private static void BuildFacilityCostFitler(List<List<FilterModel>> ret, ProjectListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.FacilitiesCost))
            {
                ret.Add(new List<FilterModel> { new FilterModel("cast(FacilitiesCost as varchar(15))", FilterBuilderEnum.LIKE, filter.FacilitiesCost) });
            }
        }

        private static void BuildDrillingCostFitler(List<List<FilterModel>> ret, ProjectListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.DrillingCost))
            {
                ret.Add(new List<FilterModel> { new FilterModel("cast(DrillingCost as varchar(15))", FilterBuilderEnum.LIKE, filter.DrillingCost) });
            }
        }

        private static void BuildThresholdFitler(List<List<FilterModel>> ret, ProjectListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.Threshold))
            {
                ret.Add(new List<FilterModel> { new FilterModel("Threshold", FilterBuilderEnum.LIKE, filter.Threshold) });
            }
        }

        private static void BuildThresholdDropdownFitler(List<List<FilterModel>> ret, ProjectListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.ThresholdDropDown))
            {
                ret.Add(new List<FilterModel> { new FilterModel("Threshold", FilterBuilderEnum.LIKE, filter.ThresholdDropDown) });
            }
        }

        private static void BuildHierLvl4Filter(List<List<FilterModel>> ret, ProjectListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.HierLvl4))
            {
                ret.Add(new List<FilterModel> { new FilterModel("HierLvl4", FilterBuilderEnum.LIKE, filter.HierLvl4) });
            }
        }

        private static void BuildHierLvl3Filter(List<List<FilterModel>> ret, ProjectListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.HierLvl3Desc))
            {
                ret.Add(new List<FilterModel> { new FilterModel("HierLvl3Desc", FilterBuilderEnum.LIKE, filter.HierLvl3Desc) });
            }
        }

        private static void BuildHierLvl2Filter(List<List<FilterModel>> ret, ProjectListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.HierLvl2Desc))
            {
                ret.Add(new List<FilterModel> { new FilterModel("HierLvl2Desc", FilterBuilderEnum.LIKE, filter.HierLvl2Desc) });
            }
        }

        private static void BuildHierLvl2DropdownFilter(List<List<FilterModel>> ret, ProjectListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.HierLvl2DropDown))
            {
                ret.Add(new List<FilterModel> { new FilterModel("HierLvl2Desc", FilterBuilderEnum.LIKE, filter.HierLvl2DropDown) });
            }
        }

        private static void BuildProjectNameFilter(List<List<FilterModel>> ret, ProjectListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.ProjectName))
            {
                ret.Add(new List<FilterModel> { new FilterModel("ProjectName", FilterBuilderEnum.LIKE, filter.ProjectName) });
            }
        }

        private static void BuildUserHierLvl2Filter(List<List<FilterModel>> ret, ProjectListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.HierLvl2))
            {
                ret.Add(new List<FilterModel> { new FilterModel("HierLvl2", FilterBuilderEnum.EQUALS, filter.HierLvl2) });
            }
        }

        private static void BuildUserHierLvl3Filter(List<List<FilterModel>> ret, ProjectListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.HierLvl3))
            {
                ret.Add(new List<FilterModel> { new FilterModel("HierLvl3", FilterBuilderEnum.EQUALS, filter.HierLvl3) });
            }
        }

        private static void BuildFidcodeFilter(List<List<FilterModel>> ret, ProjectListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.Fidcode))
            {
                ret.Add(new List<FilterModel> { new FilterModel("Fidcode", FilterBuilderEnum.EQUALS, filter.Fidcode) });
            }
        }

        private static void BuildFidcodeLikeFilter(List<List<FilterModel>> ret, ProjectListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.FidcodeLike))
            {
                ret.Add(new List<FilterModel> { new FilterModel("Fidcode", FilterBuilderEnum.LIKE, filter.FidcodeLike) });
            }
        }

        public async Task<List<TxProjectMilestone>> GetMilestone(string projectId, int projectVersion)
        {
            var item = await _dbContext.TxProjectMilestones.Where(n => n.ProjectId == projectId && n.ProjectVersion == projectVersion).Include(n => n.WorkflowSequence).OrderBy(n => n.WorkflowSequence.Order)
                        .ToListAsync();
            return item;
        }

        public async Task<GetScopeOfWorkDto?> GetScopeOfWork(string projectId, int projectVersion)
        {
            var projectHeader = await _dbContext.TxProjectHeaders.SingleOrDefaultAsync(n => n.ProjectId == projectId && n.ProjectVersion == projectVersion
                                && n.Deleted == false);
            var platform = await _dbContext.TxProjectPlatforms.Where(n => n.ProjectId == projectId && n.ProjectVersion == projectVersion).ToListAsync();
            var pipeline = await _dbContext.TxProjectPipelines.Where(n => n.ProjectId == projectId && n.ProjectVersion == projectVersion).ToListAsync();
            var compressor = await _dbContext.TxProjectCompressors.Where(n => n.ProjectId == projectId && n.ProjectVersion == projectVersion).ToListAsync();
            var equipment = await _dbContext.TxProjectEquipments.Where(n => n.ProjectId == projectId && n.ProjectVersion == projectVersion).ToListAsync();

            if (projectHeader == null) return null;
            GetScopeOfWorkDto res = new GetScopeOfWorkDto(){
                WellDrillProducerCount = projectHeader.WellDrillProducerCount,
                WellDrillInjectorCount = projectHeader.WellDrillInjectorCount,
                WellWorkOverProducerCount = projectHeader.WellWorkOverProducerCount,
                WellWorkOverInjectorCount = projectHeader.WellWorkOverInjectorCount,
                Platform = _mapper.Map<List<ProjectPlatformDto>>(platform),
                Pipeline = _mapper.Map<List<ProjectPipelineDto>>(pipeline),
                Compressor = _mapper.Map<List<ProjectCompressorDto>>(compressor),
                Equipment = _mapper.Map<List<ProjectEquipmentDto>>(equipment)
            };
            return res;
        }

        public async Task<List<TxProjectHeader>> GetAllExceptDraftProject()
        {
            return await _dbContext.TxProjectHeaders.AsNoTracking().Where(n => (n.Status == "In-Progress" || n.Status == "Completed") && n.IsActive == true && n.Deleted == false).ToListAsync();  
        }

        public async Task<RegionalZonaThresholdData?> GetThresholdRegionalZonaByProjectId(string projectId)
        {
            var res = await (from p in _dbContext.TxProjectHeaders
                             join upshier in (from ups in _dbContext.TxProjectUpstreamEntities
                                              join hier in _dbContext.VwShuhier03s on ups.EntityId equals hier.EntityId
                                              select new
                                              {
                                                  ups.ProjectId,
                                                  hier.HierLvl2Desc,
                                                  hier.HierLvl3Desc
                                              }).Distinct()
                             on p.ProjectId equals upshier.ProjectId
                             where p.ProjectId == projectId
                             join tmp in _dbContext.RefTemplates on new {templateId = p.TemplateId ,templateVersion =  p.TemplateVersion!.Value} equals 
                             new {templateId = tmp.TemplateId , templateVersion = tmp.TemplateVersion}
                             select new RegionalZonaThresholdData
                             {
                                 ThresholdNameParId = tmp.ThresholdNameParId,
                                 HierLvl2Desc = upshier.HierLvl2Desc,
                                 HierLvl3Desc = upshier.HierLvl3Desc
                             }).Distinct().SingleOrDefaultAsync();
            return res;
        }
    }
}
