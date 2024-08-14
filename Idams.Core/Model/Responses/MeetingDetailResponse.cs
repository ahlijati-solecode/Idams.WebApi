namespace Idams.Core.Model.Responses
{
    public class MeetingDetailResponse
    {
        public string? Title { get; set; }
        public DateTime? Date { get; set; }
        public TimeSpan? Start { get; set; }
        public TimeSpan? End { get; set; }
        public string? Location { get; set; }
        public string? Notes { get; set; }
        public string? Threshold { get; set; }
        public string? Regional { get; set; }
        public string? Zona { get; set; }
        public string? CreatedBy { get; set; }
        public List<MeetingParticipantResponse> Participants { get; set; } = new List<MeetingParticipantResponse>();
    }

    public class MeetingParticipantResponse
    {
        public string? EmpAccount { get; set; }
        public string? EmpName { get; set; }
        public string? EmpEmail { get; set; }
    }
}
