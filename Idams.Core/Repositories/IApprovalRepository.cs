using Idams.Core.Model.Dtos;
using Idams.Core.Model.Entities;
using Idams.Core.Model.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Repositories
{
    public interface IApprovalRepository
    {
        Task<TxApproval> GenerateNewApproval(string projectActionId);
        Task<ApprovalDetailDto> GetApprovalDetail(string projectActionId);
        Task<TxApproval> UpdateApprovalData(ApprovalRequest approvalRequest);
        Task<TxApproval?> GetLastApprovalData(string projectId);
        Task<ConfirmationDetailDto> GetConfirmationDetail(string projectActionId);
        Task<ConfirmationDetailDto> UpdateConfirmationData(ConfirmationRequest confirmationRequest);
    }
}
