using Idams.Core.Model.Entities;

namespace Idams.Core.Repositories
{
    public interface IProjectPipelineRepository
    {
        Task<List<TxProjectPipeline>> UpdatedAsync(string projectId, int projectVersion, List<TxProjectPipeline> pipeline);
        Task<List<TxProjectPipeline>> GetPipelineAsync(string projectId, int projectVersion);
        Task<List<TxProjectPipeline>> AddPipelineAsync(List<TxProjectPipeline> pipelines);
    }
}
