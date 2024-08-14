using System;
using System.Collections.Generic;

namespace Idams.Core.Model.Entities
{
    public partial class RefTemplateDocument
    {
        public RefTemplateDocument()
        {
            TxDocuments = new HashSet<TxDocument>();
        }

        public string TemplateId { get; set; } = null!;
        public int TemplateVersion { get; set; }
        public string ThresholdNameParId { get; set; } = null!;
        public int ThresholdVersion { get; set; }
        public string WorkflowSequenceId { get; set; } = null!;
        public string WorkflowActionId { get; set; } = null!;
        public string? DocGroupParId { get; set; }
        public int? DocGroupVersion { get; set; }

        public virtual MdDocumentGroup? DocGroup { get; set; }
        public virtual RefTemplate Template { get; set; } = null!;
        public virtual RefThresholdProject Threshold { get; set; } = null!;
        public virtual RefWorkflowAction WorkflowAction { get; set; } = null!;
        public virtual RefTemplateWorkflowSequence WorkflowSequence { get; set; } = null!;
        public virtual ICollection<TxDocument> TxDocuments { get; set; }
    }
}
