namespace Idams.Core.Model.Dtos
{
    public class RefTemplateWorkflowSequenceDto
    {
        public string? TemplateId { get; set; }
        public int? TemplateVersion { get; set; }
        public string? WorkflowId { get; set; }
        public string? WorkflowName { get; set; }
        public string? WorkflowType { get; set; }
        public string? WorkflowCategory { get; set; }
        public string? WorkflowSequenceId { get; set; }
        public int? Order { get; set; }
        public int? Sla { get; set; }
        public string? SlauoM { get; set; }
        public bool? WorkflowIsOptional { get; set; }
        public List<RefTemplateDocumentDto>? TemplateDocumentDtos { get; set; }
        public List<MpDocumentChecklistDto> Documents { get; set; }  

        public RefTemplateWorkflowSequenceDto()
        {
            TemplateDocumentDtos = new List<RefTemplateDocumentDto>();
        }
    }
}
