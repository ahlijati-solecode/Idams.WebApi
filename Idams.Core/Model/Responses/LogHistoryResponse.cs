namespace Idams.Core.Model.Responses
{
    public class LogHistoryResponse
    {
        public string? EmpName { get; set; }
        public string? WorkflowName { get; set; }
        public string? Action { get; set; }
        public string? ActivityDescription { get; set; }
        public string? WorkflowActionType { get; set; }
        public string? LastStatus { get; set; }
        public string? DocumentGroupParId { get; set; }
        public DateTime? DateModified { get; set; }

    }
}
