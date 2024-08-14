namespace Idams.Core.Model.Filters
{
    public class LogHistoryFilter
    {
        public string? ProjectId { get; set; }
        public string? EmpName { get; set; }
        public string? WorkflowName { get; set; }
        public string? Action { get; set; }
        public string? ActivityDescription { get; set; }
        public string? LastStatus { get; set; }

    }
}
