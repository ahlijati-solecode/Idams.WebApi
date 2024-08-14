using Idams.Core.Model.Entities;
using Idams.Core.Repositories;
using Idams.Core.Services;
using Idams.Infrastructure.EntityFramework.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using shortid;
using shortid.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Infrastructure.EntityFramework.Repositories
{
    public class EFProjectPipelineRepository : BaseRepository, IProjectPipelineRepository
    {
        public EFProjectPipelineRepository(IConfiguration configuration, ICurrentUserService currentService, IdamsDbContext dbContext) : base(configuration, currentService, dbContext)
        {
        }

        public async Task<List<TxProjectPipeline>> UpdatedAsync(string projectId, int projectVersion, List<TxProjectPipeline> pipeline)
        {
            var item = await _dbContext.TxProjectPipelines.Where(n => n.ProjectId == projectId && n.ProjectVersion == projectVersion).ToListAsync();

            _dbContext.TxProjectPipelines.RemoveRange(item);
            await _dbContext.SaveChangesAsync();
            List<TxProjectPipeline> pipelines = new List<TxProjectPipeline>();
            pipeline.ForEach(n =>
            {
                TxProjectPipeline data = new TxProjectPipeline
                {
                    ProjectPipelineId = ShortId.Generate(new GenerationOptions(true, false, 20)),
                    ProjectId = projectId,
                    ProjectVersion = projectVersion,
                    FieldServiceParId = n.FieldServiceParId,
                    PipelineCount = n.PipelineCount,
                    PipelineLenght = n.PipelineLenght,
                    PipelineLenghtUoM = "km"
                };
                pipelines.Add(data);
            });
            await _dbContext.TxProjectPipelines.AddRangeAsync(pipelines);
            await _dbContext.SaveChangesAsync();
            return pipelines;

        }

        public async Task<List<TxProjectPipeline>> GetPipelineAsync(string projectId, int projectVersion)
        {
            return await _dbContext.TxProjectPipelines.AsNoTracking().Where(n => n.ProjectId == projectId && n.ProjectVersion == projectVersion).ToListAsync();
        }

        public async Task<List<TxProjectPipeline>> AddPipelineAsync(List<TxProjectPipeline> pipelines)
        {
            await _dbContext.TxProjectPipelines.AddRangeAsync(pipelines);
            await _dbContext.SaveChangesAsync();
            return pipelines;
        }
    }
}
