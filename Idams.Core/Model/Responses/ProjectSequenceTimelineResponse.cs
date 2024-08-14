using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Model.Responses
{
    public class ProjectSequenceTimelineResponse
    {
        public int PercentageCompleted { get; set; }
        public List<WorkflowProject> Workflows { get; set; }
    }

    public class WorkflowProject
    {
        public WorkflowProject()
        {
            WorkflowActions = new();
        }

        public string WorkflowSequenceId { get; set; }
        public string WorkflowName { get; set; }
        public string WorkflowType { get; set; }
        public string WorkflowCategory { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public string Status { get; set; }
        public int? Order { get; set; }
        public DateTime? ActionStart { get; set; }
        public DateTime? ActionEnd { get; set; }
        public List<WorkflowActionDetail> WorkflowActions { get; set; }
    }

    public class WorkflowActionDetail
    {
        public string WorkflowActionId { get; set; }
        public string WorkflowActionName { get; set; }
        public string Status { get; set; }
    }

    public class WorkflowProjectAction
    {
        public string WorkflowSequenceId { get; set; }
        public string WorkflowName { get; set; }
        public string WorkflowType { get; set; }
        public string WorkflowCategory { get; set; }
        public bool? IsOptional { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public string Status { get; set; }
        public int? Order { get; set; }
        public string WorkflowActionId { get; set; }
        public string? WorkflowActionName { get; set; }
        public DateTime? ActionStart { get; set; }
        public DateTime? ActionEnd { get; set; }
        public string? ActionStatus { get; set; }
    }
}
