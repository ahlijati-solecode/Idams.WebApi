using Idams.Core.Model.Entities;
using Idams.Core.Repositories;
using Idams.Core.Services;
using Idams.Infrastructure.EntityFramework.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Idams.Infrastructure.EntityFramework.Repositories
{
    public class EFProjectCommentRepository : BaseRepository, IProjectCommentRepository
    {
        public EFProjectCommentRepository(IConfiguration configuration, ICurrentUserService currentService, IdamsDbContext dbContext) : base(configuration, currentService, dbContext)
        {
        }

        public async Task<TxProjectComment> AddCommentAsync(TxProjectComment entity)
        {
            entity.EmpName = GetCurrentUserInfo.Name;
            entity.CreatedBy = GetCurrentUser;
            entity.CreatedDate = DateTime.Now;
            _dbContext.Add(entity); 
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteCommentAsync(TxProjectComment comment)
        {
            _dbContext.Remove(comment);
            await _dbContext.SaveChangesAsync();    
            return true;
        }

        public async Task<TxProjectComment?> GetCommentAsync(string projectId, int projectCommentId)
        {
            return await _dbContext.TxProjectComments.AsNoTracking().
                SingleOrDefaultAsync(n => n.ProjectId == projectId && n.ProjectCommentId == projectCommentId);
        }

        public async Task<Paged<TxProjectComment>> GetCommentListAsync(string projectId)
        {
            var item = await _dbContext.TxProjectComments.AsNoTracking().Where(n => n.ProjectId == projectId).OrderByDescending(n => n.CreatedDate).ToListAsync();
            return new Paged<TxProjectComment>
            {
                Items = item,
                TotalItems = item.Count
            };
        }

        public async Task<int?> GetLatestProjectCommentId(string projectId)
        {
            var comment = await _dbContext.TxProjectComments.AsNoTracking().Where(n => n.ProjectId == projectId)
                .OrderByDescending(n => n.ProjectCommentId).FirstOrDefaultAsync();
            if (comment == null) return null;
            return comment?.ProjectCommentId;
        }
    }
}
