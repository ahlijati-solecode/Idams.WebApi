namespace Idams.Core.Model.Requests
{
    public class WorkflowSettingPagedRequest : BasePagedRequest
    {
        public string? TemplateName { get; set; }
        public string? ProjectCategory { get; set; }
        public string? ProjectCriteria { get; set; }
        public string? ProjectSubCriteria { get; set; }
        public string? Threshold { get; set; }
        public string? CapexValue { get; set; }
        public int? TotalWorkflow { get; set; }
        public string? StartWorkFlow { get; set; }
        public string? EndWorkFlow { get; set; }
        public string? DateModified { get; set; }
    }
}
