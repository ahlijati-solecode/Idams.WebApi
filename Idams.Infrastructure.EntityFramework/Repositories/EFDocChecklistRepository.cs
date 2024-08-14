using Idams.Core.Enums;
using Idams.Core.Model.Dtos;
using Idams.Core.Model.Entities;
using Idams.Core.Model.Filters;
using Idams.Core.Repositories;
using Idams.Infrastructure.EntityFramework.Core;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Idams.Core.Constants;

namespace Idams.Infrastructure.EntityFramework.Repositories
{
    public class EFDocChecklistRepository : EFRepository<MpDocumentChecklist, int, List<RoleEnum>>, IDocChecklistRepository
    {
        protected readonly IdamsDbContext _dbContext;
        private readonly ILogger<EFDocChecklistRepository> _logger;

        public EFDocChecklistRepository(IdamsDbContext context, IServiceProvider serviceProvider, ILogger<EFDocChecklistRepository> logger)
            : base(serviceProvider)
        {
            _dbContext = context;
            _logger = logger;
        }

        public Task<Paged<MpDocumentChecklistPagedDto>> GetPaged(PagedDto paged, DocumentChecklistFilter filter, string DocGroupParId, int DocGroupVersion)
        {
            try
            {
                var predicate = GenerateFilter(filter, DocGroupParId, DocGroupVersion);
                var query = (from x in _context.MpDocumentChecklists
                                join b in _context.MdParamaterLists on x.DocDescription.DocCategoryParId equals b.ParamListId
                                join c in _context.MdParamaterLists on x.DocDescription.DocTypeParId equals c.ParamListId
                                select new MpDocumentChecklistPagedDto
                                {
                                    DocGroupParId = x.DocGroupParId,
                                    DocGroupVersion = x.DocGroupVersion,
                                    DocId = x.DocDescriptionId,
                                    DocDescription = x.DocDescription.DocDescription ?? "",
                                    DocCategory = b.ParamValue1Text ?? "",//x.DocDescription.DocCategoryParId ?? "",
                                    DocType = c.ParamValue1Text ?? "",//x.DocDescription.DocTypeParId ?? "",
                                    ModifiedDate = x.ModifiedDate
                                }).AsNoTracking().Where(predicate).AsQueryable();

                var Paged = new Paged<MpDocumentChecklistPagedDto>();
                Paged.TotalItems = query.Count();
                query = GenerateOrder(query, paged.Sort ?? "");
                query = query
                .Skip((paged.Page - 1) * paged.Size)
                .Take(paged.Size);
                Paged.Items = query.ToList();
                return Task.FromResult(Paged);

                #region COMMENT CODE
                //var query = _context.MpDocumentChecklists.AsNoTracking().Include(x => x.DocDescription).AsQueryable();
                //if (!string.IsNullOrEmpty(filter.DocDescription))
                //{
                //    query = query.Where(x => x.DocDescription.DocDescription.Contains(filter.DocDescription));
                //}
                //if (!string.IsNullOrEmpty(filter.Category))
                //{
                //    var DocCategories = _context.MdParamaterLists.AsNoTracking().Where(x => x.Schema == "idams" && x.ParamId == "Stage" 
                //    && (x.ParamListId.Contains(filter.Category) || x.ParamValue1Text.Contains(filter.Category))).Select(x => x.ParamListId)
                //    .ToList();
                //    query = query.Where(x => DocCategories.Contains(x.DocDescription.DocCategoryParId));
                //}
                //if (!string.IsNullOrEmpty(filter.DocType))
                //{
                //    var DocTypes = _context.MdParamaterLists.AsNoTracking().Where(x => x.Schema == "idams" && x.ParamId == "DocType"
                //    && (x.ParamListId.Contains(filter.DocType) || x.ParamValue1Text.Contains(filter.DocType))).Select(x => x.ParamListId)
                //    .ToList();
                //    query = query.Where(x => DocTypes.Contains(x.DocDescription.DocTypeParId));
                //}
                //if (!string.IsNullOrEmpty(filter.DateModified))
                //{
                //    query = query.Where(x => x.ModifiedDate.ToString().Contains(filter.DateModified));
                //}
                //query = query.Where(x => x.DocGroupParId == DocGroupParId && x.DocGroupVersion == DocGroupVersion);

                //var Paged = new Paged<MpDocumentChecklistDto>();
                //Paged.TotalItems = await query.CountAsync().ConfigureAwait(true);
                //query = query.OrderBy(x => x.ModifiedDate)
                //.Skip((paged.Page - 1) * paged.Size)
                //.Take(paged.Size);
                #endregion
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private ExpressionStarter<MpDocumentChecklistPagedDto>? GenerateFilter(DocumentChecklistFilter filter, string DocGroupParId, int DocGroupVersion)
        {
            var predicate = PredicateBuilder.New<MpDocumentChecklistPagedDto>(true);
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
                predicate = predicate.And(x => x.ModifiedDate != null && x.ModifiedDate.Value.ToString().Contains(filter.DateModified));
            }
            predicate = predicate.And(x => x.DocGroupParId == DocGroupParId && x.DocGroupVersion == DocGroupVersion);
            return predicate;
        }

        private IQueryable<MpDocumentChecklistPagedDto> GenerateOrder(IQueryable<MpDocumentChecklistPagedDto> query, string sort)
        {
            if (sort.ToLower().Contains("asc"))
            {
                if (sort.IndexOf("DocDescription", StringComparison.OrdinalIgnoreCase) >= 0)
                    query = query.OrderBy(x => x.DocDescription);
                else if (sort.IndexOf("DocCategory", StringComparison.OrdinalIgnoreCase) >= 0)
                    query = query.OrderBy(x => x.DocCategory);
                else if (sort.IndexOf("DocType", StringComparison.OrdinalIgnoreCase) >= 0)
                    query = query.OrderBy(x => x.DocType);
                else if (sort.IndexOf("ModifiedDate", StringComparison.OrdinalIgnoreCase) >= 0)
                    query = query.OrderBy(x => x.ModifiedDate);
            }
            else
            {
                if (sort.IndexOf("DocDescription", StringComparison.OrdinalIgnoreCase) >= 0)
                    query = query.OrderByDescending(x => x.DocDescription);
                else if (sort.IndexOf("DocCategory", StringComparison.OrdinalIgnoreCase) >= 0)
                    query = query.OrderByDescending(x => x.DocCategory);
                else if (sort.IndexOf("DocType", StringComparison.OrdinalIgnoreCase) >= 0)
                    query = query.OrderByDescending(x => x.DocType);
                else if (sort.IndexOf("ModifiedDate", StringComparison.OrdinalIgnoreCase) >= 0)
                    query = query.OrderByDescending(x => x.ModifiedDate);
            }
            return query;
        }

        public async Task<bool> ValidateActiveTemplate(string docGroupParId, int docVersion)
        {
            var templateDocument = await _context.RefTemplateDocuments.AsNoTracking().SingleOrDefaultAsync(n => n.DocGroupParId == docGroupParId && n.DocGroupVersion == docVersion);
            var template = await _context.RefTemplates.AsNoTracking().SingleOrDefaultAsync(n => n.TemplateId == templateDocument.TemplateId && n.TemplateVersion == templateDocument.TemplateVersion);
            return template.Status == StatusConstant.Published;
        }

        public async Task<bool> SaveDocumentChecklist(string docGroupParId, int docVersion, List<MpDocumentChecklist> data)
        {
            using var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                var templateActive = await ValidateActiveTemplate(docGroupParId, docVersion);
                if (templateActive)
                {
                    throw new InvalidDataException($"Cannot add/update document checklist. This template is already active!");
                }
                var docList = await _dbContext.MpDocumentChecklists.AsQueryable().AsNoTracking()
                    .Where(n => n.DocGroupParId == docGroupParId && n.DocGroupVersion == docVersion).ToListAsync();
                var newData = data.Where(n => !docList.Exists(x => x.DocDescriptionId == n.DocDescriptionId)).ToList();
                var existData = data.Where(n => docList.Exists(x => x.DocDescriptionId == n.DocDescriptionId)).ToList();
                if(newData.Count > 0)
                {
                    _dbContext.MpDocumentChecklists.AddRange(newData);
                }
                if(existData.Count > 0)
                {
                    _dbContext.MpDocumentChecklists.UpdateRange(existData);
                }
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError(ex, "Error save document checklist");
                throw;
            }
        }

        public async Task<bool> DeleteDocumentChecklist(string docGroupParId, int docVersion, List<MpDocumentChecklist> data)
        {
            using var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                var templateActive = await ValidateActiveTemplate(docGroupParId, docVersion);
                if (templateActive)
                {
                    throw new InvalidDataException($"Cannot delete document checklist. This template is already active!");
                }
                var docList = await _dbContext.MpDocumentChecklists.AsQueryable()
                    .Where(n => n.DocGroupParId == docGroupParId && n.DocGroupVersion == docVersion).ToListAsync();
                var deleteData = docList.Where(n => data.Exists(x => x.DocDescriptionId == n.DocDescriptionId)).ToList();
                if(deleteData.Count > 0)
                {
                    _dbContext.MpDocumentChecklists.RemoveRange(deleteData);
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error delete document checklist");
                throw;
            }
        }

        public Task<List<MpDocumentChecklist>> GetDocumentCheckListByDocGroup(string? docGroupParId, int? docVersion)
        {
            var item = GetAll().AsNoTracking().Where(n => n.DocGroupParId == docGroupParId && n.DocGroupVersion == docVersion).Include(n => n.DocDescription).ToList();
            return Task.FromResult(item);
        }
    }
}
