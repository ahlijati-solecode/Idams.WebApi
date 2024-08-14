using Idams.Core.Constants;
using Idams.Core.Enums;
using Idams.Core.Model.Dtos;
using Idams.Core.Model.Entities;
using Idams.Core.Extenstions;
using Idams.Core.Repositories;
using Idams.Infrastructure.EntityFramework.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Idams.Core.Model;

namespace Idams.Infrastructure.EntityFramework.Repositories
{
    public class EFRefTemplateWorkflowSequenceRepository : EFRepository<RefTemplateWorkflowSequence, string, List<RoleEnum>>, IRefTemplateWorkflowSequenceRepository
    {
        private readonly ILogger<EFRefTemplateWorkflowSequenceRepository> _logger;
        public EFRefTemplateWorkflowSequenceRepository(IServiceProvider serviceProvider, ILogger<EFRefTemplateWorkflowSequenceRepository> logger) : base(serviceProvider)
        {
            _logger = logger;
        }

        public async Task<RefTemplateWorkflowSequence> GetByWorkflowSequenceId(string workflowSequenceId)
        {
            var result = await _context.RefTemplateWorkflowSequences.AsNoTracking().Where(n => n.WorkflowSequenceId == workflowSequenceId).SingleOrDefaultAsync();
            return result;
        }

        public async Task<string> GetLastWorkflowSequnceId()
        {
            var result = _context.RefTemplateWorkflowSequences.AsQueryable().AsNoTracking().OrderBy(n => n.WorkflowSequenceId).LastOrDefault();
            return result.WorkflowSequenceId;
        }

        public Task<RefTemplateWorkflowSequence> UpdateRefTemplateWorkflowSequenceAsync(RefTemplateWorkflowSequence templateSequence, RefTemplateWorkflowSequence model)
        {
            model.CopyProperties(templateSequence);
            templateSequence.UpdatedDate = DateTime.UtcNow;
            templateSequence.UpdatedBy = GetCurrentUser;
            _context.Entry(templateSequence).State = EntityState.Modified;
            _context.Set<RefTemplateWorkflowSequence>().Update(templateSequence);
            return Task.FromResult(templateSequence);
        }

        private async Task<string> GetNextWorkflowSequenceId()
        {
            var latest = await _context.RefTemplateWorkflowSequences.AsQueryable().AsNoTracking().OrderByDescending(n => n.WorkflowSequenceId).FirstOrDefaultAsync();
            return IdGenerationUtil.GenerateNextId(latest?.WorkflowSequenceId, "WS");
        }

        public async Task<RefTemplateWorkflowSequence> AddNewWorkflowSequence(RefTemplateWorkflowSequence data)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var workflowSequences = await _context.RefTemplateWorkflowSequences.AsQueryable()
                    .Where(n => n.TemplateId == data.TemplateId && n.TemplateVersion == data.TemplateVersion && n.Deleted == false)
                    .Include(n => n.Workflow)
                    .OrderBy(n => n.Order)
                    .ToListAsync();
                var selectedWorkflow = await _context.RefWorkflows.AsQueryable()
                    .Include(n => n.RefWorkflowActions)
                    .Where(n => n.WorkflowId == data.WorkflowId)
                    .FirstAsync();
                data.WorkflowSequenceId = await GetNextWorkflowSequenceId();
                data.Workflow = selectedWorkflow;


                List<RefTemplateWorkflowSequence> temp = new();
                temp.AddRange(workflowSequences);
                temp.Add(data);
                var orderedSequence = ReOrderBasedOnStage(temp);

                await _context.SaveChangesAsync();

                _context.RefTemplateWorkflowSequences.Add(data);

                await _context.SaveChangesAsync();

                var template = await _context.RefTemplates
                    .FirstAsync(n => n.TemplateId == data.TemplateId && n.TemplateVersion == data.TemplateVersion);

                template.TotalWorkflow = temp.Count;
                template.EndWorkflow = orderedSequence.ElementAt(orderedSequence.Count - 1).Value.WorkflowName;
                await _context.SaveChangesAsync();

