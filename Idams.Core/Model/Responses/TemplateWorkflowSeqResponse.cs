using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Model.Responses
{
    public class TemplateWorkflowSeqResponse
    {
        public string workflowSequenceId { get; set; }
        public string templateId { get; set; }
        public int templateVersion { get; set; }
    }
}
