using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Model.Requests
{
    public class UpdateInitiationDocsRequest
    {
        public string ProjectId { get; set; }
        public int ProjectVersion { get; set; }
        public string Section { get; set; }
        public bool? SaveLog { get; set; }
    }
}
