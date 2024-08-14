using Idams.Core.Constants;
using Idams.Core.Model.Dtos;
using Idams.Core.Model.Entities;
using Idams.Core.Model.Requests;
using Idams.Core.Repositories;
using Idams.Core.Services;
using Idams.Infrastructure.EntityFramework.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Idams.Infrastructure.EntityFramework.Repositories
{
    public class EFApprovalRepository : BaseRepository, IApprovalRepository
    {
        public EFApprovalRepository(IConfiguration configuration,
            ICurrentUserService currentUserService,
            IdamsDbContext dbContext)
            : base(configuration, currentUserService, dbContext)
        {
        }

        public async Task<TxApproval> GenerateNewApproval(string projectActionId)
        {
            var latest = await _dbContext.TxApprovals.AsNoTracking().Where(n => n.ProjectActionId == projectActionId).OrderByDescending(n => n.ApprovalId).FirstOrDefaultAsync();
            TxApproval newApproval = new();
            newApproval.ProjectActionId = projectActionId;
            newApproval.ApprovalId = latest == null ? 1 : latest.ApprovalId + 1;
            newApproval.ApprovalStatusParId = StatusConstant.Inprogress;
            await _dbContext.TxApprovals.AddAsync(newApproval);
            return newApproval;
        }

        public async Task<TxApproval> UpdateApprovalData(ApprovalRequest approvalRequest)
        {
            var approval = await _dbContext.TxApprovals.Where(n => n.ProjectActionId == approvalRequest.ProjectActionId)
                .OrderByDescending(n => n.ApprovalId).FirstOrDefaultAsync();

            if (approval == null)
            {
                throw new Exception($"Approval with projectActionId {approvalRequest.ProjectActionId} not found");
            }
            if (approval.ApprovalStatusParId != StatusConstant.Inprogress)
            {
                throw new Exception($"Approval with actionId {approvalRequest.ProjectActionId} already {approval.ApprovalStatusParId}");
            }

            var userInfo = GetCurrentUserInfo;
            approval.ApprovalStatusParId = approvalRequest.Approval ? StatusConstant.Completed : StatusConstant.Revised;
            approval.Notes = approvalRequest.Notes;
            approval.EmpName = userInfo?.Name;
            approval.ApprovalDate = DateTime.UtcNow;
            approval.ApprovalBy = userInfo?.EmpAccount;
            _dbContext.TxApprovals.Update(approval);
            await _dbContext.SaveChangesAsync();
            return approval;
        }

        public async Task<TxApproval?> GetLastApprovalData(string projectId)
        {
            return await (from pa in _dbContext.TxProjectActions
                          join approval in _dbContext.TxApprovals on pa.ProjectActionId equals approval.ProjectActionId
                          where pa.ProjectId == projectId
                          orderby approval.ApprovalId descending
                          select approval).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<ApprovalDetailDto> GetApprovalDetail(string projectActionId)
        {
            var approval = await _dbContext.TxApprovals.AsNoTracking().Where(n => n.ProjectActionId == projectActionId).OrderByDescending(n => n.ApprovalId).ToListAsync();

            ApprovalDetailDto ret = new()
            {
                Status = approval[0].ApprovalStatusParId,
                Date = approval[0].ApprovalDate,
                Notes = approval[0].Notes,
                EmpName = approval[0].EmpName,
            };

            for (int i = 1; i < approval.Count; i++)
            {
                ret.ApprovalHistory.Add(new()
                {
                    Date = approval[i].ApprovalDate,
                    EmpName = approval[i].EmpName,
                    Notes = approval[i].Notes
                });
            }
            return ret;
        }

        public async Task<ConfirmationDetailDto> GetConfirmationDetail(string projectActionId)
        {
            var confirmation = await _dbContext.TxApprovals.AsNoTracking().Where(n => n.ProjectActionId == projectActionId).FirstOrDefaultAsync();
            return new ConfirmationDetailDto
            {
                EmpName = confirmation.EmpName,
                Date = confirmation.ApprovalDate,
                Status = confirmation.ApprovalStatusParId
            };
        }

        public async Task<ConfirmationDetailDto> UpdateConfirmationData(ConfirmationRequest confirmationRequest)
        {
            var confirmation = await _dbContext.TxApprovals.Where(n => n.ProjectActionId == confirmationRequest.ProjectActionId)
                .Include(n => n.ProjectAction)
                .ThenInclude(n => n.WorkflowAction)
                .FirstOrDefaultAsync();

            if (confirmation == null)
            {
                throw new Exception($"Confirmation {confirmationRequest.ProjectActionId} not found");
            }
            if (confirmation.ApprovalStatusParId != StatusConstant.Inprogress)
            {
                throw new Exception($"Approval with actionId {confirmationRequest.ProjectActionId} already {confirmation.ApprovalStatusParId}");
            }
            string? type = confirmation.ProjectAction?.WorkflowAction?.WorkflowActionTypeParId;
            if (type != WorkflowActionTypeConstant.Confirmation)
            {
                throw new Exception($"{confirmationRequest.ProjectActionId}:{type} is not {WorkflowActionTypeConstant.Confirmation} workflow");
            }

            var userInfo = GetCurrentUserInfo;
            confirmation.ApprovalStatusParId = confirmationRequest.Approval ? StatusConstant.Completed : StatusConstant.Skipped;
            confirmation.EmpName = userInfo.Name;
            confirmation.ApprovalDate = DateTime.UtcNow;
            confirmation.ApprovalBy = userInfo.EmpAccount;
            _dbContext.TxApprovals.Update(confirmation);
            confirmation.ProjectAction!.Status = confirmationRequest.Approval ? StatusConstant.Completed : StatusConstant.Skipped;
            _dbContext.TxProjectActions.Update(confirmation.ProjectAction);
            await _dbContext.SaveChangesAsync();
            return new ConfirmationDetailDto
            {
                Date = confirmation.ApprovalDate,
                EmpName = confirmation.EmpName,
                Status = confirmation.ApprovalStatusParId
            };
        }
    }
}
