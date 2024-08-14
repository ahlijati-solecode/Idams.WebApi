namespace Idams.Core.Model.Requests
{
    public class CalendarEventRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? ProjectId { get; set; }
        public string? Threshold { get; set; }
    }
}
