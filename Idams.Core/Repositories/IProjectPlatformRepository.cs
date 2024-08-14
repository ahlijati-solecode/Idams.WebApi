using Idams.Core.Model.Entities;

namespace Idams.Core.Repositories
{
    public interface IProjectPlatformRepository
    {
        Task<List<TxProjectPlatform>> UpdateAsync(string projectId, int projectVersion, List<TxProjectPlatform> platforms);
        Task<List<TxProjectPlatform>> GetPlatformsAsync(string projectId, int projectVersion);
        Task<List<TxProjectPlatform>> AddPlatformAsync(List<TxProjectPlatform> platforms);  
    }
}
