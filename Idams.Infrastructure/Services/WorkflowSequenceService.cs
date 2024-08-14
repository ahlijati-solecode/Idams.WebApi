using Idams.Core.Constants;
using Idams.Core.Model.Dtos;
using Idams.Core.Model.Entities;
using Idams.Core.Model.Filters;
using Idams.Core.Model.Requests;
using Idams.Core.Repositories;
using Idams.Core.Services;

namespace Idams.Infrastructure.Services
{
    public class WorkflowSequenceService : IWorkflowSequenceService
    {
        private readonly IRefTemplateWorkflowSequenceRepository _refTemplateWorkflowSequenceRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDocChecklistRepository _docChecklistRepository;
        private readonly IUnitOfWorks _unitOfWorks;
        private readonly IWorkflowSettingRepository _workflowSettingRepository;
        public WorkflowSequenceService(IRefTemplateWorkflowSequenceRepository refTemplateWorkflowSequenceRepository,
            ICurrentUserService currentUserService,
            IDocChecklistRepository docChecklistRepository, IUnitOfWorks unitOfWorks, IWorkflowSettingRepository workflowSettingRepository)
        {
            _refTemplateWorkflowSequenceRepository = refTemplateWorkflowSequenceRepository;
            _currentUserService = currentUserService;
            _docChecklistRepository = docChecklistRepository;
            _unitOfWorks = unitOfWorks;
            _workflowSettingRepository = workflowSettingRepository; 
        }

        public async Task<RefTemplateWorkflowSequence> AddorUpdateWorkflowSequece(TemplateWorkflowSeqRequest param)
        {
            var template = await _workflowSettingRepository.GetByTemplateIdAndTemplateVersion(param.templateId, param.templateVersion);
            if (template.Status == StatusConstant.Published)
            {
                throw new InvalidOperationException($"Cannot add/update template sequence {param.templateId}:{param.templateVersion}. This template is already active!");
            }

            var entity = new RefTemplateWorkflowSequence()
            {
                TemplateId = param.templateId,
                TemplateVersion = param.templateVersion,
                //WorkflowId = param.workflowId, //not include workflowid to avoid conflict
                WorkflowName = param.workflowName,
                Sla = param.SLA,
                SlauoM = param.SLAUoM,
                WorkflowIsOptional = param.isOptional,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = _currentUserService.CurrentUser.Id,
                Deleted = false
            };

            if (string.IsNullOrWhiteSpace(param.workflowSequenceId))
            {
                entity.WorkflowId = param.workflowId;
                return await _refTemplateWorkflowSequenceRepository.AddNewWorkflowSequence(entity);
            }
            else
            {
                var existEntity = await _refTemplateWorkflowSequenceRepository.GetByWorkflowSequenceId(param.workflowSequenceId);
                var result =   await _refTemplateWorkflowSequenceRepository.UpdateRefTemplateWorkflowSequenceAsync(existEntity, entity);
                await _unitOfWorks.SaveChangesAsync();
                return result;
            }
        }

        public async Task<TemplateWorkflowSeqDto?> GetWorkflowSequece(string workflowSequenceId)
        {
            var result = await _refTemplateWorkflowSequenceRepository.GetWorkflowSequence(workflowSequenceId);
            return result;
        }

        public async Task<bool> SaveDocumentChecklist(DocumentChecklistRequest param)
        {
            var ListDockChecklist = new List<MpDocumentChecklist>();
            foreach (var item in param.DocList)
            {
                ListDockChecklist.Add(new MpDocumentChecklist()
                {
                    DocGroupParId = param.DocGroupParId,
                    DocGroupVersion = param.DocVersion,
                    DocDescriptionId = item,
                    ModifiedDate = DateTime.UtcNow
                });
            }
            var result = await _docChecklistRepository.SaveDocumentChecklist(param.DocGroupParId, param.DocVersion, ListDockChecklist);
            return result;
        }

        public async Task<bool> DeleteDocumentChecklist(DocumentChecklistRequest param)
        {
            var ListDockChecklist = new List<MpDocumentChecklist>();
            foreach (var item in param.DocList)
            {
                ListDockChecklist.Add(new MpDocumentChecklist()
                {
                    DocGroupParId = param.DocGroupParId,
                    DocGroupVersion = param.DocVersion,
                    DocDescriptionId = item,
                    ModifiedDate = DateTime.UtcNow
                });
            }
            var result = await _docChecklistRepository.DeleteDocumentChecklist(param.DocGroupParId, param.DocVersion, ListDockChecklist);
            return result;
        }

        public async Task<Paged<MpDocumentChecklistPagedDto>> GetDocumentChecklistPaged(PagedDto pagedDto, DocumentChecklistFilter filter, string DocGroupParId, int DocGroupVersion)
        {
            var result = await _docChecklistRepository.GetPaged(pagedDto, filter, DocGroupParId, DocGroupVersion);
            return result;
        }

        public async Task<bool> DeleteWorkflowSequece(string workflowSequenceId)
        {
            using var transaction = _unitOfWorks.BeginTransaction();
            try
            {
                var res = await _refTemplateWorkflowSequenceRepository.DeleteWorkflowSequenceAsync(workflowSequenceId);
                await transaction.CommitAsync();
                return res;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
