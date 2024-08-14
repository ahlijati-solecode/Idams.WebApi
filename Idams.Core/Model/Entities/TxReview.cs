using System;
using System.Collections.Generic;

namespace Idams.Core.Model.Entities
{
    public partial class TxReview
    {
        public string ReviewId { get; set; } = null!;
        public string? ProjectActionId { get; set; }
        public string? ReviewAspectParId { get; set; }
        public string? Reviewer { get; set; }
        public string? FuctionalPosition { get; set; }
        public string? ReviewResult { get; set; }
        public string? RiskDescription { get; set; }
        public string? Recomendation { get; set; }
        public string? RiskAssessmentParId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public bool? Deleted { get; set; }

        public virtual TxProjectAction? ProjectAction { get; set; }
    }
}
