using AutoMapper;
using Idams.Core.Constants;
using Idams.Core.Enums;
using Idams.Core.Model;
using Idams.Core.Model.Dtos;
using Idams.Core.Model.Entities;
using Idams.Core.Model.Filters;
using Idams.Core.Model.Requests;
using Idams.Core.Model.Responses;
using Idams.Core.Repositories;
using Idams.Core.Services;
using Microsoft.AspNetCore.Http;
using shortid;
using shortid.Configuration;

namespace Idams.Infrastructure.Services
{
    public class WorkflowService : IWorkflowService
    {
        private readonly IWorkflowSettingRepository _workflowSettingRepository;
        private readonly IRefTemplateWorkflowSequenceRepository _refTemplateWorkflowSequenceRepository;
        private readonly IRefWorkflowRepository _iRefWorkflowRepository;
        private readonly IDocChecklistRepository _docChecklistRepository;
        private readonly IUnitOfWorks _unitOfWorks;
        private readonly IParameterListRepository _parameterListRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEmailService _emailService;
        private readonly ITemplateDocumentRepository _templateDocumentRepository;

        public WorkflowService(IWorkflowSettingRepository workflowSettingRepository, 
            IRefTemplateWorkflowSequenceRepository refTemplateWorkflowSequenceRepository,
            IRefWorkflowRepository iRefWorkflowRepository, 
            IDocChecklistRepository docChecklistRepository,
            IUnitOfWorks unitOfWorks, IMapper mapper,
            ICurrentUserService currentUserService,
            IParameterListRepository parameterListRepository,
            IEmailService emailService,
            ITemplateDocumentRepository templateDocumentRepository)
        {
            _mapper = mapper;
            _workflowSettingRepository = workflowSettingRepository;
            _refTemplateWorkflowSequenceRepository = refTemplateWorkflowSequenceRepository;
            _iRefWorkflowRepository = iRefWorkflowRepository;
            _docChecklistRepository = docChecklistRepository;
            _unitOfWorks = unitOfWorks;
            _currentUserService = currentUserService;
            _parameterListRepository = parameterListRepository;
            _emailService = emailService;
            _templateDocumentRepository = templateDocumentRepository;
        }

        protected string GetCurrentUser
        {
            get { return _currentUserService.CurrentUser?.Id ?? String.Empty; }
        }




        public async Task AssingDataTemplate(RefTemplate data)
        {
            var latestId = await _workflowSettingRepository.GetLastTemplateId();
            data.TemplateId = IdGenerationUtil.GenerateNextId(latestId, "TI");
            data.TemplateVersion = 1;
            data.ThresholdVersion = await _workflowSettingRepository.GetThresholdVersion(data.ThresholdNameParId);
            data.TotalWorkflow = 1;
            data.IsActive = true;
            data.StartWorkflow = "Project Initiation";
            data.EndWorkflow = "Project Initiation";
            data.Deleted = false;
            data.UpdatedDate = DateTime.UtcNow;
            data.UpdatedBy = GetCurrentUser;
        }

        public async Task<string> GenerateTemplateId()
        {
            string lastId = await _workflowSettingRepository.GetLastTemplateId();
            string newId = IncrementStringEnd(lastId, 5);
            return newId;
        }

        public static string IncrementStringEnd(string name, int minNumericalCharacters = 1)
        {
            var prefix = System.Text.RegularExpressions.Regex.Match(name, @"\d+$");
            if (prefix.Success)
            {
                var capture = prefix.Captures[0];
                int number = int.Parse(capture.Value) + 1;
                name = name.Remove(capture.Index, capture.Length) + number.ToString("D" + minNumericalCharacters);
            }

            return name;
        }

