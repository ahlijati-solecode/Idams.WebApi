namespace Idams.Core.Model.Responses
{
    public class ProjectCommentResponse
    {
        public string? ProjectId { get; set; }
        public int? ProjectCommentId { get; set; }
        public string? ProjectActionId { get; set; }
        public string? Comment { get; set; }
        public string? Stage { get; set; }
        public string? Workflow { get; set; }
        public string? EmpName { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

    }
}
