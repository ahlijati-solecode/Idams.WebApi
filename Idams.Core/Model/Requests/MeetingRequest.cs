namespace Idams.Core.Model.Requests
{
    public class MeetingRequest
    {
        public string? ProjectActionId { get; set; }
        public int? MeetingId { get; set; }
        public string? Title { get; set; }
        public DateTime? Date { get; set; }
        public TimeSpan? Start { get; set; }
        public TimeSpan? End { get; set; }
        public string? Location { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }
        public List<MeetingParticipantRequest> Participants { get; set; } = new List<MeetingParticipantRequest>();
    }

    public class MeetingParticipantRequest
    {
        public string? EmpAccount { get; set; }
        public string? EmpName { get; set; }
        public string? EmpEmail { get; set; }
    }
}
