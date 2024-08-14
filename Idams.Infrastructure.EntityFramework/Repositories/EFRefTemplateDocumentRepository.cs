using Idams.Core.Model.Entities;
using Idams.Core.Repositories;
using Idams.Core.Services;
using Idams.Infrastructure.EntityFramework.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Idams.Infrastructure.EntityFramework.Repositories
{
    public class EFRefTemplateDocumentRepository : BaseRepository, ITemplateDocumentRepository
    {
        public EFRefTemplateDocumentRepository(IdamsDbContext dbContext, IConfiguration configuration, ICurrentUserService currentUserService)
            : base(configuration, currentUserService, dbContext)
        {
        }

        public async Task<bool> DeleteDocGroup(string docGroupParId, int docGroupVersion)
        {
            var docGroup = await _dbContext.MdDocumentGroups.AsNoTracking().Where(n => n.DocGroupParId == docGroupParId && n.DocGroupVersion == docGroupVersion).SingleOrDefaultAsync();
            _dbContext.MdDocumentGroups.Remove(docGroup);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteRefTemplateDocument(RefTemplateDocument refTemplateDocument)
        {
            _dbContext.RefTemplateDocuments.Remove(refTemplateDocument);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<RefTemplateDocument?> GetTemplateDocument(string workflowSequenceId, string workflowActionId)
        {
            return await _dbContext.RefTemplateDocuments.AsNoTracking().Where(n => n.WorkflowSequenceId == workflowSequenceId &&
                n.WorkflowActionId == workflowActionId).FirstOrDefaultAsync();
        }

        public async Task<List<RefTemplateDocument>> GetTemplateDocumentByTemplateIdAndTemplateVersion(string templateId, int templateVersion)
        {
            return await _dbContext.RefTemplateDocuments.Where(n => n.TemplateId == templateId && n.TemplateVersion == templateVersion).ToListAsync();
        }
    }
}
