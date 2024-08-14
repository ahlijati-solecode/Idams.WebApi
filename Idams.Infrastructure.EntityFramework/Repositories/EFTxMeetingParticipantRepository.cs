using Idams.Core.Constants;
using Idams.Core.Enums;
using Idams.Core.Model.Dtos;
using Idams.Core.Model.Entities;
using Idams.Core.Repositories;
using Idams.Core.Services;
using Idams.Infrastructure.EntityFramework.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Idams.Infrastructure.EntityFramework.Repositories
{
    public class EFTxMeetingParticipantRepository : BaseRepository, ITxMeetingParticipantRepository
    {
        public EFTxMeetingParticipantRepository(IConfiguration configuration, ICurrentUserService currentService, IdamsDbContext dbContext) : base(configuration, currentService, dbContext)
        {
        }

        public async Task<List<TxMeetingParticipant>> AddAsync(List<TxMeetingParticipant> txMeetingParticipant)
        {
            _dbContext.TxMeetingParticipants.AddRange(txMeetingParticipant);  
            await _dbContext.SaveChangesAsync();
            return txMeetingParticipant;
        }

        public async Task<bool> DeleteAsync(List<TxMeetingParticipant> txMeetingParticipants)
        {
            _dbContext.TxMeetingParticipants.RemoveRange(txMeetingParticipants);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<TxMeetingParticipant> GetByEmail(string email)
        {
            return await _dbContext.TxMeetingParticipants.AsNoTracking().SingleAsync(n => n.EmpEmail == email);
        }

        public async Task<List<MeetingWithProjectAndStageDto>> GetCalendarEvent(DateTime startDate, DateTime endDate, string? projectId, string? threshold)
        {
            var email = GetCurrentUserInfo.Email;
            var allData = false;
            var participants = _dbContext.TxMeetingParticipants.AsNoTracking().Include(n => n.TxMeeting).ThenInclude(n => n.ProjectAction).ThenInclude(n => n.WorkflowSequence).ThenInclude(n => n!.Workflow)
                                .Where(n => n.TxMeeting.Date >= startDate && n.TxMeeting.Date <= endDate && n.TxMeeting.MeetingStatusParId != StatusConstant.Draft && n.TxMeeting.Deleted == false);
            if (GetCurrentUserInfo.Roles.Any(n => n.Enum == RoleEnum.SUPER_ADMIN || n.Enum == RoleEnum.ADMIN_SHU))
            {
                allData = true;
            }
            if (!allData)
            {
                participants = participants.Where(n => n.EmpEmail == email);
            }

            if (!string.IsNullOrWhiteSpace(projectId))
            {
                participants = participants.Where(n => n.TxMeeting.ProjectAction.ProjectId == projectId);
            }
            if (!string.IsNullOrWhiteSpace(threshold))
            {
                participants = participants.Where(n => n.TxMeeting.ProjectAction.WorkflowSequence!.Template!.ThresholdNameParId == threshold);
            }
            return await participants.Select(n => new MeetingWithProjectAndStageDto
            {
                MeetingId = n.MeetingId,
                ProjectActionId = n.ProjectActionId,
                ProjectName = _dbContext.TxProjectHeaders.Where(e => e.ProjectId == n.TxMeeting.ProjectAction.ProjectId).SingleOrDefault()!.ProjectName,
                Stage = _dbContext.MdParamaterLists.AsNoTracking().Where(e => e.Schema == "idams" && e.ParamId == "Stage" &&
                        e.ParamListId == n.TxMeeting.ProjectAction.WorkflowSequence!.Workflow!.WorkflowCategoryParId).SingleOrDefault()!.ParamValue1Text,
                Title = n.TxMeeting.Title,
                Date = n.TxMeeting.Date,
                Start = n.TxMeeting.Start,
                End = n.TxMeeting.End,
                Status = n.TxMeeting.MeetingStatusParId
            }).ToListAsync();                
        }

        public async Task<List<TxMeetingParticipant>> GetListAsync(string projectaActionId, int meetingId)
        {
            return await _dbContext.TxMeetingParticipants.AsNoTracking().Where(n => n.ProjectActionId == projectaActionId &&
            n.MeetingId == meetingId).ToListAsync();
        }
    }
}
