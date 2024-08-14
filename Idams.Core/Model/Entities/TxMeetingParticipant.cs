using System;
using System.Collections.Generic;

namespace Idams.Core.Model.Entities
{
    public partial class TxMeetingParticipant
    {
        public int MeetingId { get; set; }
        public string ProjectActionId { get; set; } = null!;
        public string EmpEmail { get; set; } = null!;
        public string? EmpAccount { get; set; }
        public string? EmpName { get; set; }

        public virtual TxMeeting TxMeeting { get; set; } = null!;
    }
}
