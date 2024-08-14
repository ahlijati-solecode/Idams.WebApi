namespace Idams.Core.Model.Responses
{
    public class DocumentManagementPreviewResponse
    {
        public string? TransactionDocId { get; set; }
        public string? FileName { get; set; }
        public string? DocType { get; set; }
        public string? ProjectName { get; set; }
        public string? WorkflowType { get; set; }
        public string? Threshold { get; set; }
        public string? Regional { get; set; }
        public string? Zona { get; set; }
        public string? FileExtension { get; set; }
        public DateTime? LastModified { get; set; }
    }
}
