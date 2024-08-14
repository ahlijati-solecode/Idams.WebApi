namespace Idams.Core.Model.Dtos
{
    public class GetMilestoneDto
    {
        public string? TemplateName { get; set; }
        public List<MilestoneDto> Milestone { get; set; } = new List<MilestoneDto>();
    }

    public class MilestoneDto
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
