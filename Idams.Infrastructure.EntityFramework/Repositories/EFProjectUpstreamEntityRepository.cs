using Idams.Core.Model.Entities;
using Idams.Core.Repositories;
using Idams.Core.Services;
using Idams.Infrastructure.EntityFramework.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Idams.Infrastructure.EntityFramework.Repositories
{
    public class EFProjectUpstreamEntityRepository : BaseRepository, IProjectUpstreamEntityRepository
    {
        public EFProjectUpstreamEntityRepository(IdamsDbContext dbContext, IConfiguration configuration, ICurrentUserService currentUserService)
            : base(configuration, currentUserService, dbContext)
        {

        }

        public async Task<int> SaveProjectUpstreamEntity(string projectId, List<string> EntitiesId)
        {
            if (EntitiesId == null || EntitiesId.Count == 0)
                throw new InvalidDataException("Must include at least 1 entity");
            var oldEntities = await _dbContext.TxProjectUpstreamEntities.Where(n => n.ProjectId == projectId).ToListAsync();
            if(oldEntities.Count > 0)
            {
                _dbContext.TxProjectUpstreamEntities.RemoveRange(oldEntities);
                await _dbContext.SaveChangesAsync();
            }
            List<TxProjectUpstreamEntity> newEntities = new();
            foreach(var entityId in EntitiesId)
            {
                TxProjectUpstreamEntity newEntity = new()
                {
                    ProjectId = projectId,
                    EntityId = entityId
                };
                newEntities.Add(newEntity);
            }
            await _dbContext.TxProjectUpstreamEntities.AddRangeAsync(newEntities);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<List<TxProjectUpstreamEntity>> GetProjectUpstreamEntities(string projectId)
        {
            return await _dbContext.TxProjectUpstreamEntities.AsNoTracking().Where(n => n.ProjectId == projectId).ToListAsync();
        }
    }
}
