using Idams.Core.Model.Dtos;
using Idams.Core.Model.Entities;
using Idams.Core.Model.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Repositories
{
    public interface IRefWorkflowRepository
    {
        Task<List<RefWorkflowDto>> GetList(string? CategoryParId = null);
        Task<RefTemplateWorkflowSequence> SaveWorkflowSequence(RefTemplateWorkflowSequence data);
        Task<TemplateWorkflowSeqDto?> GetWorkflowSequence(string workflowSequenceId);
        Task<List<RefWorkflow>> GetFirstRefWorkflow();
        Task<RefWorkflow> GetByWorkflowId(string workflowId);
        Task<List<RefWorkflowAction>> GetListWorkflowAction(string workflowId);
        Task<List<RefWorkflowAction>> GetListWorkflowAction(string workflowId, bool? isOptional);
        Task<List<RefWorkflowAction>> GetListWorkflowActionWithCard(string workflowId, bool? isOptional);
        Task<RefWorkflow?> GetByWorkflowSequenceId(string workflowSequenceId);
        Task<List<RefWorkflow>> GetWorkflowByStage(string stage);
    }
}
