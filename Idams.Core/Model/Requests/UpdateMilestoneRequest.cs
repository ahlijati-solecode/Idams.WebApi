using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Model.Requests
{
    public class UpdateMilestoneRequest
    {
        public string? ProjectId { get; set; }
        public int? ProjectVersion { get; set; }
        public string? Section { get; set; }
        public bool? SaveLog { get; set; }
        public List<MilestoneRequest> Milestone { get; set; } = new List<MilestoneRequest>();
    }

    public class MilestoneRequest
    {
        public string? WorkflowSequenceId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
