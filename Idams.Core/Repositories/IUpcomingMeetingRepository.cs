using Idams.Core.Model.Dtos;

namespace Idams.Core.Repositories
{
    public interface IUpcomingMeetingRepository
    {
        Task<List<UpcomingMeetingDto>> GetList(string? projectId);
    }
}

