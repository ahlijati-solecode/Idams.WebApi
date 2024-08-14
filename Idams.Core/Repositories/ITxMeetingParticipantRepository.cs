using Idams.Core.Model.Dtos;
using Idams.Core.Model.Entities;

namespace Idams.Core.Repositories
{
    public interface ITxMeetingParticipantRepository
    {
        Task<List<TxMeetingParticipant>> AddAsync(List<TxMeetingParticipant> txMeetingParticipants);
        Task<TxMeetingParticipant> GetByEmail(string email);
        Task<List<TxMeetingParticipant>> GetListAsync(string projectaActionId, int meetingId);
        Task<bool> DeleteAsync(List<TxMeetingParticipant> txMeetingParticipants);
        Task<List<MeetingWithProjectAndStageDto>> GetCalendarEvent(DateTime startDate, DateTime endDate, string? projectId, string? threshold);
    }
}
