using Idams.Core.Constants;
using Idams.Core.Model.Entities;
using Idams.Core.Repositories;
using Idams.Core.Services;
using Idams.Infrastructure.EntityFramework.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Idams.Infrastructure.EntityFramework.Repositories
{
    public class EFTxMeetingRepositories : BaseRepository, ITxMeetingRepositories
    {
        public EFTxMeetingRepositories(IConfiguration configuration, ICurrentUserService currentService, IdamsDbContext dbContext) : base(configuration, currentService, dbContext)
        {
        }

        public async Task<List<TxMeeting>> GetListAsync(string projectActionId, bool deleted)
        {
            List<TxMeeting> item = new List<TxMeeting>();
            if (deleted)
            {
                item = await _dbContext.TxMeetings.AsNoTracking().Where(n => n.ProjectActionId == projectActionId).ToListAsync();
            }
            else
            {
                item = await _dbContext.TxMeetings.AsNoTracking().Where(n => n.ProjectActionId == projectActionId && n.Deleted == false).ToListAsync();
            }
            return item.ToList();
        }

        public async Task<List<TxMeeting>> GetListWithoutDraftAsync(string projectActionId)
        {
            var item = await _dbContext.TxMeetings.AsNoTracking().Where(n => n.ProjectActionId == projectActionId && (n.MeetingStatusParId == StatusConstant.Completed
            || n.MeetingStatusParId == StatusConstant.Canceled || n.MeetingStatusParId == StatusConstant.Scheduled) && n.Deleted == false).ToListAsync();
            return item.ToList();
        }

        public async Task<TxMeeting> GetMeetingWithParticipant(string projectActionId, int meetingId)
        {
            var item = await _dbContext.TxMeetings.AsNoTracking().Include(n => n.ProjectAction)
                .Include(n => n.TxMeetingParticipants).Where(n => n.ProjectActionId == projectActionId && n.MeetingId == meetingId).SingleAsync();
            return item;
        }

        public async Task<TxMeeting?> GetMeeting(string projectActionId, int meetingId)
        {
            return await _dbContext.TxMeetings.Include(n => n.TxMeetingParticipants).SingleAsync(n => n.ProjectActionId == projectActionId && n.MeetingId == meetingId && n.Deleted == false);
        }
        public async Task<TxMeeting> AddMeetingAsync(TxMeeting meeting)
        {
            meeting.CreatedBy = GetCurrentUser;
            meeting.CreatedDate = DateTime.UtcNow;
            meeting.Deleted = false;
            _dbContext.TxMeetings.Add(meeting);
            await _dbContext.SaveChangesAsync();
            return meeting;
        }

        public async Task<TxMeeting> UpdateMeetingAsync(TxMeeting meeting)
        {
            _dbContext.TxMeetings.Update(meeting);
            await _dbContext.SaveChangesAsync();
            return meeting;
        }
    }
}