        public List<MpDocumentChecklist> AddDocumentChecklistInitiation(WorkflowSeqDocGroupDto seqDocGroupDto)
        {
            List<MpDocumentChecklist> checklists = new List<MpDocumentChecklist>();
            checklists.Add(new MpDocumentChecklist()
            {
                DocGroupParId = seqDocGroupDto.DocGroupParId,
                DocGroupVersion = seqDocGroupDto.DocVersion,
                DocDescriptionId = "D_2022_00001",
                ModifiedDate = DateTime.UtcNow
            });
            checklists.Add(new MpDocumentChecklist()
            {
                DocGroupParId = seqDocGroupDto.DocGroupParId,
                DocGroupVersion = seqDocGroupDto.DocVersion,
                DocDescriptionId = "D_2022_00002",
                ModifiedDate = DateTime.UtcNow
            });
            checklists.Add(new MpDocumentChecklist()
            {
                DocGroupParId = seqDocGroupDto.DocGroupParId,
                DocGroupVersion = seqDocGroupDto.DocVersion,
                DocDescriptionId = "D_2022_00003",
                ModifiedDate = DateTime.UtcNow
            });
            checklists.Add(new MpDocumentChecklist()
            {
                DocGroupParId = seqDocGroupDto.DocGroupParId,
                DocGroupVersion = seqDocGroupDto.DocVersion,
                DocDescriptionId = "D_2022_00004",
                ModifiedDate = DateTime.UtcNow
            });
            checklists.Add(new MpDocumentChecklist()
            {
                DocGroupParId = seqDocGroupDto.DocGroupParId,
                DocGroupVersion = seqDocGroupDto.DocVersion,
                DocDescriptionId = "D_2022_00005",
                ModifiedDate = DateTime.UtcNow
            });
            checklists.Add(new MpDocumentChecklist()
            {
                DocGroupParId = seqDocGroupDto.DocGroupParId,
                DocGroupVersion = seqDocGroupDto.DocVersion,
                DocDescriptionId = "D_2022_00006",
                ModifiedDate = DateTime.UtcNow
            });
            checklists.Add(new MpDocumentChecklist()
            {
                DocGroupParId = seqDocGroupDto.DocGroupParId,
                DocGroupVersion = seqDocGroupDto.DocVersion,
                DocDescriptionId = "D_2022_00007",
                ModifiedDate = DateTime.UtcNow
            });
            checklists.Add(new MpDocumentChecklist()
            {
                DocGroupParId = seqDocGroupDto.DocGroupParId,
                DocGroupVersion = seqDocGroupDto.DocVersion,
                DocDescriptionId = "D_2022_00008",
                ModifiedDate = DateTime.UtcNow
            });

            return checklists;
        }
        public async Task<RefTemplateDto?> AddWorkflowSettingAsync(RefTemplateDto dto, List<RefTemplateWorkflowSequenceDto> sequenceDtos)
        {
            var template = _mapper.Map<RefTemplate>(dto);
            List<RefTemplateWorkflowSequence> listSequence = _mapper.Map<List<RefTemplateWorkflowSequence>>((sequenceDtos));
            
            RefTemplate? result = null;
            if (template.TemplateId == null)
            {
                var checkTemplateCriteria = await _workflowSettingRepository.CheckThresholdCategoryCriteriaSubCriteria(template.ThresholdNameParId!, template.ProjectCategoryParId!, template.ProjectCriteriaParId, template.ProjectSubCriteriaParId);
                if (checkTemplateCriteria) return null;
                await AssingDataTemplate(template);
                result = await _workflowSettingRepository.AddTemplateAsync(template);

                //add default workflow sequence
                string wfId = template.ThresholdNameParId == ParIdConstant.ThresholdNameRegional ? WorkflowIdConstant.InitiationReg : WorkflowIdConstant.InitiationShu;
                RefTemplateWorkflowSequence refTemplateWorkflowSequence = new RefTemplateWorkflowSequence();
                refTemplateWorkflowSequence.TemplateId = template.TemplateId;
                refTemplateWorkflowSequence.TemplateVersion = template.TemplateVersion;
                refTemplateWorkflowSequence.WorkflowName = "Project Initiation";
                refTemplateWorkflowSequence.WorkflowId = wfId;
                refTemplateWorkflowSequence.Sla = 1;
                refTemplateWorkflowSequence.SlauoM = "day";
                refTemplateWorkflowSequence.WorkflowIsOptional = false;
                refTemplateWorkflowSequence.Order = 1;
                refTemplateWorkflowSequence.Deleted = false;
                refTemplateWorkflowSequence.CreatedBy = GetCurrentUser;
                refTemplateWorkflowSequence.CreatedDate = DateTime.UtcNow;

                await _refTemplateWorkflowSequenceRepository.AddNewWorkflowSequence(refTemplateWorkflowSequence);

                var tempWorkflowSeqDto = await _refTemplateWorkflowSequenceRepository.GetWorkflowSequence(refTemplateWorkflowSequence.WorkflowSequenceId);
                var documentGroupFirst = tempWorkflowSeqDto.documentGroup[0];
                List<MpDocumentChecklist> documentChecklists = AddDocumentChecklistInitiation(documentGroupFirst);
                
                await _docChecklistRepository.SaveDocumentChecklist(tempWorkflowSeqDto.documentGroup[0].DocGroupParId, tempWorkflowSeqDto.documentGroup[0].DocVersion,
                    documentChecklists);
            }
            else
            {
                var refTemplateEntity = await _workflowSettingRepository.GetByTemplateIdAndTemplateVersion(template.TemplateId, template.TemplateVersion);
                await ValidateUpdateSequence(template.TemplateId, template.TemplateVersion, listSequence, refTemplateEntity);
                var lastWorkflowIdInSequence = sequenceDtos.OrderBy(x => x.Order).Last().WorkflowId;
                refTemplateEntity.EndWorkflow = _iRefWorkflowRepository.GetByWorkflowId(lastWorkflowIdInSequence).Result.WorkflowType;

                foreach(var seq in listSequence)
                {
                    RefTemplateWorkflowSequence templateWorkflowSequence = await _refTemplateWorkflowSequenceRepository.GetByWorkflowSequenceId(seq.WorkflowSequenceId);
                    await _refTemplateWorkflowSequenceRepository.UpdateRefTemplateWorkflowSequenceAsync(templateWorkflowSequence, seq);
                }
                //lookup to prev template to bypass checkcriteria if all criteria in new template matching criteria in oldtemplate
                if ( template.ThresholdNameParId != refTemplateEntity.ThresholdNameParId || template.ProjectCategoryParId != refTemplateEntity.ProjectCategoryParId || template.ProjectCriteriaParId != refTemplateEntity.ProjectCriteriaParId
                    || template.ProjectSubCriteriaParId != refTemplateEntity.ProjectSubCriteriaParId)
                {
                    var checkTemplateCriteria = await _workflowSettingRepository.CheckThresholdCategoryCriteriaSubCriteria(template.ThresholdNameParId!, template.ProjectCategoryParId!, template.ProjectCriteriaParId, template.ProjectSubCriteriaParId);
                    if (checkTemplateCriteria) return null;
                }
                result = await _workflowSettingRepository.UpdateRefTemplateAsync(refTemplateEntity, template);

                await _unitOfWorks.SaveChangesAsync();
                if(template.Status == StatusConstant.Published)
                {
                    List<SendMailRequest> sendMailReqList = new List<SendMailRequest>();
                    SendMailRequest sendMailRequest = new SendMailRequest()
                    {
                        EmailCode = "IDAMS.ProjectWorkflowActivated",
                        ActionBy = _currentUserService.CurrentUserInfo.Email,
                        To = _currentUserService.CurrentUserInfo.Email,
                        ListJson = new List<BaseKeyValueObject> {
                        new BaseKeyValueObject {Key = "User", Value = _currentUserService.CurrentUserInfo.Name},
                        new BaseKeyValueObject {Key = "Project_Workflow_Template", Value = template.TemplateName},
                        new BaseKeyValueObject {Key = "Project_Category", Value=_parameterListRepository.GetParam("idams", "ProjectCategory", template.ProjectCategoryParId).Result!.ParamValue1Text},
                        new BaseKeyValueObject {Key = "Project_Criteria", Value = _parameterListRepository.GetParam("idams", "ProjectCriteria", template.ProjectCriteriaParId).Result!.ParamValue1Text},
                        new BaseKeyValueObject {Key = "Project_SubCriteria", Value = template.ProjectSubCriteriaParId == null ? "-" :
                        _parameterListRepository.GetParam("idams", "SubProjectCriteria", template.ProjectSubCriteriaParId).Result!.ParamValue1Text},
                        new BaseKeyValueObject {Key = "Project_Threshold", Value = _parameterListRepository.GetParam("idams", "ThresholdName", template.ThresholdNameParId).Result!.ParamValue1Text}
                    }
                    };
                    sendMailReqList.Add(sendMailRequest);
                    await _emailService.SendMailAsync(sendMailReqList);
                }
            }
            return _mapper.Map<RefTemplateDto>(result);
        }

