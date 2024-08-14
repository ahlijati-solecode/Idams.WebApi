using Idams.Core.Enums;
using Idams.Core.Model.Entities;
using Idams.Core.Model.Entities.Custom;
using Idams.Core.Model.Filters;
using Idams.Core.Repositories;
using Idams.Core.Services;
using Idams.Infrastructure.EntityFramework.Core;
using Idams.Infrastructure.EntityFramework.Queries;
using Microsoft.Extensions.Configuration;

namespace Idams.Infrastructure.EntityFramework.Repositories
{
    public class EFOutstandingTaskListRepository : BaseRepository, IOutstandingTaskListRepository
    {
        public EFOutstandingTaskListRepository(IdamsDbContext dbContext, IConfiguration configuration, ICurrentUserService currentUserService) : base(configuration, currentUserService, dbContext)
        {
        }

        public async Task<Paged<OutstandingTaskListPaged>> GetPaged(OutstandingTaskListFilter filter, int page, int size, string sort)
        {
            return await GetPaged<OutstandingTaskListPaged>(OutstandingTaskListQuery.SelectPagedQuery, OutstandingTaskListQuery.CountQuery, BuildOutstandingTaskListQuery(filter), page, size, sort);
        }

        private List<List<FilterModel>> BuildOutstandingTaskListQuery(OutstandingTaskListFilter filter)
        {
            List<List<FilterModel>> ret = new List<List<FilterModel>>();

            var roles = GetCurrentUserInfo.Roles;

            ret.Add(roles.Select(e => new FilterModel("WorkflowActor", FilterBuilderEnum.LIKE, e.Key)).ToList());
            ret.Add(new List<FilterModel> { new FilterModel("Status", FilterBuilderEnum.NOT_EQUALS, "Draft") });
            ret.Add(new List<FilterModel> { new FilterModel("Status", FilterBuilderEnum.NOT_EQUALS, "Completed") });

            BuildNotDeleted(ret);
            BuildIsActive(ret);
            BuildProjectNameFilter(ret, filter);
            BuildHierLvl2Filter(ret, filter);
            BuildHierLvl2DropdownFilter(ret, filter);
            BuildHierLvl3Filter(ret, filter);
            BuildThresholdFitler(ret, filter);
            BuildThresholdDropdownFitler(ret, filter);
            BuildStageFitler(ret, filter);
            BuildStageDropdownFilter(ret, filter);
            BuildWorkflowNameFitler(ret, filter);
            BuildWorkflowActionNameFitler(ret, filter);
            BuildUpdatedDateFitler(ret, filter);
            BuildWorkflowTypeFitler(ret, filter);
            BuildUserHierLvl2Filter(ret, filter);
            BuildUserHierLvl3Filter(ret, filter);

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

        private static void BuildWorkflowTypeFitler(List<List<FilterModel>> ret, OutstandingTaskListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.WorkflowType))
            {
                ret.Add(new List<FilterModel> { new FilterModel("WorkflowType", FilterBuilderEnum.LIKE, filter.WorkflowType) });
            }
        }

        private static void BuildUpdatedDateFitler(List<List<FilterModel>> ret, OutstandingTaskListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.UpdatedDate))
            {
                ret.Add(new List<FilterModel> { new FilterModel("convert(varchar(25), UpdatedDate, 113)", FilterBuilderEnum.LIKE, filter.UpdatedDate) });
            }
        }

        private static void BuildWorkflowActionNameFitler(List<List<FilterModel>> ret, OutstandingTaskListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.WorkflowActionName))
            {
                ret.Add(new List<FilterModel> { new FilterModel("WorkflowActionName", FilterBuilderEnum.LIKE, filter.WorkflowActionName) });
            }
        }

        private static void BuildWorkflowNameFitler(List<List<FilterModel>> ret, OutstandingTaskListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.WorkflowName))
            {
                ret.Add(new List<FilterModel> { new FilterModel("WorkflowName", FilterBuilderEnum.LIKE, filter.WorkflowName) });
            }
        }

        private static void BuildStageFitler(List<List<FilterModel>> ret, OutstandingTaskListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.Stage))
            {
                ret.Add(new List<FilterModel> { new FilterModel("Stage", FilterBuilderEnum.LIKE, filter.Stage) });
            }
        }

        private static void BuildStageDropdownFilter(List<List<FilterModel>> ret, OutstandingTaskListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.StageDropdown))
            {
                ret.Add(new List<FilterModel> { new FilterModel("Stage", FilterBuilderEnum.LIKE, filter.StageDropdown) });
            }
        }


        private static void BuildThresholdFitler(List<List<FilterModel>> ret, OutstandingTaskListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.Threshold))
            {
                ret.Add(new List<FilterModel> { new FilterModel("Threshold", FilterBuilderEnum.LIKE, filter.Threshold) });
            }
        }

        private static void BuildThresholdDropdownFitler(List<List<FilterModel>> ret, OutstandingTaskListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.ThresholdDropDown))
            {
                ret.Add(new List<FilterModel> { new FilterModel("Threshold", FilterBuilderEnum.LIKE, filter.ThresholdDropDown) });
            }
        }

        private static void BuildHierLvl3Filter(List<List<FilterModel>> ret, OutstandingTaskListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.HierLvl3Desc))
            {
                ret.Add(new List<FilterModel> { new FilterModel("HierLvl3Desc", FilterBuilderEnum.LIKE, filter.HierLvl3Desc) });
            }
        }

        private static void BuildHierLvl2Filter(List<List<FilterModel>> ret, OutstandingTaskListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.HierLvl2Desc))
            {
                ret.Add(new List<FilterModel> { new FilterModel("HierLvl2Desc", FilterBuilderEnum.LIKE, filter.HierLvl2Desc) });
            }
        }

        private static void BuildHierLvl2DropdownFilter(List<List<FilterModel>> ret, OutstandingTaskListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.HierLvl2DropDown))
            {
                ret.Add(new List<FilterModel> { new FilterModel("HierLvl2Desc", FilterBuilderEnum.LIKE, filter.HierLvl2DropDown) });
            }
        }

        private static void BuildProjectNameFilter(List<List<FilterModel>> ret, OutstandingTaskListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.ProjectName))
            {
                ret.Add(new List<FilterModel> { new FilterModel("ProjectName", FilterBuilderEnum.LIKE, filter.ProjectName) });
            }
        }

        private static void BuildUserHierLvl2Filter(List<List<FilterModel>> ret, OutstandingTaskListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.HierLvl2))
            {
                ret.Add(new List<FilterModel> { new FilterModel("HierLvl2", FilterBuilderEnum.EQUALS, filter.HierLvl2) });
            }
        }

        private static void BuildUserHierLvl3Filter(List<List<FilterModel>> ret, OutstandingTaskListFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.HierLvl3))
            {
                ret.Add(new List<FilterModel> { new FilterModel("HierLvl3", FilterBuilderEnum.EQUALS, filter.HierLvl3) });
            }
        }
    }
}

