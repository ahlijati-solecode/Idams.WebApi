using System;
using System.Collections.Generic;

namespace Idams.Core.Model.Entities
{
    public partial class RefTemplate
    {
        public RefTemplate()
        {
            RefTemplateDocuments = new HashSet<RefTemplateDocument>();
            RefTemplateWorkflowSequences = new HashSet<RefTemplateWorkflowSequence>();
            TxProjectHeaders = new HashSet<TxProjectHeader>();
        }

        public string TemplateId { get; set; } = null!;
        public int TemplateVersion { get; set; }
        public string? ThresholdNameParId { get; set; }
        public int? ThresholdVersion { get; set; }
        public string? TemplateName { get; set; }
        public string? ProjectCategoryParId { get; set; }
        public string? ProjectCriteriaParId { get; set; }
        public string? ProjectSubCriteriaParId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public bool? Deleted { get; set; }
        public int TotalWorkflow { get; set; }
        public string? StartWorkflow { get; set; }
        public string? EndWorkflow { get; set; }
        public string? Status { get; set; }

        public virtual RefThresholdProject? Threshold { get; set; }
        public virtual ICollection<RefTemplateDocument> RefTemplateDocuments { get; set; }
        public virtual ICollection<RefTemplateWorkflowSequence> RefTemplateWorkflowSequences { get; set; }
        public virtual ICollection<TxProjectHeader> TxProjectHeaders { get; set; }
    }
}
