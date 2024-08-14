namespace Idams.Core.Model.Dtos
{
    public class UpdateMilestoneDto
    {
        public string? ProjectId { get; set; }
        public int? ProjectVersion { get; set; }
        public string? Section { get; set; }
        public bool? SaveLog { get; set; }
        public List<MilestoneDto> Milestone { get; set; } = new List<MilestoneDto>();
    }
}
