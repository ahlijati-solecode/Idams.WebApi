using System.Data.Entity;
using Idams.Core.Constants;
using Idams.Core.Model.Dtos;
using Idams.Core.Repositories;
using Idams.Core.Services;
using Idams.Infrastructure.EntityFramework.Core;
using Microsoft.Extensions.Configuration;

namespace Idams.Infrastructure.EntityFramework.Repositories
{
    public class EFUpcomingMeetingRepository : BaseRepository, IUpcomingMeetingRepository
    {
        private readonly IProjectRepository _projectRepository;

        public EFUpcomingMeetingRepository(
            IConfiguration configuration,
            ICurrentUserService currentService,
            IdamsDbContext dbContext,
            IProjectRepository projectRepository) : base(configuration, currentService, dbContext)
        {
            _projectRepository = projectRepository;
        }

        public async Task<List<UpcomingMeetingDto>> GetList(string? projectId)
        {
            List<UpcomingMeetingDto> item = new List<UpcomingMeetingDto>();

            DateTime now = DateTime.Now;

            List<UpcomingMeetingDto>? meetingParticipants =
                _dbContext.TxMeetingParticipants
                    .AsNoTracking()
                    .Where(e =>
                        e.EmpAccount == GetCurrentUserInfo.EmpAccount &&
                        (
                            string.IsNullOrEmpty(projectId) ?
                                true :
                                e.TxMeeting.ProjectAction.ProjectId == projectId
                        ) &&
                        (
                            e.TxMeeting.Date > now.Date ||
                            (e.TxMeeting.Date == now.Date && e.TxMeeting.End > now.TimeOfDay)
                        ) && e.TxMeeting.MeetingStatusParId == StatusConstant.Scheduled && e.TxMeeting.Deleted == false
                    )
                    .OrderBy(e => e.TxMeeting.Date)
                    .ThenBy(e => e.TxMeeting.Start)
                    .Take(3)
                    .Include(e => e.TxMeeting)
                    .Select(e => new UpcomingMeetingDto
                    {
                        MeetingName = e.TxMeeting.Title,
                        ProjectName = e.TxMeeting.ProjectAction.ProjectId,
                        Date = e.TxMeeting.Date,
                        Start = e.TxMeeting.Start,
                        End = e.TxMeeting.End,
                    }).ToList();

            List<UpcomingMeetingDto>? upcomingMeetings = new List<UpcomingMeetingDto>();

            foreach (UpcomingMeetingDto? e in meetingParticipants)
            {
                Idams.Core.Model.Entities.TxProjectHeader? projectHeader = e.ProjectName == null ? null : await _projectRepository.GetLatestVersionProjectHeader(e.ProjectName);

                upcomingMeetings.Add(new UpcomingMeetingDto
                {
                    ProjectName = projectHeader == null ? "" : projectHeader.ProjectName,
                    MeetingName = e.MeetingName,
                    Date = e.Date,
                    Start = e.Start,
                    End = e.End,
                }) ;
            }

            return upcomingMeetings;
        }
    }
}