                foreach (var workflowAction in selectedWorkflow.RefWorkflowActions.Where(n => n.RequiredDocumentGroupParId != null))
                {
                    var group = await _context.MdDocumentGroups.AsQueryable().AsNoTracking()
                        .Where(n => n.DocGroupParId == workflowAction.RequiredDocumentGroupParId)
                        .OrderByDescending(n => n.DocGroupVersion)
                        .FirstOrDefaultAsync();

                    var newGroup = new MdDocumentGroup();
                    newGroup.DocGroupParId = workflowAction.RequiredDocumentGroupParId!;
                    int lastVer = group?.DocGroupVersion ?? 0;
                    newGroup.DocGroupVersion = lastVer+1;
                    newGroup.CreatedDate = DateTime.UtcNow;
                    newGroup.CreatedBy = GetCurrentUser;

                    _context.MdDocumentGroups.Add(newGroup);
                    await _context.SaveChangesAsync();

                    RefTemplateDocument refTemplateDocument = new RefTemplateDocument();
                    refTemplateDocument.TemplateId = data.TemplateId!;
                    refTemplateDocument.TemplateVersion = data.TemplateVersion!.Value;
                    refTemplateDocument.ThresholdNameParId = template.ThresholdNameParId!;
                    refTemplateDocument.ThresholdVersion = template.ThresholdVersion!.Value;
                    refTemplateDocument.WorkflowSequenceId = data.WorkflowSequenceId;
                    refTemplateDocument.WorkflowActionId = workflowAction.WorkflowActionId;
                    refTemplateDocument.DocGroupParId = newGroup.DocGroupParId;
                    refTemplateDocument.DocGroupVersion = newGroup.DocGroupVersion;
                    _context.RefTemplateDocuments.Add(refTemplateDocument);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return data;
            }
            catch(Exception ex)
            {
                transaction.Rollback();
                _logger.LogError(ex, $"Error Adding new workflow sequence, payload:{JsonConvert.SerializeObject(data)}");
                throw;
            }
        }

        private static List<KeyValuePair<int, RefTemplateWorkflowSequence>> ReOrderBasedOnStage(List<RefTemplateWorkflowSequence> inputs)
        {
            Dictionary<string, List<RefTemplateWorkflowSequence>> categorized = new();
            categorized[ParIdConstant.Inisiasi] = new List<RefTemplateWorkflowSequence>();
            categorized[ParIdConstant.Seleksi] = new List<RefTemplateWorkflowSequence>();
            categorized[ParIdConstant.KLanjut] = new List<RefTemplateWorkflowSequence>();
            foreach (var workflowSequence in inputs)
            {
                categorized[workflowSequence.Workflow!.WorkflowCategoryParId!].Add(workflowSequence);
            }
            int order = 1;
            List<KeyValuePair<int, RefTemplateWorkflowSequence>> orderedSequence = new();

            foreach (var workflowSequence in categorized[ParIdConstant.Inisiasi])
            {
                workflowSequence.Order = order;
                orderedSequence.Add(new KeyValuePair<int, RefTemplateWorkflowSequence>(order, workflowSequence));
                order++;
            }
            foreach (var workflowSequence in categorized[ParIdConstant.Seleksi])
            {
                workflowSequence.Order = order;
                orderedSequence.Add(new KeyValuePair<int, RefTemplateWorkflowSequence>(order, workflowSequence));
                order++;
            }
            foreach (var workflowSequence in categorized[ParIdConstant.KLanjut])
            {
                workflowSequence.Order = order;
                orderedSequence.Add(new KeyValuePair<int, RefTemplateWorkflowSequence>(order, workflowSequence));
                order++;
            }

            return orderedSequence;
        }

