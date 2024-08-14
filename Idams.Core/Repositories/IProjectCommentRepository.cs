using Idams.Core.Model.Entities;

namespace Idams.Core.Repositories
{
    public interface IProjectCommentRepository
    {
        Task<Paged<TxProjectComment>> GetCommentListAsync(string projectId);
        Task<TxProjectComment> AddCommentAsync(TxProjectComment entity);
        Task<bool> DeleteCommentAsync(TxProjectComment comment);
        Task<int?> GetLatestProjectCommentId(string projectId);
        Task <TxProjectComment?> GetCommentAsync(string projectId, int projectCommentId); 
    }
}
