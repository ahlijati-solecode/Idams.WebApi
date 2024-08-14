using System;
using System.Collections.Generic;

namespace Idams.Core.Model.Entities
{
    public partial class TxProjectMilestone
    {
        public string ProjectId { get; set; } = null!;
        public int ProjectVersion { get; set; }
        public string WorkflowSequenceId { get; set; } = null!;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public virtual TxProjectHeader Project { get; set; } = null!;
        public virtual RefTemplateWorkflowSequence WorkflowSequence { get; set; } = null!;
    }
}
