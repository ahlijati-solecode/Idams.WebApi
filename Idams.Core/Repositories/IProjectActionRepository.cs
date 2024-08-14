using Idams.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Repositories
{
    public interface IProjectActionRepository
    {
        Task<TxProjectAction> AddAsync(TxProjectAction txProjectAction);
        Task<TxProjectAction> UpdateAsync(TxProjectAction txProjectAction);
        Task<TxProjectAction?> GetAction(string? projectActionId);
        Task<List<TxProjectAction>> GetProjectAction(string projectId, string sequenceId);
        Task<TxProjectAction?> GetProjectActionWithRefWorkflowAction(string? projectActionId);
        Task<TxProjectAction?> GetProjectAction(string? projectId, string? workflowSequenceId, string? workflowActionId);
        Task<List<RefWorkflowActor>> GetActorByAction(string workflowActionId);
        Task<TxProjectAction?> GetLastProjectAction(string projectId, string workfloSequeceId);
    }
}
