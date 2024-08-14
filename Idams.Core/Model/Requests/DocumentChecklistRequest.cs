using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Model.Requests
{
    public class DocumentChecklistRequest
    {
        public string DocGroupParId { get; set; }
        public int DocVersion { get; set; }
        public List<string> DocList { get; set; }
    }
}
