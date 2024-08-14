using System;
using System.Collections.Generic;

namespace Idams.Core.Model.Entities
{
    public partial class TxDocument
    {
        public string TransactionDocId { get; set; } = null!;
        public string? ProjectActionId { get; set; }
        public string? TemplateId { get; set; }
        public int? TemplateVersion { get; set; }
        public string? ThresholdNameParId { get; set; }
        public int? ThresholdVersion { get; set; }
        public string? WorkflowSequenceId { get; set; }
        public string? WorkflowActionId { get; set; }
        public string? DocName { get; set; }
        public string? DocDescriptionId { get; set; }
        public string? FilePath { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public bool? Deleted { get; set; }
        public string? FileExtension { get; set; }
        public int? FileSize { get; set; }
        public string? FileSizeUoM { get; set; }
        public bool? IsActive { get; set; }
        public string? LastUpdateWorkflowSequence { get; set; }

        public virtual MdDocumentDescription? DocDescription { get; set; }
        public virtual TxProjectAction? ProjectAction { get; set; }
        public virtual RefTemplateDocument? RefTemplateDocument { get; set; }
    }
}
