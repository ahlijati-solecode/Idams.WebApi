namespace Idams.Core.Model.Dtos
{
    public class CalendarEventDropdownDto
    {
        public Dictionary<string, string> Project { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Threshold { get; set; } = new Dictionary<string, string>();
    }
}
