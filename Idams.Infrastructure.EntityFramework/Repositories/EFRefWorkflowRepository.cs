using Idams.Core.Constants;
using Idams.Core.Enums;
using Idams.Core.Model.Dtos;
using Idams.Core.Model.Entities;
using Idams.Core.Model.Responses;
using Idams.Core.Repositories;
using Idams.Infrastructure.EntityFramework.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Infrastructure.EntityFramework.Repositories
{
    public class EFRefWorkflowRepository : EFRepository<RefWorkflow, string, List<RoleEnum>>, IRefWorkflowRepository
    {
        protected readonly IdamsDbContext _dbContext;
        public EFRefWorkflowRepository(IdamsDbContext context, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _dbContext = context;// serviceProvider.GetService<IdamsDbContext>() ?? throw new ArgumentNullException(nameof(IdamsDbContext));
        }
        public Task<List<RefWorkflowDto>> GetList(string? CategoryParId = null)
        {
            try
            {
                var query = _dbContext.RefWorkflows.AsQueryable();
                if (CategoryParId != null)
                {
                    query = query.Where(x => x.WorkflowCategoryParId == CategoryParId);
                }
                return query.AsNoTracking().Select(x => new RefWorkflowDto()
                {
                    key = x.WorkflowId,
                    value = x.WorkflowType
                }).ToListAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<List<RefWorkflow>> GetFirstRefWorkflow()
        {
            var result = GetAll().AsNoTracking().Where(n => n.WorkflowCategoryParId == "Inisiasi").OrderBy(n => n.WorkflowId).ToList();
            return Task.FromResult(result);
        }

        public async Task<RefTemplateWorkflowSequence> SaveWorkflowSequence(RefTemplateWorkflowSequence data)
        {
            try
            {
                using var transaction = _dbContext.Database.BeginTransaction();
                var order = _dbContext.RefTemplateWorkflowSequences.AsNoTracking()
                    .Where(x => x.TemplateId == data.TemplateId && x.TemplateVersion == data.TemplateVersion).Max(x => x.Order);
                data.Order = order != null ? order++ : 1;
                _dbContext.RefTemplateWorkflowSequences.Add(data);
                await _dbContext.SaveChangesAsync();

                var template = _dbContext.RefTemplates.AsNoTracking().FirstOrDefault(x => x.TemplateId == data.TemplateId);

                var workflow = _dbContext.RefWorkflows.AsNoTracking().Include(x => x.RefWorkflowActions)
                    .Where(x => x.WorkflowId == data.WorkflowId).FirstOrDefault();
                if(workflow != null)
                {
                    foreach(var workflowAction in workflow.RefWorkflowActions.Where(x => x.RequiredDocumentGroupParId != null))
                    {
                        var docGroup = _dbContext.MdDocumentGroups.FirstOrDefault(x => x.DocGroupParId == workflowAction.RequiredDocumentGroupParId);
                        _dbContext.RefTemplateDocuments.Add(new RefTemplateDocument()
                        {
                            TemplateId = data.TemplateId ?? "",
                            TemplateVersion = data.TemplateVersion ?? 0,
                            ThresholdNameParId = template?.ThresholdNameParId ?? "",
                            ThresholdVersion = template?.ThresholdVersion ?? 0,
                            WorkflowSequenceId = data.WorkflowSequenceId,
                            WorkflowActionId = workflowAction.WorkflowActionId,
                            DocGroupParId = docGroup?.DocGroupParId,
                            DocGroupVersion = docGroup?.DocGroupVersion,
                        });
                    }
                }
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return data;
            }
            catch (Exception ex)
            {
                throw ;
            }
        }
        public async Task<TemplateWorkflowSeqDto?> GetWorkflowSequence(string workflowSequenceId)
        {
            var data = await _dbContext.RefTemplateWorkflowSequences.AsNoTracking()
                .Include(x => x.RefTemplateDocuments)
                .Where(x => x.WorkflowSequenceId == workflowSequenceId).Select(x => new TemplateWorkflowSeqDto()
                {
                    WorkflowSequenceId = x.WorkflowSequenceId,
                    TemplateId = x.TemplateId,
                    WorkflowType = x.WorkflowId,
                    WorkflowName = x.WorkflowName,
                    Sla = x.Sla,
                    isOptional = x.WorkflowIsOptional,
                    documentGroup = x.RefTemplateDocuments.Select(y => new WorkflowSeqDocGroupDto()
                    {
                        DocGroupParId = y.DocGroupParId ?? "",
                        DocVersion = y.DocGroupVersion ?? 0,
                        DocGroup = _dbContext.MdParamaterLists
                        .Where(p => p.ParamListId == y.DocGroupParId && p.ParamId == "DocGroup" && p.Schema == "idams")
                        .Select(p => p.ParamValue1Text).FirstOrDefault() ?? ""
                    }).ToList()
                }).FirstOrDefaultAsync();
            return data;
        }

        public Task<RefWorkflow> GetByWorkflowId(string workflowId)
        {
            var result = GetAll().AsNoTracking().Where(n => n.WorkflowId == workflowId).SingleOrDefault();
            return Task.FromResult(result);
        }

        public async Task<List<RefWorkflowAction>> GetListWorkflowAction(string workflowId)
        {
            return await _dbContext.RefWorkflowActions.AsNoTracking()
                .Where(n => n.WorkflowId == workflowId)
                .ToListAsync();
        }

        public async Task<List<RefWorkflowAction>> GetListWorkflowActionWithCard(string workflowId, bool? isOptional)
        {
            if (isOptional == true)
            {
                return await _dbContext.RefWorkflowActions.AsNoTracking()
                    .Where(n => n.WorkflowId == workflowId && n.WorkflowActionTypeParId != null)
                    .Include(n => n.RefWorkflowActors)
                    .ToListAsync();
            }
            return await _dbContext.RefWorkflowActions.AsNoTracking()
                .Where(n => n.WorkflowId == workflowId && n.WorkflowActionTypeParId != null && n.WorkflowActionTypeParId != WorkflowActionTypeConstant.Confirmation)
                .Include(n => n.RefWorkflowActors)
                .ToListAsync();
        }

        public async Task<List<RefWorkflowAction>> GetListWorkflowAction(string workflowId, bool? isOptional)
        {
            if (isOptional == true)
            {
                return await _dbContext.RefWorkflowActions.AsNoTracking()
                    .Where(n => n.WorkflowId == workflowId)
                    .Include(n => n.RefWorkflowActors)
                    .ToListAsync();
            }
            return await _dbContext.RefWorkflowActions.AsNoTracking()
                .Where(n => n.WorkflowId == workflowId && n.WorkflowActionTypeParId != WorkflowActionTypeConstant.Confirmation)
                .Include(n => n.RefWorkflowActors)
                .ToListAsync();
        }

        public async Task<RefWorkflow?> GetByWorkflowSequenceId(string workflowSequenceId)
        {
            var res = await _dbContext.RefTemplateWorkflowSequences.Where(n => n.WorkflowSequenceId == workflowSequenceId)
                .Include(n => n.Workflow).FirstOrDefaultAsync();

            return res?.Workflow;
        }

        public async Task<List<RefWorkflow>> GetWorkflowByStage(string stage)
        {
            var res = await _dbContext.RefWorkflows.AsNoTracking().Where(n => n.WorkflowCategoryParId == stage).ToListAsync();
            return res;
        }
    }
}
