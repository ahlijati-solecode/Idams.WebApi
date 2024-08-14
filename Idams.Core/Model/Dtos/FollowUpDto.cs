using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Model.Dtos
{
    public class FollowUpDtoWithSum
    {
        public int TotalData { get; set; }
        public int TotalAspect { get; set; }
        public List<FollowUpDto> Dtos { get; set; } = new List<FollowUpDto>();
    }

    public class FollowUpDto
    {
        public string? FollowUpAspectParId { get; set; }
        public string? ReviewerName { get; set; }
        public string? PositionFunction { get; set; }
        public string? ReviewResult { get; set; }
        public string? RiskDescription { get; set; }
        public string? RiskLevelParId { get; set; }
        public string? Notes { get; set; }
        public string? Recommendation { get; set; }
    }
}
