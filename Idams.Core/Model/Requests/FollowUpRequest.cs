using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Model.Requests
{
    public class FollowUpRequest
    {
        public int? FollowUpId { get; set; }
        public string ProjectActionId { get; set; } = null!;
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
