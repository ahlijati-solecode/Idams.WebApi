using Idams.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Model.Responses
{
    public class ProjectSequenceActionsResponse
    {
        public ProjectSequenceActionsResponse()
        {
            Actions = new List<ProjectSequenceAction>();
        }

        public List<ProjectSequenceAction> Actions { get; set; }
    }

    public class ProjectSequenceAction
    {
        public RefWorkflowAction WorkflowAction { get; set; }
        public TxProjectAction ProjectAction { get; set; }
        public List<string> ActorName { get; set; } = new List<string>();
    }
}
