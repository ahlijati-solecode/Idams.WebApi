namespace Idams.Core.Model.Dtos
{
    public class WorkflowSettingPagedDto
    {
        public string TemplateId { get; set; } = null!;
        public string TemplateVersion { get; set; } = null!;
        public string? TemplateName { get; set; }
        public string? ProjectCategory { get; set; }
        public string? ProjectCriteria { get; set; }
        public string? ProjectSubCriteria { get; set; }
        public string? Threshold { get; set; }
        public string? CapexValue { get; set; }
        public int? TotalWorkflow { get; set; }
        public string? StartWorkflow { get; set; }
        public string? EndWorkflow { get; set; }
        public string Status { get; set; } = null!;
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        

    }
}
