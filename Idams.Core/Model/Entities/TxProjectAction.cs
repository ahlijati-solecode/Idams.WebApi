using System;
using System.Collections.Generic;

namespace Idams.Core.Model.Entities
{
    public partial class TxProjectAction
    {
        public TxProjectAction()
        {
            TxApprovals = new HashSet<TxApproval>();
            TxDocuments = new HashSet<TxDocument>();
            TxFollowUps = new HashSet<TxFollowUp>();
            TxMeetings = new HashSet<TxMeeting>();
            TxProjectComments = new HashSet<TxProjectComment>();
        }

        public string ProjectActionId { get; set; } = null!;
        public string? ProjectId { get; set; }
        public string? WorkflowSequenceId { get; set; }
        public string? WorkflowActionId { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public string? AimanTransactionNumber { get; set; }

        public virtual RefWorkflowAction? WorkflowAction { get; set; }
        public virtual RefTemplateWorkflowSequence? WorkflowSequence { get; set; }
        public virtual ICollection<TxApproval> TxApprovals { get; set; }
        public virtual ICollection<TxDocument> TxDocuments { get; set; }
        public virtual ICollection<TxFollowUp> TxFollowUps { get; set; }
        public virtual ICollection<TxMeeting> TxMeetings { get; set; }
        public virtual ICollection<TxProjectComment> TxProjectComments { get; set; }
    }
}
