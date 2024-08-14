namespace Idams.Core.Model.Requests
{
    public class AddWorkflowSettingRequest
    {
        public string? TemplateId { get; set; }
        public int? TemplateVersion { get; set; }
        public string? TemplateName { get; set; }
        public string? ProjectCategory { get; set; }
        public string? ProjectCriteria { get; set; }
        public string? ProjectSubCriteria { get; set; }
        public string? Threshold { get; set; }
        public string? Status { get; set; }
        public List<RefTemplateWorkflowSequenceRequest>? WorkflowSequence { get; set; }
    }
}
