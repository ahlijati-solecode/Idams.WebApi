namespace Idams.Core.Model.Responses
{
    public class DocumentManagementResponse
    {
        public string? TransactionDocId { get; set; }
        public string? FileName { get; set; }
        public string? DocType { get; set; }
        public string? DocCategory { get; set; }
        public string? ProjectName { get; set; }
        public string? Regional { get; set; }
        public string? Zona { get; set; }
        public string? Threshold { get; set; }
        public string? Stage { get; set; }
        public int? RKAP { get; set; }
        public bool? Revision { get; set; }
        public string? WorkflowType { get; set; }
        public int? FileSize { get; set; }
        public string? UploadBy { get; set; }
        public string? DateModified { get; set; }
        public string? FileExtension { get; set; }
    }
}
