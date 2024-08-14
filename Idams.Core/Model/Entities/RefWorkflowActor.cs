using System;
using System.Collections.Generic;

namespace Idams.Core.Model.Entities
{
    public partial class RefWorkflowActor
    {
        public string WorkflowActionId { get; set; } = null!;
        public string Action { get; set; } = null!;
        public string Actor { get; set; } = null!;

        public virtual RefWorkflowAction WorkflowAction { get; set; } = null!;
    }
}
