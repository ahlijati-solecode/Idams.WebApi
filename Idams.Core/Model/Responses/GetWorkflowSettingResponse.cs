namespace Idams.Core.Model.Responses
{
    public class GetWorkflowSettingResponse
    {
        public string? TemplateId { get; set; }
        public int? TemplateVersion { get; set; }
        public string? TemplateName { get; set; }
        public string? ProjectCategory { get; set; }
        public string? ProjectCriteria { get; set; }
        public string? ProjectSubCriteria { get; set; }
        public string? Threshold { get; set; }
        public bool? IsActive { get; set; }
        public int? TotalWorkflow { get; set; }
        public string? Status { get; set; }
        public List<RefTemplateWorkflowSequenceResponse> WorkflowSequence { get; set; }

        public GetWorkflowSettingResponse(List<RefTemplateWorkflowSequenceResponse> workflowSequence)
        {
            WorkflowSequence = workflowSequence;
        }
    }
}
