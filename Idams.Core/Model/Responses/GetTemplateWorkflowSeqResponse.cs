using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Model.Responses
{
    public class GetTemplateWorkflowSeqResponse
    {
        public string WorkflowSequenceId { get; set; } = null!;
        public string? TemplateId { get; set; }
        public string? WorkflowType { get; set; }
        public string? WorkflowName { get; set; }
        public int? Sla { get; set; }
        public bool? isOptional { get; set; }
        public List<WorkflowSeqDocGroupResponse> documentGroup { get; set; }
    }
    public class WorkflowSeqDocGroupResponse
    {
        public string docGroupParId { get; set; }
        public string docGroup { get; set; }
        public int docVersion { get; set; }
    }
}