        public async Task<TemplateWorkflowSeqDto?> GetWorkflowSequence(string workflowSequenceId)
        {
            var data = await _context.RefTemplateWorkflowSequences.AsNoTracking()
                .Include(x => x.RefTemplateDocuments)
                .Include(x => x.Workflow)
                .Where(x => x.WorkflowSequenceId == workflowSequenceId).Select(x => new TemplateWorkflowSeqDto()
                {
                    WorkflowSequenceId = x.WorkflowSequenceId,
                    TemplateId = x.TemplateId,
                    WorkflowType = x.Workflow.WorkflowType,
                    WorkflowName = x.WorkflowName,
                    Sla = x.Sla,
                    isOptional = x.WorkflowIsOptional,
                    documentGroup = x.RefTemplateDocuments.Select(y => new WorkflowSeqDocGroupDto()
                    {
                        DocGroupParId = y.DocGroupParId ?? "",
                        DocVersion = y.DocGroupVersion ?? 0,
                        DocGroup = _context.MdParamaterLists
                        .Where(p => p.ParamListId == y.DocGroupParId && p.ParamId == "DocGroup" && p.Schema == "idams")
                        .Select(p => p.ParamValue1Text).FirstOrDefault() ?? ""
                    }).ToList()
                }).FirstOrDefaultAsync();
            return data;
        }

        public async Task<List<RefTemplateWorkflowSequence>> GetListTemplateWorkflowSequence(string? templateId, int? templateVersion)
        {
            return await _context.RefTemplateWorkflowSequences.AsNoTracking().Where(n => n.TemplateId == templateId && n.TemplateVersion == templateVersion && n.Deleted == false)
                .OrderBy(n => n.Order)
                .ToListAsync();
        }

        public async Task<bool> DeleteWorkflowSequenceAsync(string workflowSequenceId)
        {
            var item = await _context.RefTemplateWorkflowSequences.Where(n => n.WorkflowSequenceId == workflowSequenceId).SingleOrDefaultAsync();
            item.Template = await _context.RefTemplates.
                Where(x => x.TemplateId == item.TemplateId && x.TemplateVersion == item.TemplateVersion).SingleOrDefaultAsync();
            
            if(item.Template?.Status == StatusConstant.Published)
            {
                throw new InvalidDataException($"Cannot delete template sequence {item.Template.TemplateId}:{item.Template.TemplateVersion}. This template is already active!");
            }
            if (item == null) return false;

            if (item.Template?.TotalWorkflow == item.Order.GetValueOrDefault())
            {
                var orderBefore = item.Order - 1;
                var lastSequence = await _context.RefTemplateWorkflowSequences.SingleOrDefaultAsync(n => n.TemplateId == item.Template.TemplateId &&
                n.TemplateVersion == item.Template.TemplateVersion && n.Order == orderBefore.GetValueOrDefault());
                item.Template.EndWorkflow = lastSequence?.WorkflowName;
            }

            var sequenceByTemplate = await _context.RefTemplateWorkflowSequences.Where(n => n.TemplateId == item.Template.TemplateId
                                    && n.TemplateVersion == item.Template.TemplateVersion && n.Deleted == false).Include(n => n.Workflow)
                                    .OrderBy(n => n.Order)
                                    .ToListAsync();
            sequenceByTemplate.RemoveAt(item.Order.GetValueOrDefault() - 1);

            var orderedSequenceAfterDelete = ReOrderBasedOnStage(sequenceByTemplate);
            await _context.SaveChangesAsync();

            item.Order = null;
            item.Deleted = true;
            item.Template.TotalWorkflow = item.Template.TotalWorkflow - 1;
            item.UpdatedBy = GetCurrentUser;
            item.UpdatedDate = DateTime.UtcNow;
            _context.RefTemplateWorkflowSequences.Update(item);
            await _context.SaveChangesAsync();

            item.Template.UpdatedBy = _currentUserService.CurrentUser.Id;
            item.Template.UpdatedDate = DateTime.UtcNow;
            _context.RefTemplates.Update(item.Template);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteWorkflowSequence(List<RefTemplateWorkflowSequence> refTemplateWorkflowSequences)
        {
            _context.RemoveRange(refTemplateWorkflowSequences);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
