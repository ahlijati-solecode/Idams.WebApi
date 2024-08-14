using Idams.Core.Constants;
using Idams.Core.Enums;
using Idams.Core.Extenstions;
using Idams.Core.Model.Dtos;
using Idams.Core.Model.Entities;
using Idams.Core.Model.Entities.Custom;
using Idams.Core.Model.Filters;
using Idams.Core.Model.Requests;
using Idams.Core.Repositories;
using Idams.Infrastructure.EntityFramework.Core;
using Idams.Infrastructure.EntityFramework.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Idams.Infrastructure.EntityFramework.Repositories
{
    public class EFWorkflowSettingRepository : EFPagedBaseRepository<RefTemplate, WorkflowSettingFilter, WorkflowSettingPaged, List<RoleEnum>>, IWorkflowSettingRepository
    {
        protected readonly HttpContext _httpContext;
        protected readonly IdamsDbContext _dbContext;

        public EFWorkflowSettingRepository(IServiceProvider serviceProvider, IdamsDbContext dbContext) : base(serviceProvider)
        {
            _dbContext = dbContext;
        }

        public override string SelectPagedQuery => WorkflowSettingPagedQuery.SelectPagedQuery;

        public override string CountQuery => WorkflowSettingPagedQuery.CountQuery;

        protected override void BuildQuery(List<FilterBuilderModel> param, List<FilterBuilderModel> paramRole,  WorkflowSettingFilter filter, List<RoleEnum> roles)
        {
            List<List<FilterBuilderModel>> result = new List<List<FilterBuilderModel>>();
            foreach (var role in roles)
            {
                if (role == RoleEnum.SUPER_ADMIN)
                {
                    BuildFilterThreshold(param, filter);
                }
                else if (role == RoleEnum.ADMIN_REGIONAL)
                {
                    BuildFilterThreshold(param, filter);
                    BuildFilterThresholdRegional(paramRole, filter);
                }
                else if (role == RoleEnum.ADMIN_SHU)
                {

                    BuildFilterThreshold(param, filter);
                    BuildFilterThresholdShu(paramRole, filter);
                    BuildFilterThresholdHolding(paramRole, filter);
                }
                else
                {
                    continue;
                }
            }
            
            BuildFilterTemplateName(param, filter);
            BuildFilterProjectCategory(param, filter);
            BuildFilterProjectCriteria(param, filter);
            BuildFilterProjectSubCriteria(param, filter);
            BuildFilterCapexValue(param, filter);
            BuildFilterTotalWorkflow(param, filter);
            BuildFilterStartlWorkflow(param, filter);
            BuildFilterEndWorkflow(param, filter);
            BuildFilterStatus(param, filter);
        }
        private static void BuildFilterStatus(List<FilterBuilderModel> param, WorkflowSettingFilter filter)
        {
            param.Add(new FilterBuilderModel("IsActive", FilterBuilderEnum.EQUALS, $"'{true}'"));
        }
        private static void BuildFilterTemplateName(List<FilterBuilderModel> param, WorkflowSettingFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.TemplateName))
            {
                param.Add(new FilterBuilderModel("TemplateName", FilterBuilderEnum.LIKE, $"'%{filter.TemplateName}%'"));
            }
        }

        private static void BuildFilterProjectCategory(List<FilterBuilderModel> param, WorkflowSettingFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.ProjectCategory))
            {
                param.Add(new FilterBuilderModel("ProjectCategory", FilterBuilderEnum.LIKE, $"'%{filter.ProjectCategory}%'"));
            }
        }

        private static void BuildFilterProjectCriteria(List<FilterBuilderModel> param, WorkflowSettingFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.ProjectCriteria))
            {
                param.Add(new FilterBuilderModel("ProjectCriteria", FilterBuilderEnum.LIKE, $"'%{filter.ProjectCriteria}%'"));
            }
        }
        private static void BuildFilterProjectSubCriteria(List<FilterBuilderModel> param, WorkflowSettingFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.ProjectSubCriteria))
            {
                param.Add(new FilterBuilderModel("ProjectSubCriteria", FilterBuilderEnum.LIKE, $"'%{filter.ProjectSubCriteria}%'"));
            }
        }
        private static void BuildFilterThreshold(List<FilterBuilderModel> param, WorkflowSettingFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.Threshold))
            {
                param.Add(new FilterBuilderModel("Threshold", FilterBuilderEnum.LIKE, $"'%{filter.Threshold}%'"));
            }
        }

        private static void BuildFilterThresholdRegional(List<FilterBuilderModel> param, WorkflowSettingFilter filter)
        {
            filter.Threshold = "Regional";
            param.Add(new FilterBuilderModel("Threshold", FilterBuilderEnum.LIKE, $"'%{filter.Threshold}%'"));
        }

        private static void BuildFilterThresholdShu(List<FilterBuilderModel> param, WorkflowSettingFilter filter)
        {
            filter.Threshold = "SHU";
            param.Add(new FilterBuilderModel("Threshold", FilterBuilderEnum.EQUALS, $"'{filter.Threshold}'"));
            filter.Threshold = "";
        }
        private static void BuildFilterThresholdHolding(List<FilterBuilderModel> param, WorkflowSettingFilter filter)
        {
            filter.Threshold = "Holding";
            param.Add(new FilterBuilderModel("Threshold", FilterBuilderEnum.EQUALS, $"'{filter.Threshold}'"));
            filter.Threshold = "";
        }

        private static void BuildFilterCapexValue(List<FilterBuilderModel> param, WorkflowSettingFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.CapexValue))
            {
                param.Add(new FilterBuilderModel("CapexValue", FilterBuilderEnum.LIKE, $"'%{filter.CapexValue}%'"));
            }
        }
        private static void BuildFilterTotalWorkflow(List<FilterBuilderModel> param, WorkflowSettingFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.TotalWorkflow))
            {
                param.Add(new FilterBuilderModel("TotalWorkflow", FilterBuilderEnum.LIKE, $"'%{filter.TotalWorkflow}%'"));
            }
        }
        private static void BuildFilterStartlWorkflow(List<FilterBuilderModel> param, WorkflowSettingFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.StartWorkFlow))
            {
                param.Add(new FilterBuilderModel("StartWorkflow", FilterBuilderEnum.LIKE, $"'%{filter.StartWorkFlow}%'"));
            }
        }
        private static void BuildFilterEndWorkflow(List<FilterBuilderModel> param, WorkflowSettingFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.EndWorkFlow))
            {
                param.Add(new FilterBuilderModel("EndWorkflow", FilterBuilderEnum.LIKE, $"'%{filter.EndWorkFlow}%'"));
            }
        }

        public async Task<RefTemplate> GetByTemplateIdAndTemplateVersion(string? templateId, int? templateVersion)
        {
            var template = await GetAll().AsNoTracking().Where(n => n.TemplateId == templateId && n.TemplateVersion == templateVersion)
                .Include(n => n.RefTemplateWorkflowSequences).SingleOrDefaultAsync();
            return template;
        }

        public async Task<TemplateWithThresholdNameDto?> GetTemplateWithThesholdName(string? templateId, int? templateVersion)
        {
            var paramList = (from paramIdList in _dbContext.MdParamaterLists
                             where paramIdList.Schema == "idams" && paramIdList.ParamId == "ThresholdName"
                             select paramIdList).AsNoTracking();

            var template = await (from t in _dbContext.RefTemplates
                            join p in paramList on t.ThresholdNameParId equals p.ParamListId
                            where t.TemplateId == templateId && t.TemplateVersion == templateVersion
                            select new TemplateWithThresholdNameDto()
                            {
                                ProjectCategory = t.ProjectCategoryParId,
                                ProjectCriteria = t.ProjectCriteriaParId,
                                ProjectSubCriteria = t.ProjectSubCriteriaParId,
                                TemplateName = t.TemplateName,
                                Threshold = p.ParamValue1Text
                            }).FirstOrDefaultAsync();
            return template;
        }

        public async Task<RefTemplate?> GetByTemplateIdAndTemplateVersionWithTemplateDocument(string? templateId, int? templateVersion)
        {
            var template = await GetAll().AsNoTracking().Where(n => n.TemplateId == templateId && n.TemplateVersion == templateVersion && n.Deleted == false)
                .Include(n => n.RefTemplateWorkflowSequences).ThenInclude(n => n.RefTemplateDocuments).SingleOrDefaultAsync();
            return template;
        }

        public Task<RefTemplate> UpdateRefTemplateAsync(RefTemplate template, RefTemplate model)
        {
            model.CopyProperties(template);
            template.UpdatedDate = DateTime.UtcNow;
            template.UpdatedBy = GetCurrentUser;
            _context.Entry(template).State = EntityState.Modified;
            _context.Set<RefTemplate>().Update(template);
            return Task.FromResult(template);
        }

        public Task<GetWorkflowSettingDropdown> GetDropdownList()
        {
            GetWorkflowSettingDropdown getWorkflowSettingDropdown = new GetWorkflowSettingDropdown();

            string[] clause = new string[] { "ProjectCategory", "ProjectCriteria", "SubProjectCriteria", "ThresholdName", "ThresholdOperator"};
            var res = _dbContext.MdParamaterLists.AsNoTracking()
                .Where(n => n.Schema == "idams" && clause.Contains(n.ParamId))
                .Select(n => new { clause = n.ParamId, key = n.ParamListId, value = n.ParamValue1Text })
                .ToList();

            res.ForEach(n =>
            {
                Dictionary<string, string?> data = new Dictionary<string, string?>();
                data.Add("key", n.key);
                data.Add("value", n.value);
                if (n.clause == "ProjectCategory")
                {
                    getWorkflowSettingDropdown.ProjectCategory.Add(data);
                }
                else if (n.clause == "ProjectCriteria")
                {
                    getWorkflowSettingDropdown.ProjectCriteria.Add(data);
                }
                else if (n.clause == "SubProjectCriteria")
                {
                    getWorkflowSettingDropdown.ProjectSubCriteria.Add(data);
                }
                else if (n.clause == "ThresholdName")
                {
                    var threshold = n.key;
                    var thresholdData = _dbContext.RefThresholdProjects
                                    .Where(n => n.ThresholdNameParId == threshold)
                                    .SingleOrDefault();
                    var mathOps = res.Where(n => n.key == thresholdData.MathOperatorParId).SingleOrDefault();

                    data.Add("capex1", thresholdData.Capex1.ToString());
                    data.Add("mathOps", mathOps.value.ElementAtOrDefault(0).ToString());
                    data.Add("capex2", thresholdData.Capex2 == null ? "" : thresholdData.Capex2.ToString());
                    getWorkflowSettingDropdown.Threshold.Add(data);
                }
            });
            return Task.FromResult(getWorkflowSettingDropdown);
        }

        public async Task<int> GetLatestTemplateVersion()
        {
            var result = await GetAll().AsNoTracking().Where(n => n.Deleted == false).FirstOrDefaultAsync();
            return result.TemplateVersion;
        }

        public async Task<int> GetThresholdVersion(string threshold)
        {
            var result =  await _dbContext.RefThresholdProjects.AsNoTracking().Where(n => n.ThresholdNameParId == threshold).SingleOrDefaultAsync();
            return result.ThresholdVersion;
        }

        public async Task<string?> GetLastTemplateId()
        {
            var result = await GetAll().AsNoTracking().OrderBy(n => n.TemplateId).LastOrDefaultAsync();
            return result?.TemplateId;
        }

        public async Task<RefTemplate?> DetermineTemplateByMultipleCategory(ProjectTemplateFilter projectTemplateFilter)
        {
            var query = _context.RefTemplates.AsQueryable().AsNoTracking();
            query = query.Where(n => n.ThresholdNameParId == projectTemplateFilter.Threshold &&
                            n.ProjectCategoryParId == projectTemplateFilter.ProjectCategory &&
                            n.ProjectCriteriaParId == projectTemplateFilter.ProjectCriteria &&
                            n.Status == StatusConstant.Published &&
                            n.Deleted == false &&
                            n.IsActive == true);
            if (!string.IsNullOrWhiteSpace(projectTemplateFilter.ProjectSubCriteria))
            {
                query = query.Where(n => n.ProjectSubCriteriaParId == projectTemplateFilter.ProjectSubCriteria);
            }

            var ret = await query.FirstOrDefaultAsync();
            return ret;
        }

        public async Task<GetScopeOfWorkDropdown> GetScopeOfWorkDropdownList()
        {
            GetScopeOfWorkDropdown res = new GetScopeOfWorkDropdown();

            string[] clause = { "ProjectFieldService", "ProjectCompressorType" };
            var item = await _dbContext.MdParamaterLists.AsNoTracking()
                .Where(n => n.Schema == "idams" && clause.Contains(n.ParamId))
                .Select(n => new { clause = n.ParamId, key = n.ParamListId, value = n.ParamValue1Text, value2 = n.ParamValue2Text })
                .ToListAsync();

            item.ForEach(n =>
            {
                Dictionary<string, string> data = new Dictionary<string, string>();
                if(n.clause == "ProjectFieldService")
                {
                    data.Add("key", n.key);
                    data.Add("value", n.value);
                    res.FieldService.Add(data);
                }
                else
                {
                    data.Add("key", n.key);
                    data.Add("value", n.value);
                    data.Add("unit", n.value2);
                    res.CompressorType.Add(data);
                }
            });
            return res;
        }

        public async Task<RefTemplate> AddTemplateAsync(RefTemplate template)
        {
            template.CreatedBy = GetCurrentUser;
            template.CreatedDate = DateTime.UtcNow;
            _dbContext.RefTemplates.Add(template);
            return template;
        }

        public async Task<bool> DeleteRefTemplate(RefTemplate template)
        {
            if (template == null) return false;
            _dbContext.RefTemplates.Remove(template);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CheckThresholdCategoryCriteriaSubCriteria(string? threshold, string? category, string? criteria, string? subcriteria)
        {
            var res = await _dbContext.RefTemplates.AsNoTracking().FirstOrDefaultAsync(n => n.ThresholdNameParId == threshold && n.ProjectCategoryParId == category
                       && n.ProjectCriteriaParId == criteria && n.ProjectSubCriteriaParId == subcriteria && n.Deleted == false && n.IsActive == true);
            return res != null;
        }
    }
}
