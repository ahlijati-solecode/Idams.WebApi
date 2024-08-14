using Idams.Core.Model.Entities;

namespace Idams.Core.Repositories
{
    public interface IProjectCompressorRepository
    {
        Task<List<TxProjectCompressor>?> UpdateAsync(string projectId, int projectVersion, List<TxProjectCompressor> projectCompressors);
        Task<List<TxProjectCompressor>> GetCompressorsAsync(string projectId, int projectVersion);
        Task<List<TxProjectCompressor>> AddCompressorAsync(List<TxProjectCompressor> compressors);
    }

}
