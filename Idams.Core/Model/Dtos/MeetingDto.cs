namespace Idams.Core.Model.Dtos
{
    public class MeetingDto
    {
        public int? MeetingId { get; set; }
        public string? ProjectActionId { get; set; }
        public string? Title { get; set; }
        public DateTime? Date { get; set; }
        public TimeSpan? Start { get; set; }
        public TimeSpan? End { get; set; }
        public string? Status { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class MeetingWithProjectAndStageDto : MeetingDto
    {
        public string? ProjectName { get; set; }
        public string? Stage { get; set; }
    }
}
