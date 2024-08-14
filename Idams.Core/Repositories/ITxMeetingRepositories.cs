using Idams.Core.Model.Entities;

namespace Idams.Core.Repositories
{
    public interface ITxMeetingRepositories
    {
        Task<List<TxMeeting>> GetListAsync(string projectActionId, bool deleted);
        Task<List<TxMeeting>> GetListWithoutDraftAsync(string projectActionId);
        Task<TxMeeting> GetMeetingWithParticipant(string projectActionId, int meetingId);
        Task<TxMeeting?> GetMeeting(string projectActionId, int meetingId);
        Task<TxMeeting> AddMeetingAsync(TxMeeting meeting);
        Task<TxMeeting> UpdateMeetingAsync(TxMeeting meeting);
    }
}
