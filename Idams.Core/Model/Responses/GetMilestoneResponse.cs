namespace Idams.Core.Model.Responses
{
    public class GetMilestoneResponse
    {
        public string? TemplateName { get; set; }
        public List<MilestoneResponse> Milestone { get; set; } = new List<MilestoneResponse>();
    }
    public class MilestoneResponse
    {
        public string? WorkflowSequenceId { get; set; }
        public string? WorkflowName { get; set; }
        public string? WorkflowType { get; set; }
        public string? WorkflowCategory { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Sla { get; set; }
        public string? Done { get; set; }
    }
}
