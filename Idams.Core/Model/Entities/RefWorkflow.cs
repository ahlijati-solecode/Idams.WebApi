using System;
using System.Collections.Generic;

namespace Idams.Core.Model.Entities
{
    public partial class RefWorkflow
    {
        public RefWorkflow()
        {
            RefTemplateWorkflowSequences = new HashSet<RefTemplateWorkflowSequence>();
            RefWorkflowActions = new HashSet<RefWorkflowAction>();
        }

        public string WorkflowId { get; set; } = null!;
        public string? WorkflowType { get; set; }
        public string? WorkflowCategoryParId { get; set; }
        public string? WorkflowMadamPk { get; set; }

        public virtual ICollection<RefTemplateWorkflowSequence> RefTemplateWorkflowSequences { get; set; }
        public virtual ICollection<RefWorkflowAction> RefWorkflowActions { get; set; }
    }
}
