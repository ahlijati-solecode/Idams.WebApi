using System;
using System.Collections.Generic;

namespace Idams.Core.Model.Entities
{
    public partial class TxMeeting
    {
        public TxMeeting()
        {
            TxMeetingParticipants = new HashSet<TxMeetingParticipant>();
        }

        public int MeetingId { get; set; }
        public string ProjectActionId { get; set; } = null!;
        public string? Title { get; set; }
        public DateTime? Date { get; set; }
        public TimeSpan? Start { get; set; }
        public TimeSpan? End { get; set; }
        public string? Location { get; set; }
        public string? MeetingStatusParId { get; set; }
        public string? Notes { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public bool? Deleted { get; set; }

        public virtual TxProjectAction ProjectAction { get; set; } = null!;
        public virtual ICollection<TxMeetingParticipant> TxMeetingParticipants { get; set; }
    }
}