        private async Task<bool> ValidateUpdateSequence(string templateId, int templateVersion, List<RefTemplateWorkflowSequence> input, RefTemplate template)
        {
            if (template.Status == StatusConstant.Published)
            {
                throw new InvalidOperationException($"Cannot update template {templateId}:{templateVersion}. This template is already active!");
            }

            var existing = await _refTemplateWorkflowSequenceRepository.GetListTemplateWorkflowSequence(templateId, templateVersion);
            if (existing.Count != input.Count)
            {
                throw new InvalidDataException($"Template {templateId}:{templateVersion}, Expected to update {existing.Count} sequences, but got {input.Count} sequences as arguments");
            }

            var orderred = input.OrderBy(n => n.Order).ToList();
            for (int i = 0; i < orderred.Count; i++)
            {
                RefTemplateWorkflowSequence? item = orderred[i];
                if(item.Order != i + 1)
                {
                    throw new InvalidDataException($"Sequence {item.WorkflowName} expected to have order: {i+1} but have {item.Order} instead");
                }
            }

            return true;
        }

        public async Task<bool> DeleteWorkflowSettingAsync(string templateId, int? templateVersion)
        {
            var template = await _workflowSettingRepository.GetByTemplateIdAndTemplateVersion(templateId, templateVersion);

            if(template != null)
            {
                //if(template.Status == StatusConstant.Published)
                //{
                //    throw new InvalidOperationException($"Cannot delete template {templateId}, template already active");
                //}
                template.Deleted = true;
                await _workflowSettingRepository.UpdateRefTemplateAsync(template, new());

                List<SendMailRequest> sendMailReqList = new List<SendMailRequest>();
                SendMailRequest sendMailRequest = new SendMailRequest()
                {
                    EmailCode = "IDAMS.ProjectWorkflowDeleted",
                    ActionBy = _currentUserService.CurrentUserInfo.Email,
                    To = _currentUserService.CurrentUserInfo.Email,
                    ListJson = new List<BaseKeyValueObject> {
                        new BaseKeyValueObject {Key = "User", Value = _currentUserService.CurrentUserInfo.Name},
                        new BaseKeyValueObject {Key = "Project_Workflow_Template", Value = template.TemplateName},
                        new BaseKeyValueObject {Key = "Project_Category", Value=_parameterListRepository.GetParam("idams", "ProjectCategory", template.ProjectCategoryParId).Result!.ParamValue1Text},
                        new BaseKeyValueObject {Key = "Project_Criteria", Value = _parameterListRepository.GetParam("idams", "ProjectCriteria", template.ProjectCriteriaParId).Result!.ParamValue1Text},
                        new BaseKeyValueObject {Key = "Project_SubCriteria", Value = template.ProjectSubCriteriaParId == null ? "-" : 
                        _parameterListRepository.GetParam("idams", "SubProjectCriteria", template.ProjectSubCriteriaParId).Result!.ParamValue1Text},
                        new BaseKeyValueObject {Key = "Project_Threshold", Value = _parameterListRepository.GetParam("idams", "ThresholdName", template.ThresholdNameParId).Result!.ParamValue1Text}
                    }
                };
                sendMailReqList.Add(sendMailRequest);
                await _emailService.SendMailAsync(sendMailReqList);

                await _unitOfWorks.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<RefTemplateDto?> GetWorkflowSettingByTemplateAsync(string? templateId, int? templateVersion)
        {
            var template = await _workflowSettingRepository.GetByTemplateIdAndTemplateVersionWithTemplateDocument(templateId, templateVersion);
            if (template == null) return null;

            var result = _mapper.Map<RefTemplateDto>(template);
            var sequences = template.RefTemplateWorkflowSequences.Where(n => n.Deleted == false).ToList();
            List<RefTemplateWorkflowSequenceDto> workflowSeqDto = _mapper.Map<List<RefTemplateWorkflowSequenceDto>>(sequences);

            int idx = 0;

            foreach (var sequence in sequences)
            {
                List<MpDocumentChecklistDto> documents = new();
                foreach (var document in sequence.RefTemplateDocuments)
                {
                    MpDocumentChecklistDto mpDocumentChecklistDto = new MpDocumentChecklistDto();
                    var documentChecklist = await _docChecklistRepository.GetDocumentCheckListByDocGroup(document.DocGroupParId, document.DocGroupVersion);
                    mpDocumentChecklistDto.GroupName = _parameterListRepository.GetParam("idams", "DocGroup", document.DocGroupParId).Result.ParamValue1Text;
                    documentChecklist.ForEach(elem =>
                    {
                        mpDocumentChecklistDto.DocList.Add(elem.DocDescription.DocDescription);
                    });
                    documents.Add(mpDocumentChecklistDto);
                }
                workflowSeqDto[idx].Documents = documents;
                idx++;
            }

            var resultSeq = workflowSeqDto.OrderBy(n => n.Order).ToList();
            resultSeq.ForEach(n =>
            {
                n.WorkflowType = _iRefWorkflowRepository.GetByWorkflowId(n.WorkflowId).Result.WorkflowType;
                n.WorkflowCategory = _iRefWorkflowRepository.GetByWorkflowId(n.WorkflowId).Result.WorkflowCategoryParId;
                result.WorkflowSequence.Add(n);
            });
            return result;
        }

        public async Task<GetWorkflowSettingDropdownDto> GetDropdownListAsync()
        {
            var items = await _workflowSettingRepository.GetDropdownList();
            return _mapper.Map<GetWorkflowSettingDropdownDto>(items);
        }

        public async Task<Paged<WorkflowSettingPagedDto>> GetPaged(PagedDto pagedDto, WorkflowSettingFilter filter, List<RoleEnum> roles)
        {
            var result = await _workflowSettingRepository.GetPaged(filter, roles, pagedDto.Page, pagedDto.Size, pagedDto.Sort);
            return _mapper.Map<Paged<WorkflowSettingPagedDto>>(result);
        }
        public async Task<List<RefWorkflowDto>> GetWorkflowType(string CategoryParId)
        {
            var result = await _iRefWorkflowRepository.GetList(CategoryParId);
            if(CategoryParId == "Inisiasi")
            {
                // Remove Default Type for Inisiasi
                var rem = result.FindAll(n => n.key == WorkflowIdConstant.InitiationReg || n.key == WorkflowIdConstant.InitiationShu);
                rem.ForEach(n =>
                {
                    result.Remove(n);
                });
            }
            return result;
        }

        public async Task<bool> SaveAllChanges(string templateId, int templateVersion)
        {
            using var transaction = _unitOfWorks.BeginTransaction();
            try
            {
                var templateUpdate = await _workflowSettingRepository.GetByTemplateIdAndTemplateVersion(templateId, templateVersion);
                templateUpdate.Status = StatusConstant.Published;
                templateUpdate.IsActive = true;
                await _workflowSettingRepository.UpdateRefTemplateAsync(templateUpdate, new());
                await _unitOfWorks.SaveChangesAsync();
                //set previous to inactive
                var prevTemplate = await _workflowSettingRepository.GetByTemplateIdAndTemplateVersion(templateId, templateVersion - 1);
                prevTemplate.IsActive = false;
                await _workflowSettingRepository.UpdateRefTemplateAsync(prevTemplate, new());
                await _unitOfWorks.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }catch (Exception ex)
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<RefTemplateDto> GenerateShadowTemplate(string templateId, int templateVersion)
        {
            RefTemplateDto dto = new RefTemplateDto();
            var oldTemplate = await _workflowSettingRepository.GetByTemplateIdAndTemplateVersionWithTemplateDocument(templateId, templateVersion);
            if (oldTemplate!.Status == StatusConstant.Published)
            {
                //reconstruct the template
                var templateDocs = await _templateDocumentRepository.GetTemplateDocumentByTemplateIdAndTemplateVersion(templateId, templateVersion + 1);
                foreach (var templateDoc in templateDocs)
                {
                    var docChecklist = await _docChecklistRepository.GetDocumentCheckListByDocGroup(templateDoc.DocGroupParId, templateDoc.DocGroupVersion);
                    var deletedDocChecklist = await _docChecklistRepository.DeleteDocumentChecklist(templateDoc.DocGroupParId!, templateDoc.DocGroupVersion.GetValueOrDefault(), docChecklist);
                    var deletedRefTemplateDocument = await _templateDocumentRepository.DeleteRefTemplateDocument(templateDoc);
                    var deletedDocGroup = await _templateDocumentRepository.DeleteDocGroup(templateDoc.DocGroupParId!, templateDoc.DocGroupVersion.GetValueOrDefault());
                }

                var workflowSequences = await _refTemplateWorkflowSequenceRepository.GetListTemplateWorkflowSequence(templateId, templateVersion + 1);
                var deletedWorkflowSequence = await _refTemplateWorkflowSequenceRepository.DeleteWorkflowSequence(workflowSequences);
                var refTemplate = await _workflowSettingRepository.GetByTemplateIdAndTemplateVersion(templateId, templateVersion + 1);
                var deletedRefTemplate = await _workflowSettingRepository.DeleteRefTemplate(refTemplate);


                var newTemplate = new RefTemplate
                {
                    TemplateId = oldTemplate.TemplateId,
                    Status = StatusConstant.Draft,
                    TemplateVersion = templateVersion + 1,
                    IsActive = false,
                    Deleted = false,
                    ThresholdNameParId = oldTemplate.ThresholdNameParId,
                    ThresholdVersion = oldTemplate.ThresholdVersion,
                    TemplateName = oldTemplate.TemplateName,
                    StartWorkflow = oldTemplate.StartWorkflow,
                    ProjectCategoryParId = oldTemplate.ProjectCategoryParId,
                    ProjectCriteriaParId = oldTemplate.ProjectCriteriaParId,
                    ProjectSubCriteriaParId = oldTemplate.ProjectSubCriteriaParId,
                    CreatedBy = _currentUserService.CurrentUser.Id,
                    UpdatedBy = _currentUserService.CurrentUser.Id,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                };
                await _workflowSettingRepository.AddTemplateAsync(newTemplate);
                dto.TemplateId = newTemplate.TemplateId;
                dto.TemplateVersion = newTemplate.TemplateVersion;
                oldTemplate.RefTemplateWorkflowSequences = oldTemplate.RefTemplateWorkflowSequences.Where(n => n.Deleted == false).ToList();
                oldTemplate.RefTemplateWorkflowSequences = oldTemplate.RefTemplateWorkflowSequences.OrderBy(n => n.Order).ToList();
                foreach (var seq in oldTemplate.RefTemplateWorkflowSequences)
                {
                    var newSeq = new RefTemplateWorkflowSequence
                    {
                        TemplateId = seq.TemplateId,
                        TemplateVersion = newTemplate.TemplateVersion,
                        WorkflowName = seq.WorkflowName,
                        WorkflowId = seq.WorkflowId,
                        Sla = seq.Sla,
                        SlauoM = seq.SlauoM,
                        WorkflowIsOptional = seq.WorkflowIsOptional,
                        CreatedBy = _currentUserService.CurrentUser.Id,
                        CreatedDate = DateTime.UtcNow,
                        Deleted = false
                    };
                    await _refTemplateWorkflowSequenceRepository.AddNewWorkflowSequence(newSeq);
                }
                await _unitOfWorks.SaveChangesAsync();
                var templateDocOldTemplate = await _templateDocumentRepository.GetTemplateDocumentByTemplateIdAndTemplateVersion(templateId, templateVersion);
                var templateDocNewTemplate = await _templateDocumentRepository.GetTemplateDocumentByTemplateIdAndTemplateVersion(templateId, templateVersion + 1);

                //add docChecklist
                for (int i = 0; i < templateDocOldTemplate.Count; i++)
                {
                    var docChecklistOldTemplate = await _docChecklistRepository.GetDocumentCheckListByDocGroup(templateDocOldTemplate.ElementAt(i).DocGroupParId, templateDocOldTemplate.ElementAt(i).DocGroupVersion);
                    var oldSeq = oldTemplate.RefTemplateWorkflowSequences.Where(n => n.WorkflowSequenceId == templateDocOldTemplate.ElementAt(i).WorkflowSequenceId).SingleOrDefault();
                    var allNewSeq = await _refTemplateWorkflowSequenceRepository.GetListTemplateWorkflowSequence(templateId, templateVersion + 1);
                    if (oldSeq != null)
                    {
                        var newSeqMatchOldSeq = allNewSeq.Single(n => n.WorkflowName == oldSeq.WorkflowName);

                        var newTemplateDoc = templateDocNewTemplate.Single(n => n.WorkflowSequenceId == newSeqMatchOldSeq.WorkflowSequenceId && n.WorkflowActionId == templateDocOldTemplate.ElementAt(i).WorkflowActionId);
                        List<MpDocumentChecklist> docCheckList = new List<MpDocumentChecklist>();
                        foreach (var docList in docChecklistOldTemplate)
                        {
                            var doc = new MpDocumentChecklist
                            {
                                DocGroupParId = newTemplateDoc.DocGroupParId!,
                                DocGroupVersion = newTemplateDoc.DocGroupVersion.GetValueOrDefault(),
                                DocDescriptionId = docList.DocDescriptionId,
                                ModifiedDate = DateTime.UtcNow,
                            };
                            docCheckList.Add(doc);
                        }
                        var saveDocChecklist = await _docChecklistRepository.SaveDocumentChecklist(newTemplateDoc.DocGroupParId!, newTemplateDoc.DocGroupVersion.GetValueOrDefault(), docCheckList);
                    }
                }
            }
            else
            {
                dto.TemplateId = templateId;
                dto.TemplateVersion = templateVersion;
            }
            return dto;
        }
    }
}
