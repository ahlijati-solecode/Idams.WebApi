
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
    public class EFProjectCompressorRepository : BaseRepository, IProjectCompressorRepository
    {
        public EFProjectCompressorRepository(IConfiguration configuration, ICurrentUserService currentService, IdamsDbContext dbContext) : base(configuration, currentService, dbContext)
        {
        }

        public async Task<List<TxProjectCompressor>?> UpdateAsync(string projectId, int projectVersion, List<TxProjectCompressor> projectCompressors)
        {
            var item = await _dbContext.TxProjectCompressors.Where(n => n.ProjectId == projectId && n.ProjectVersion == projectVersion).ToListAsync();

            _dbContext.TxProjectCompressors.RemoveRange(item);
            await _dbContext.SaveChangesAsync();
            List<TxProjectCompressor> txProjectCompressors = new List<TxProjectCompressor>();

            projectCompressors.ForEach(n =>
            {
                TxProjectCompressor data = new TxProjectCompressor
                {
                    ProjectCompressorId = ShortId.Generate(new GenerationOptions(true, false, 20)),
                    ProjectId = projectId,
                    ProjectVersion = projectVersion,
                    CompressorTypeParId = n.CompressorTypeParId,
                    CompressorCapacity = n.CompressorCapacity,
                    CompressorCapacityUoM = n.CompressorCapacityUoM,
                    CompressorCount = n.CompressorCount,
                    CompressorDischargePressure = n.CompressorDischargePressure,
                    CompressorDischargePressureUoM = "psig"
                };
                txProjectCompressors.Add(data);
            });
            await _dbContext.TxProjectCompressors.AddRangeAsync(txProjectCompressors);
            await _dbContext.SaveChangesAsync();
            return txProjectCompressors;

        }

        public async Task<List<TxProjectCompressor>> GetCompressorsAsync(string projectId, int projectVersion)
        {
            return await _dbContext.TxProjectCompressors.AsNoTracking().Where(n => n.ProjectId == projectId && n.ProjectVersion == projectVersion).ToListAsync();
        }

        public async Task<List<TxProjectCompressor>> AddCompressorAsync(List<TxProjectCompressor> compressors)
        {
            await _dbContext.AddRangeAsync(compressors);
            await _dbContext.SaveChangesAsync();
            return compressors;
        }
    }
}
