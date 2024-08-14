using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Model.Requests
{
    public class GetAvailableStageWorkflowTypeRequest
    {
        public List<string> Stage { get; set; } = new List<string>();
        public List<string> WorkflowType { get; set; } = new List<string>();
    }
}
