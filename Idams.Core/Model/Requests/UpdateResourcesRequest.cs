namespace Idams.Core.Model.Requests
{
    public class UpdateResourcesRequest
    {
        public string ProjectId { get; set; } = null!;
        public int ProjectVersion { get; set; }
        public string? Section { get; set; }
        public Decimal? Oil { get; set; }
        public Decimal? Gas { get; set; }
        public Decimal? OilEquivalent { get; set; }
        public bool? SaveLog { get; set; }
    }
}
