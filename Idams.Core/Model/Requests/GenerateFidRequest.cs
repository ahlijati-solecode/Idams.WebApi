namespace Idams.Core.Model.Requests
{
    public class GenerateFidRequest
    {
        public string? SubholdingCode { get; set; }
        public string? ProjectCategory { get; set; }
        public int ApprovedYear { get; set; }
        public string? Regional { get; set; }
        public string? Fidcode { get; set; }
        public string? ProjectId { get; set; }
        public int ProjectVersion { get; set; }
    }
}
