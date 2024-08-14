using Idams.Core.Enums;
using Idams.Core.Model.Entities;
using Idams.Core.Model.Entities.Custom;
using Idams.Core.Model.Filters;
using Idams.Core.Repositories;
using Idams.Core.Services;
using Idams.Infrastructure.EntityFramework.Core;
using Idams.Infrastructure.EntityFramework.Queries;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Infrastructure.EntityFramework.Repositories
{
    public class EFLgProjectActivityAuditTrailRepository : EFPagedBaseRepository<LgProjectActivityAuditTrail, LogHistoryFilter, LogHistoryPaged, List<RoleEnum>>, ILgProjectActivityAuditTrailRepository
    {
        protected readonly IdamsDbContext _dbContext;
        protected readonly ICurrentUserService _currentUserService;
        public EFLgProjectActivityAuditTrailRepository(IServiceProvider serviceProvider, IdamsDbContext dbContext, ICurrentUserService currentUserService) : base(serviceProvider)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        public override string SelectPagedQuery => LogHistoryQuery.SelectPagedQuery;

        public override string CountQuery => LogHistoryQuery.CountQuery;

        protected override void BuildQuery(List<FilterBuilderModel> param, List<FilterBuilderModel> paramRole, LogHistoryFilter filter, List<RoleEnum> role)
        {
            BuildFilterProjectId(param, filter);
            BuildFilterEmpName(param, filter);
            BuildFilterWorkflowName(param, filter);
            BuildFilterAction(param, filter);
            BuildFilterActivityDescription(param, filter);
            BuildFilterLastStatus(param, filter);
        }

        private static void BuildFilterProjectId(List<FilterBuilderModel> param, LogHistoryFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.ProjectId))
            {
                param.Add(new FilterBuilderModel("ProjectId", FilterBuilderEnum.LIKE, $"'%{filter.ProjectId}%'"));
            }
        }

        private static void BuildFilterEmpName(List<FilterBuilderModel> param, LogHistoryFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.EmpName))
            {
                param.Add(new FilterBuilderModel("EmpName", FilterBuilderEnum.LIKE, $"'%{filter.EmpName}%'"));
            }
        }
        private static void BuildFilterWorkflowName(List<FilterBuilderModel> param, LogHistoryFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.WorkflowName))
            {
                param.Add(new FilterBuilderModel("WorkflowName", FilterBuilderEnum.LIKE, $"'%{filter.WorkflowName}%'"));
            }
        }
        private static void BuildFilterAction(List<FilterBuilderModel> param, LogHistoryFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.Action))
            {
                param.Add(new FilterBuilderModel("Action", FilterBuilderEnum.LIKE, $"'%{filter.Action}%'"));
            }
        }
        private static void BuildFilterActivityDescription(List<FilterBuilderModel> param, LogHistoryFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.ActivityDescription))
            {
                param.Add(new FilterBuilderModel("ActivityDescription", FilterBuilderEnum.LIKE, $"'%{filter.ActivityDescription}%'"));
            }
        }
        private static void BuildFilterLastStatus(List<FilterBuilderModel> param, LogHistoryFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.LastStatus))
            {
                param.Add(new FilterBuilderModel("ParamValue1Text", FilterBuilderEnum.LIKE, $"'%{filter.LastStatus}%'"));
            }
        }

        public async Task<LgProjectActivityAuditTrail> AddLog(LgProjectActivityAuditTrail entity)
        {
            entity.CreatedDate = DateTime.UtcNow;
            entity.EmpAccount = _currentUserService.CurrentUserInfo.Email;
            entity.EmpName = _currentUserService.CurrentUserInfo.Name;
            await _dbContext.LgProjectActivityAuditTrails.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<List<LgProjectActivityAuditTrail>> GetLogByActivityDescription(string projectId, string activityDescription)
        {
            return await _dbContext.LgProjectActivityAuditTrails.AsNoTracking().Where(n => n.ProjectId == projectId && n.ActivityDescription == activityDescription).OrderBy(n => n.CreatedDate).ToListAsync();
        }
    }
}
