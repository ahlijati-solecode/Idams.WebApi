using System;
using System.Collections.Generic;

namespace Idams.Core.Model.Entities
{
    public partial class RefTemplateWorkflowSequence
    {
        public RefTemplateWorkflowSequence()
        {
            LgProjectActivityAuditTrails = new HashSet<LgProjectActivityAuditTrail>();
            RefTemplateDocuments = new HashSet<RefTemplateDocument>();
            TxProjectActions = new HashSet<TxProjectAction>();
            TxProjectMilestones = new HashSet<TxProjectMilestone>();
        }

        public string WorkflowSequenceId { get; set; } = null!;
        public string? TemplateId { get; set; }
        public int? TemplateVersion { get; set; }
        public string? WorkflowId { get; set; }
        public string? WorkflowName { get; set; }
        public int? Order { get; set; }
        public int? Sla { get; set; }
        public string? SlauoM { get; set; }
        public bool? WorkflowIsOptional { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public bool? Deleted { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }

        public virtual RefTemplate? Template { get; set; }
        public virtual RefWorkflow? Workflow { get; set; }
        public virtual ICollection<LgProjectActivityAuditTrail> LgProjectActivityAuditTrails { get; set; }
        public virtual ICollection<RefTemplateDocument> RefTemplateDocuments { get; set; }
        public virtual ICollection<TxProjectAction> TxProjectActions { get; set; }
        public virtual ICollection<TxProjectMilestone> TxProjectMilestones { get; set; }
    }
}
