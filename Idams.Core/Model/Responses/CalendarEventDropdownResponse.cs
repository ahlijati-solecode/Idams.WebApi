namespace Idams.Core.Model.Responses
{
    public class CalendarEventDropdownResponse
    {
        public Dictionary<string, string> Project { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Threshold { get; set; } = new Dictionary<string, string>();
    }
}
