using System;
using System.Collections.Generic;

namespace Idams.Core.Model.Entities
{
    public partial class RefWorkflowAction
    {
        public RefWorkflowAction()
        {
            RefTemplateDocuments = new HashSet<RefTemplateDocument>();
            RefWorkflowActors = new HashSet<RefWorkflowActor>();
            TxProjectActions = new HashSet<TxProjectAction>();
        }

        public string WorkflowActionId { get; set; } = null!;
        public string? WorkflowId { get; set; }
        public string? WorkflowActionName { get; set; }
        public string? RequiredDocumentGroupParId { get; set; }
        public string? WorkflowActionTypeParId { get; set; }

        public virtual RefWorkflow? Workflow { get; set; }
        public virtual ICollection<RefTemplateDocument> RefTemplateDocuments { get; set; }
        public virtual ICollection<RefWorkflowActor> RefWorkflowActors { get; set; }
        public virtual ICollection<TxProjectAction> TxProjectActions { get; set; }
    }
}
