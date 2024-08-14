namespace Idams.Core.Model.Requests
{
    public class LogHistoryPagedRequest : BasePagedRequest
    {
        public string? ProjectId { get; set; }
        public string? EmpName { get; set; }
        public string? WorkflowName { get; set; }
        public string? Action { get; set; }
        public string? ActivityDescription { get; set; }
        public string? LastStatus { get; set; }
    }
}
