namespace Idams.Core.Model.Responses
{
    public class RefTemplateWorkflowSequenceResponse
    {
        public string? WorkflowSequenceId { get; set; }
        public string? TemplateId { get; set; }
        public int? TemplateVersion { get; set; }
        public string? WorkflowId { get; set; }
        public string? WorkflowName { get; set; }
        public string? WorkflowType { get; set; }
        public string? WorkflowCategory { get; set; }
        public int? Order { get; set; }
        public int? Sla { get; set; }
        public string? SlauoM { get; set; }
        public bool? WorkflowIsOptional { get; set; }
        public List<MpDocumentChecklistResponse> Documents { get; set; }

        public RefTemplateWorkflowSequenceResponse(List<MpDocumentChecklistResponse> documents)
        {
            Documents = documents;
        }
    }
}
