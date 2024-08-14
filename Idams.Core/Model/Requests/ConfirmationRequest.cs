using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Model.Requests
{
    public class ConfirmationRequest
    {
        public string ProjectActionId { get; set; }
        public bool Approval { get; set; }
    }
}
