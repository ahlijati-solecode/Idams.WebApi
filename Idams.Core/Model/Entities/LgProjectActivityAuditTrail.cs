using System;
using System.Collections.Generic;

namespace Idams.Core.Model.Entities
{
    public partial class LgProjectActivityAuditTrail
    {
        public string ProjectId { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public string? WorkflowSequenceId { get; set; }
        public string? Action { get; set; }
        public string? ActivityDescription { get; set; }
        public string? ActivityStatusParId { get; set; }
        public string? EmpName { get; set; }
        public string? EmpAccount { get; set; }
        public string? WorkflowActionId { get; set; }

        public virtual RefTemplateWorkflowSequence? WorkflowSequence { get; set; }
    }
}
