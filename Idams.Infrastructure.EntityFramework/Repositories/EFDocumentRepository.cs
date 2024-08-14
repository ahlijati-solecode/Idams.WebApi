using AutoMapper;
using Dapper;
using Idams.Core.Constants;
using Idams.Core.Enums;
using Idams.Core.Model.Dtos;
using Idams.Core.Model.Entities;
using Idams.Core.Model.Filters;
using Idams.Core.Model.Responses;
using Idams.Core.Repositories;
using Idams.Infrastructure.EntityFramework.Core;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using shortid;
using shortid.Configuration;

namespace Idams.Infrastructure.EntityFramework.Repositories
{
    public class EFDocumentRepository : EFRepository<MdDocumentDescription, int, List<RoleEnum>>, IDocumentRepository
    {
        private readonly IMapper _mapper;
        public EFDocumentRepository(IServiceProvider serviceProvider, IMapper mapper) : base(serviceProvider)
        {
            _mapper = mapper;
        }

        public Task<Paged<MdDocumentPagedDto>> GetPagedAsync(MdDocumentFilter filter, int page = 1, int size = 10, string? sort = "DocDescription asc")
        {
            try
            {
                var predicate = GenerateFilter(filter);
                var query = (from a in _context.MdDocumentDescriptions
                             join b in _context.MdParamaterLists on a.DocCategoryParId equals b.ParamListId
                             join c in _context.MdParamaterLists on a.DocTypeParId equals c.ParamListId
                             select new MdDocumentPagedDto()
                             {
                                 DocDescriptionId = a.DocDescriptionId,
                                 DocCategoryParId = a.DocCategoryParId,
                                 DocCategory = b.ParamValue1Text,
                                 DocDescription = a.DocDescription,
                                 DocTypeParId = a.DocTypeParId,
                                 DocType = c.ParamValue1Text,
                                 CreatedDate = a.CreatedDate,
                             }).AsNoTracking().Where(predicate).AsQueryable();

                var Paged = new Paged<MdDocumentPagedDto>();
                Paged.TotalItems = query.Count();
                query = GenerateOrder(query, sort ?? "");
                query = query
                .Skip((page - 1) * size)
                .Take(size);
                Paged.Items = query.ToList();
                return Task.FromResult(Paged);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private ExpressionStarter<MdDocumentPagedDto>? GenerateFilter(MdDocumentFilter filter)
        {
            var predicate = PredicateBuilder.New<MdDocumentPagedDto>(true);
            if (filter.DocDescription != null)
            {
                predicate = predicate.And(x => x.DocDescription != null && x.DocDescription.ToLower().Contains(filter.DocDescription));
            }
            if (filter.DocCategory != null)
            {
                predicate = predicate.And(x => x.DocCategory != null && x.DocCategory.ToLower().Contains(filter.DocCategory));
            }
            if (filter.DocType != null)
            {
                predicate = predicate.And(x => x.DocType != null && x.DocType.ToLower().Contains(filter.DocType));
            }
            if (filter.DateModified != null)
            {
                predicate = predicate.And(x => x.CreatedDate != null && x.CreatedDate.Value.ToString().ToLower().Contains(filter.DateModified));
            }
            predicate = predicate.And(x => x.DocTypeParId != ParIdConstant.DokumenPendukungParId);
            return predicate;
        }
        
        private IQueryable<MdDocumentPagedDto> GenerateOrder(IQueryable<MdDocumentPagedDto> query, string sort)
        {
            if (sort.ToLower().Contains("asc"))
            {
                if(sort.IndexOf("DocDescription", StringComparison.OrdinalIgnoreCase) >= 0)
                    query = query.OrderBy(x => x.DocDescription);
                else if(sort.IndexOf("DocCategory", StringComparison.OrdinalIgnoreCase) >= 0)
                    query = query.OrderBy(x => x.DocCategory);
                else if (sort.IndexOf("DocType", StringComparison.OrdinalIgnoreCase) >= 0)
                    query = query.OrderBy(x => x.DocType);
                else if (sort.IndexOf("CreatedDate", StringComparison.OrdinalIgnoreCase) >= 0)
                    query = query.OrderBy(x => x.CreatedDate);
            }
            else
            {
                if (sort.IndexOf("DocDescription", StringComparison.OrdinalIgnoreCase) >= 0)
                    query = query.OrderByDescending(x => x.DocDescription);
                else if (sort.IndexOf("DocCategory", StringComparison.OrdinalIgnoreCase) >= 0)
                    query = query.OrderByDescending(x => x.DocCategory);
                else if (sort.IndexOf("DocType", StringComparison.OrdinalIgnoreCase) >= 0)
                    query = query.OrderByDescending(x => x.DocType);
                else if (sort.IndexOf("CreatedDate", StringComparison.OrdinalIgnoreCase) >= 0)
                    query = query.OrderByDescending(x => x.CreatedDate);
            }
            return query;
        }

        public async Task<ProjectDocumentGroupResponse> GetDocumentGroupFromProjectAction(string projectActionId)
        {
            var projectAction = await _context.TxProjectActions.AsNoTracking().Where(n => n.ProjectActionId == projectActionId)
                .FirstOrDefaultAsync();
            if (projectAction == null)
                throw new InvalidDataException($"ProjectAction {projectActionId} not found");

            var templateDoc = await _context.RefTemplateDocuments.AsNoTracking().Where(n => n.WorkflowSequenceId == projectAction.WorkflowSequenceId &&
                n.WorkflowActionId == projectAction.WorkflowActionId).FirstOrDefaultAsync();

            if(templateDoc == null)
            {
                throw new InvalidOperationException($"there is no document for workflowActionId {projectAction.WorkflowActionId}");
            }

            var paramList = await _context.MdParamaterLists.Where(n => n.Schema == "idams" && n.ParamId == "DocGroup" && n.ParamListId == templateDoc.DocGroupParId).FirstAsync();
            ProjectDocumentGroupResponse ret = new ProjectDocumentGroupResponse();
            ret.ProjectActionId = projectActionId;
            ret.DocGroupName = paramList.ParamValue1Text!;

            bool? IsActive = true;
            ret.RequiredDocs = await (from mp in _context.MpDocumentChecklists
                         join md in _context.MdDocumentDescriptions on mp.DocDescriptionId equals md.DocDescriptionId
                         join td in _context.TxDocuments on new { mp.DocDescriptionId, projectAction.ProjectActionId, IsActive } equals new { td.DocDescriptionId, td.ProjectActionId, td.IsActive } into map
                         from td in map.DefaultIfEmpty()
                         where mp.DocGroupParId == templateDoc.DocGroupParId && mp.DocGroupVersion == templateDoc.DocGroupVersion
                         orderby md.DocDescription ascending
                         select new DocumentDetailResponse()
                         {
                             RequiredName = md.DocDescription!,
                             DocDescriptionId = md.DocDescriptionId,
                             UploadedDocument = _mapper.Map<DocumentDto>(td)
                         }).AsNoTracking().ToListAsync();

            ret.SupportingDocs = await (from td in _context.TxDocuments
                                        join md in _context.MdDocumentDescriptions on td.DocDescriptionId equals md.DocDescriptionId
                                        where td.ProjectActionId == projectAction.ProjectActionId && md.DocTypeParId == ParIdConstant.DokumenPendukungParId
                                        select new DocumentDetailResponse()
                                        {
                                            RequiredName = md.DocDescription!,
                                            DocDescriptionId = md.DocDescriptionId,
                                            UploadedDocument = _mapper.Map<DocumentDto>(td)
                                        }).AsNoTracking().ToListAsync();

            return ret;
        }

        public async Task<TxDocument> SaveAsync(TxDocument document)
        {
            if (string.IsNullOrWhiteSpace(document.TransactionDocId))
            {
                document.TransactionDocId = ShortId.Generate(new GenerationOptions(true, false, 20));
                document.CreatedDate = DateTime.UtcNow;
                document.CreatedBy = GetCurrentUser;
                document.UpdatedDate = DateTime.UtcNow;
                document.UpdatedBy = GetCurrentUser;
                await _context.TxDocuments.AddAsync(document);
            }
            else
            {
                document.UpdatedDate = DateTime.UtcNow;
                document.UpdatedBy = GetCurrentUser;
                _context.TxDocuments.Update(document);
            }
            return document;
        }

        public TxDocument SaveAsOldVersion(TxDocument document)
        {
            document.IsActive = false;
            _context.TxDocuments.Update(document);
            return document;
        }

        public async Task<TxDocument?> GetByProjectActionAndDocId(string projectActionId, string docDescriptionId)
        {
            return await _context.TxDocuments.Where(n => n.ProjectActionId == projectActionId && n.DocDescriptionId == docDescriptionId && n.IsActive == true)
                .AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<TxDocument?> GetByTransactionId(string transactionId)
        {
            return await _context.TxDocuments.AsNoTracking().Where(n => n.TransactionDocId == transactionId)
                .Include(n => n.DocDescription).Include(n => n.ProjectAction).ThenInclude(n => n.WorkflowSequence).FirstOrDefaultAsync();
        }

        public async Task<TxDocument?> RenameDocument(string transactionId, string newName)
        {
            var doc = await _context.TxDocuments.Where(n => n.TransactionDocId == transactionId).FirstOrDefaultAsync();
            if (doc == null)
                return null;

            doc.DocName = newName;
            doc.UpdatedDate = DateTime.UtcNow;
            doc.UpdatedBy = GetCurrentUser;
            await _context.SaveChangesAsync();
            return doc;
        }

        public async Task<bool> DeleteAsync(TxDocument doc)
        {
            _context.TxDocuments.Remove(doc);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<MdDocumentDescription?> GetDocumentDescription(string docDescriptionId)
        {
            return await _context.MdDocumentDescriptions.AsNoTracking().Where(n => n.DocDescriptionId == docDescriptionId).SingleOrDefaultAsync();
        }

        public async Task<TxDocument?> GetDocumentByTransactionDocId(string transactionDocId)
        {
            return await _context.TxDocuments.AsNoTracking().Include(n => n.ProjectAction).Include(n => n.DocDescription).SingleOrDefaultAsync(n => n.TransactionDocId == transactionDocId);
        }

        public async Task<List<DocumentManagementPreviewDto>> GetDocumentByProjectAndDocDescription(string projectId, string docTypeParId)
        {
            var res = await (from td in _context.TxDocuments
                             join tpa in _context.TxProjectActions
                             on td.ProjectActionId equals tpa.ProjectActionId
                             join mdd in _context.MdDocumentDescriptions
                             on td.DocDescriptionId equals mdd.DocDescriptionId
                             join tph in _context.TxProjectHeaders
                             on tpa.ProjectId equals tph.ProjectId
                             join wac in _context.RefWorkflowActions
                             on td.WorkflowActionId equals wac.WorkflowActionId
                             join wo in _context.RefWorkflows
                             on wac.WorkflowId equals wo.WorkflowId
                             join upshier in (from ups in _context.TxProjectUpstreamEntities.Distinct()
                                              join hier in _context.VwShuhier03s.Distinct() on ups.EntityId equals hier.EntityId
                                              select new
                                              {
                                                  ups.ProjectId,
                                                  hier.HierLvl2Desc,
                                                  hier.HierLvl3Desc
                                              }).Distinct()
                             on tph.ProjectId equals upshier.ProjectId
                             where mdd.DocTypeParId == docTypeParId && tpa.ProjectId == projectId
                             select new DocumentManagementPreviewDto
                             {
                                 TransactionDocId = td.TransactionDocId,
                                 FileName = td.DocName,
                                 DocType = _context.MdParamaterLists.Where(n => n.Schema == "idams" && n.ParamId == "docType" && n.ParamListId == mdd.DocTypeParId).SingleOrDefault()!.ParamValue1Text,
                                 ProjectName = tph.ProjectName,
                                 WorkflowType = wo.WorkflowType,
                                 Regional = upshier.HierLvl2Desc,
                                 Zona = upshier.HierLvl3Desc,
                                 Threshold = _context.MdParamaterLists.Where(n => n.Schema == "idams" && n.ParamId == "ThresholdName" && n.ParamListId == td.ThresholdNameParId).SingleOrDefault()!.ParamValue1Text,
                                 FileExtension = td.FileExtension,
                                 LastModified = td.UpdatedDate
                             }).Distinct().ToListAsync();
            res = res.OrderByDescending(n => n.LastModified).ToList();
            return res;
        }
    }
}
