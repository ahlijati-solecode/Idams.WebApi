using Idams.Core.Model.Entities;
using Idams.Core.Repositories;
using Idams.Core.Services;
using Idams.Infrastructure.EntityFramework.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using shortid;
using shortid.Configuration;

namespace Idams.Infrastructure.EntityFramework.Repositories
{
    public class EFProjectPlatformRepository : BaseRepository, IProjectPlatformRepository
    {

        public EFProjectPlatformRepository(IdamsDbContext dbContext, IConfiguration configuration, ICurrentUserService currentUserService)
            : base(configuration, currentUserService, dbContext)
        {
        }

        public async Task<List<TxProjectPlatform>> UpdateAsync(string projectId, int projectVersion, List<TxProjectPlatform> platforms)
        {
            var item = await _dbContext.TxProjectPlatforms.Where(n => n.ProjectId == projectId && n.ProjectVersion == projectVersion).ToListAsync();

            _dbContext.TxProjectPlatforms.RemoveRange(item);
            await _dbContext.SaveChangesAsync();
            List<TxProjectPlatform> txProjectPlatforms = new List<TxProjectPlatform>();
            platforms.ForEach(n =>
            {
                TxProjectPlatform data = new TxProjectPlatform
                {
                    ProjectPlateformId = ShortId.Generate(new GenerationOptions(true, false, 20)),
                    ProjectId = projectId,
                    ProjectVersion = projectVersion,
                    PlatformCount = n.PlatformCount,
                    PlatformLegCount = n.PlatformLegCount
                };
                txProjectPlatforms.Add(data);
            });

            await _dbContext.TxProjectPlatforms.AddRangeAsync(txProjectPlatforms);
            await _dbContext.SaveChangesAsync();
            return txProjectPlatforms;
        }

        public async Task<List<TxProjectPlatform>> GetPlatformsAsync(string projectId, int projectVersion)
        {
            return await _dbContext.TxProjectPlatforms.AsNoTracking().Where(n => n.ProjectId == projectId && n.ProjectVersion == projectVersion).ToListAsync();
        }

        public async Task<List<TxProjectPlatform>> AddPlatformAsync(List<TxProjectPlatform> platforms)
        {
            await _dbContext.TxProjectPlatforms.AddRangeAsync(platforms);
            await _dbContext.SaveChangesAsync();
            return platforms;
        }

    }
}
