using System;
using System.Collections.Generic;

namespace Idams.Core.Model.Entities
{
    public partial class TxFollowUp
    {
        public int FollowUpId { get; set; }
        public string ProjectActionId { get; set; } = null!;
        public string? FollowUpAspectParId { get; set; }
        public string? ReviewerName { get; set; }
        public string? PositionFunction { get; set; }
        public string? ReviewResult { get; set; }
        public string? RiskDescription { get; set; }
        public string? RiskLevelParId { get; set; }
        public string? Notes { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public bool? Deleted { get; set; }
        public string? Recommendation { get; set; }

        public virtual TxProjectAction ProjectAction { get; set; } = null!;
    }
}
