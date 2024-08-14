using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Model.Requests
{
    public class ApprovalRequest
    {
        public string ProjectActionId { get; set; }
        public string? Notes { get; set; }
        public bool Approval { get; set; }
    }
}
