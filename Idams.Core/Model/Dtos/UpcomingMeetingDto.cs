using System;
namespace Idams.Core.Model.Dtos
{
    public class UpcomingMeetingDto
    {
        public string? MeetingName { get; set; }
        public string? ProjectName { get; set; }
        public DateTime? Date { get; set; }
        public TimeSpan? Start { get; set; }
        public TimeSpan? End { get; set; }
    }
}

