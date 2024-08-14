using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Model.Dtos
{
    public class TemplateWorkflowSeqDto
    {
        public string WorkflowSequenceId { get; set; } = null!;
        public string? TemplateId { get; set; }
        public string? WorkflowType { get; set; }
        public string? WorkflowName { get; set; }
        public int? Sla { get; set; }
        public bool? isOptional { get; set; }
        public List<WorkflowSeqDocGroupDto> documentGroup { get; set; }
    }

    public class WorkflowSeqDocGroupDto
    {
        public string DocGroupParId { get; set; }
        public string DocGroup { get; set; }
        public int DocVersion { get; set; }
    }
}
