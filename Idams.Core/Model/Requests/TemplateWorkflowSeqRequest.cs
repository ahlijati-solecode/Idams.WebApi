using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Model.Requests
{
    public class TemplateWorkflowSeqRequest
    {
        public string workflowSequenceId { get; set; }
        public string templateId { get; set; }
        public int templateVersion { get; set; }
        public string workflowId { get; set; }
        public string workflowName { get; set; }
        public int SLA { get; set; }
        public string SLAUoM { get; set; }
        public bool isOptional { get; set; }
    }
}
